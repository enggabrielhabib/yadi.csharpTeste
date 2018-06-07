using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yadi.dlms.linklayer
{
    [TestClass]
    public class TestHdlcParameters
    {
        [TestMethod]
        public void testSetServerAddress()
        {
            HdlcParameters hdlcParams = new HdlcParameters();
            hdlcParams.setServerAddress(0x3FFE, 0x3FFF);
            byte[] expected = new byte[] { (0xFE), (0xFC), (0xFE), (0xFF) };
            CollectionAssert.AreEqual(expected, hdlcParams.serverAddress);
        }

        [TestMethod]
        public void testSetClientAddress()
        {
            HdlcParameters hdlcParams = new HdlcParameters();
            hdlcParams.setClientAddress(0x01);
            Assert.AreEqual(0x03, hdlcParams.clientAddress);

        }
    }

}
