using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SongFeedReaders.Utilities
{
    /// <summary>
    /// Represents a central place to manage a paused state.
    /// </summary>
    public class PauseTokenSource
    {
        private readonly object _lock = new object();
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        private readonly ConcurrentDictionary<Guid, object?> _pauseRequests = new ConcurrentDictionary<Guid, object?>();
        /// <summary>
        /// Gets a <see cref="PauseToken"/> associated with this <see cref="PauseTokenSource"/>
        /// </summary>
        public PauseToken Token => new PauseToken(this);
        /// <summary>
        /// Creates a new <see cref="PauseTokenSource"/>.
        /// </summary>
        public PauseTokenSource()
        {
            _tcs.TrySetResult(true);
        }
        /// <summary>
        /// Requests a pause and returns a <see cref="PauseRegistration"/>.
        /// All <see cref="PauseRegistration"/>s must be disposed to enter a resumed state.
        /// </summary>
        /// <returns></returns>
        public PauseRegistration Pause()
        {
            Guid guid = new Guid();
            while (!_pauseRequests.TryAdd(guid, null)) { guid = new Guid(); }
            lock (_lock)
            {
                if (!_pauseRequests.IsEmpty && _tcs.Task.IsCompleted)
                {
                    TaskCompletionSource<bool> newTcs = new TaskCompletionSource<bool>();
                    TaskCompletionSource<bool> oldTcs = Interlocked.Exchange(ref _tcs, newTcs);
                }
            }
            return new PauseRegistration(guid, this);
        }
        internal bool PauseRequested => !_pauseRequests.IsEmpty;
        internal Task GetPauseTask()
        {
            return _tcs.Task;
        }
        internal void DisposeRegistration(Guid guid)
        {
            _pauseRequests.TryRemove(guid, out _);
            lock (_lock)
            {
                if (_pauseRequests.IsEmpty)
                {
                    _tcs.TrySetResult(true);
                }
            }
        }
    }
    /// <summary>
    /// Represents a request to pause with the associated <see cref="PauseTokenSource"/>.
    /// Must be disposed for the <see cref="PauseTokenSource"/> to resume.
    /// </summary>
    public struct PauseRegistration : IDisposable
    {
        private Guid _id;
        private PauseTokenSource _source;

        internal PauseRegistration(Guid id, PauseTokenSource source)
        {
            _id = id;
            _source = source;
        }
        /// <summary>
        /// Disposes the registration, indicating to the <see cref="PauseTokenSource"/>
        /// that it can resume if there are no other active registrations.
        /// </summary>
        public void Dispose()
        {
            _source.DisposeRegistration(_id);
        }
    }

    /// <summary>
    /// A token linked with a <see cref="PauseTokenSource"/> that provides a <see cref="Task"/>
    /// that will complete with then source is unpaused.
    /// </summary>
    public struct PauseToken
    {
        /// <summary>
        /// A <see cref="PauseToken"/> that will never be paused.
        /// </summary>
        public static PauseToken None => new PauseToken(null);
        private readonly PauseTokenSource? _source;
        internal PauseToken(PauseTokenSource? source)
        {
            _source = source;
        }
        /// <summary>
        /// Returns true if this <see cref="PauseToken"/> can be in a paused state.
        /// </summary>
        public bool CanPause => _source != null;
        /// <summary>
        /// Returns true if the associated <see cref="PauseTokenSource"/> state is paused.
        /// </summary>
        public bool IsPauseRequested
        {
            get
            {
                return _source?.PauseRequested ?? false;
            }
        }
        /// <summary>
        /// Returns a <see cref="Task"/> that will complete when the associated <see cref="PauseTokenSource"/>
        /// is unpaused, or the given <paramref name="cancellationToken"/> is cancelled.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WaitForPause(CancellationToken cancellationToken)
        {
            if (!CanPause || _source == null)
                return;
            Task pauseTask = _source.GetPauseTask();
            if (pauseTask.IsCompleted)
                return;
            if (cancellationToken.CanBeCanceled)
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                using CancellationTokenRegistration reg = cancellationToken.Register(() => tcs.TrySetResult(false));
                if (!cancellationToken.IsCancellationRequested)
                    await Task.WhenAny(pauseTask, tcs.Task).ConfigureAwait(false);
                tcs.TrySetResult(true);
            }
            else
                await pauseTask.ConfigureAwait(false);
        }
    }
}
