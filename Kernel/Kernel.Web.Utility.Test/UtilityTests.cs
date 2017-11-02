using NUnit.Framework;

namespace Kernel.Web.Test
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void IsLocalPTest()
        {
            //ARRANGE
            var host = "dg-mfb";
            var localhost = "localhost";
            var other = "www.testshib.org";
            //ACT
            var isLocal = Utility.IsLocalIpAddress(host);
            var isLocalLocalhost = Utility.IsLocalIpAddress(localhost);
            var isLocalOther = Utility.IsLocalIpAddress(other);
            //ASSERT
            Assert.IsTrue(isLocal);
            Assert.IsTrue(isLocalLocalhost);
            Assert.False(isLocalOther);
        }
    }
}
