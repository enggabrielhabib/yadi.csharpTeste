using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using yadi.dlms;
using yadi.dlms.cosem;
using yadi.dlms.linklayer;
using yadi.dlms.phylayer;
using UnitTestProject1.yadi.dlms;
using System.Threading;

namespace UnitTestProject1.yadi.dlms
{
    [TestClass]
    public class GeneralTest
    {

        [TestMethod]
        public void boolTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(70, new Obis("0.0.96.3.10.255"), 2); //Get Relay Status
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();    
            byte[] expected = { 03, 01 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);

            data = new LnDescriptor(18, new Obis("0.0.44.0.0.255"), 5); // image_transfer_enabled 
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            actual = data.getResponseData();
            expected = new byte[] { 03, 01 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void bitStringTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(18, new Obis("0.0.44.0.0.255"), 3);
            HelperTest.connect(serial, hdlc, dlms, data);
            Thread.Sleep(4000);
            //data.getRequestData();
            dlms.get(serial, data);
            byte[] actual = data.getResponseData();
            byte[] expected = { 0x04, 0x82, 0x0A, 0x96, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                        00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void doubleLongTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(1, new Obis("1.0.31.128.49.255"), 2); //Read Configure clock correction factor
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x05, 0x00, 0x00, 0x00, 0x00 });
            dlms.set(serial, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 05, 00, 00, 00, 00 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void doubleLongUnsignedTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(1, new Obis("1.0.1.8.0.255"), 2); //set Active energy import register
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x06, 0x00, 0x01, 0xC1, 0x95 });
            dlms.set(serial, data);
            HelperTest.disconnect(serial, dlms);

