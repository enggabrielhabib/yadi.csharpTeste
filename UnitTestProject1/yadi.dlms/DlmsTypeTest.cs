using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace yadi.dlms
{

    [TestClass]
    public class TestDlmsType
    {

        [TestMethod]
        public  void testDlmsTypeTag()
        {
            Assert.AreEqual(1, DlmsType.ARRAY.tag);
            Assert.AreEqual(2, DlmsType.STRUCTURE.tag);
            Assert.AreEqual(3, DlmsType.BOOLEAN.tag);
            Assert.AreEqual(4, DlmsType.BITSTRING.tag);
            Assert.AreEqual(5, DlmsType.INT32.tag);
            Assert.AreEqual(6, DlmsType.UINT32.tag);
            Assert.AreEqual(9, DlmsType.OCTET_STRING.tag);
            Assert.AreEqual(10, DlmsType.STRING.tag);
            Assert.AreEqual(12, DlmsType.UTF8_STRING.tag);
            Assert.AreEqual(13, DlmsType.BCD.tag);
            Assert.AreEqual(15, DlmsType.INT8.tag);
            Assert.AreEqual(16, DlmsType.INT16.tag);
            Assert.AreEqual(17, DlmsType.UINT8.tag);
            Assert.AreEqual(18, DlmsType.UINT16.tag);
            Assert.AreEqual(20, DlmsType.INT64.tag);
            Assert.AreEqual(21, DlmsType.UINT64.tag);
            Assert.AreEqual(22, DlmsType.ENUM.tag);
            Assert.AreEqual(23, DlmsType.FLOAT32.tag);
            Assert.AreEqual(24, DlmsType.FLOAT64.tag);
            Assert.AreEqual(25, DlmsType.DATE_TIME.tag);
            Assert.AreEqual(26, DlmsType.DATE.tag);
            Assert.AreEqual(27, DlmsType.TIME.tag);
        }

        [TestMethod]
        public  void testDlmsTypeSize()
        {
            Assert.AreEqual(0, DlmsType.ARRAY.size);
            Assert.AreEqual(0, DlmsType.STRUCTURE.size);
            Assert.AreEqual(1, DlmsType.BOOLEAN.size);
            Assert.AreEqual(0, DlmsType.BITSTRING.size);
            Assert.AreEqual(4, DlmsType.INT32.size);
            Assert.AreEqual(4, DlmsType.UINT32.size);
            Assert.AreEqual(0, DlmsType.OCTET_STRING.size);
            Assert.AreEqual(0, DlmsType.STRING.size);
            Assert.AreEqual(0, DlmsType.UTF8_STRING.size);
            Assert.AreEqual(0, DlmsType.BCD.size);
            Assert.AreEqual(1, DlmsType.INT8.size);
            Assert.AreEqual(2, DlmsType.INT16.size);
            Assert.AreEqual(1, DlmsType.UINT8.size);
            Assert.AreEqual(2, DlmsType.UINT16.size);
            Assert.AreEqual(8, DlmsType.INT64.size);
            Assert.AreEqual(8, DlmsType.UINT64.size);
            Assert.AreEqual(1, DlmsType.ENUM.size);
            Assert.AreEqual(4, DlmsType.FLOAT32.size);
            Assert.AreEqual(8, DlmsType.FLOAT64.size);
            Assert.AreEqual(12, DlmsType.DATE_TIME.size);
            Assert.AreEqual(5, DlmsType.DATE.size);
            Assert.AreEqual(4, DlmsType.TIME.size);
        }
    }
}

