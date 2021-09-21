using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SongFeedReadersTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Method());
        }

        void Method()
        {
            try
            {
                throw new ArgumentNullException();
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                Assert.Fail();
            }

        }
    }
}