            data = new LnDescriptor(3, new Obis("1.0.1.8.0.255"), 2); //get Active energy import register
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x06, 0x00, 0x01, 0xC1, 0x95 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);

            data = new LnDescriptor(1, new Obis("1.0.1.8.0.255"), 2); //set Active energy import register
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x06, 0x00, 0x01, 0xC1, 0xFF });
            dlms.set(serial, data);
            HelperTest.disconnect(serial, dlms);

            data = new LnDescriptor(3, new Obis("1.0.1.8.0.255"), 2); //get Active energy import register
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            actual = data.getResponseData();
            expected = new byte[] { 0x06, 0x00, 0x01, 0xC1, 0xFF };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void octetStringTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(1, new Obis("0.0.96.14.0.255"), 2); // Current tariff No.
            HelperTest.connect(serial, hdlc, dlms, data);
            //data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            var expected = new byte[] { 09, 01 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void visibleStringTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(1, new Obis("0.1.96.1.146.255"), 2); //Brazil firmware version number
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x0A, 0x06, 0x31, 0x37, 0x31, 0x31, 0x31,  0x34 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);

            data = new LnDescriptor(1, new Obis("0.0.96.1.0.255"), 2); //Meter address for communication
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x0A, 0x0A, 0x36, 0x30, 0x30, 0x39, 0x32, 0x39, 0x30, 0x39, 0x35, 0x38 });
            dlms.set(serial, data);
            data.getRequestData();
            dlms.get(serial, data);
            actual = data.getResponseData();
            expected = new byte[] { 0x0A, 0x0A, 0x36, 0x30, 0x30, 0x39, 0x32, 0x39, 0x30, 0x39, 0x35, 0x38 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void bcdTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(3, new Obis("1.0.1.2.0.255"), 2); //Active accumulated MD (+)
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x0D, 0x04, 0x00, 0x01, 0x02, 0x08 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);

            data = new LnDescriptor(3, new Obis("1.0.1.14.0.255"), 2); //Active MD (+)
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            actual = data.getResponseData();
            expected = new byte[] { 0x0D, 0x03, 0x00, 0x00, 0x00 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void integerTest()
        {
            //no examples
        }

        [TestMethod]
        public void longTest()
        {
            //not constant

            //SerialPhyLayer serial = new SerialPhyLayer();
            //HdlcLinkLayer hdlc = new HdlcLinkLayer();
            //DlmsClient dlms = new DlmsClient(hdlc);
            //LnDescriptor data = new LnDescriptor(3, new Obis("0.0.96.9.0.255"), 2); //meter temperature
            //HelperTest.connect(serial, hdlc, dlms, data);
            //data.getRequestData();
            //dlms.get(serial, data);
            //var actual = data.getResponseData();
            //byte[] expected = { 0x10, 0x00, 0x1D };
            //HelperTest.disconnect(serial, dlms);
            //CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void unsignedTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(3, new Obis("1.0.0.9.11.255"), 2); //set Clock Set the maximum offset
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x11, 0x1F });
            dlms.set(serial, data);
            HelperTest.disconnect(serial, dlms);

            data = new LnDescriptor(3, new Obis("1.0.0.9.11.255"), 2); //get Clock Set the maximum offset
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x11, 0x1F };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual); 
        }

        [TestMethod]
        public void longUnsignedTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(3, new Obis("1.0.0.3.139.255"), 2); //set Auxiliary LED second pulse cycle
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x12, 0x00, 0x00 });
            dlms.set(serial, data);
            HelperTest.disconnect(serial, dlms);

            data = new LnDescriptor(3, new Obis("1.0.0.3.139.255"), 2); //get Auxiliary LED second pulse cycle
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x12, 0x00, 0x00 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void long64Test()
        {
            //no examples
        }

        [TestMethod]
        public void long64UnsignedTest()
        {
            //no examples
        }

        [TestMethod]
        public void enumTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(18, new Obis("0.0.44.0.0.255"), 6); //image_transfer_status
            HelperTest.connect(serial, hdlc, dlms, data);
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x16, 0x04 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void float32Test()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(1, new Obis("1.0.31.128.57.255"), 2); //Zero line current
            HelperTest.connect(serial, hdlc, dlms, data);
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x17, 0x00, 0x00, 0x00, 0x00 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void float64Test()
        {
            //no examples
        }

        [TestMethod]
        public void dateTimeTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(20, new Obis("0.0.13.0.0.255"), 10); //Date&time to activate the passive calendar
            HelperTest.connect(serial, hdlc, dlms, data);
            //data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x09, 0x0C, 0x07, 0xE2, 0x05, 0x0B, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x80, 0x00, 0x02 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void dateTest()
        {
            //no examples
        }

        [TestMethod]
        public void timeTest()
        {
            //no examples
        }

        [TestMethod]
        public void arrayTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(11, new Obis("0.0.11.0.0.255"), 2); //set Holidays
            HelperTest.connect(serial, hdlc, dlms, data);
            data.setRequestData(new byte[] { 0x01, 0x02, 0x02, 0x03, 0x12, 0x00, 0x01, 0x09, 0x05, 0x07, 0xE2, 0x02, 0x0D, 0xFE, 0x11,
                                             0x02, 0x02, 0x03, 0x12, 0x00, 0x02, 0x09, 0x05, 0xFF, 0xFF, 0x01, 0x01, 0xFF, 0x11, 0x02 });
            HelperTest.disconnect(serial, dlms);
            serial.close();

            Thread.Sleep(4000);

            data = new LnDescriptor(11, new Obis("0.0.11.0.0.255"), 2); //get Holidays
            HelperTest.connect(serial, hdlc, dlms, data);
            data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x01, 0x02, 0x02, 0x03, 0x12, 0x00, 0x01, 0x09, 0x05, 0x07, 0xE2, 0x02, 0x0D, 0xFE, 0x11,
                                0x02, 0x02, 0x03, 0x12, 0x00, 0x02, 0x09, 0x05, 0xFF, 0xFF, 0x01, 0x01, 0xFF, 0x11, 0x02 };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void structureTest()
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(3, new Obis("1.0.1.8.128.255"), 2); //Forward total Active energy + 4 FEE
            HelperTest.connect(serial, hdlc, dlms, data);
            //data.getRequestData();
            dlms.get(serial, data);
            var actual = data.getResponseData();
            byte[] expected = { 0x02, 0x05, 0x06, 0x00, 0x11, 0x93, 0xF6, 0x06, 0x00, 0x03, 0x9B, 0x98, 0x06, 0x00,
                                0x0B, 0x63, 0x50, 0x06, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x02, 0x90, 0xEA };
            HelperTest.disconnect(serial, dlms);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void compactArrayTest()
        {
            //no examples
        }


    }
}
