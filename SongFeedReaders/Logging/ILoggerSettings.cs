namespace SongFeedReaders.Logging
{
    public interface ILoggerSettings
    {
        string? ModuleName { get; set; }
        LogLevel LogLevel { get; set; }
        bool ShowModule { get; set; }
        bool ShortSource { get; set; }
        bool EnableTimeStamp { get; set; }
    }
}
