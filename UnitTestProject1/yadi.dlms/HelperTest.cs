using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yadi.dlms;
using yadi.dlms.cosem;
using yadi.dlms.linklayer;
using yadi.dlms.phylayer;
using static yadi.dlms.cosem.CosemParameters;

namespace UnitTestProject1.yadi.dlms
{
    public class HelperTest
    {
        public static void connect(SerialPhyLayer serial, HdlcLinkLayer hdlc, DlmsClient dlms, LnDescriptor data)
        {
            

            serial.open("COM3");
            serial.config(300, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.One);
            try
            {
                Emode.connect(serial);
            }
            catch (Exception)
            {

            }

            serial.config(9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
            hdlc.getParameters().setClientAddress(0x01);
            hdlc.getParameters().setServerAddress(0x01); //emode meter addr

            dlms.getParameters().setAuthenticationType(AuthenticationType.HLS_GMAC);
            dlms.getParameters().setSecurityType(SecurityType.AUTHENTICATION_ENCRYPTION);

            dlms.connect(serial);
        }
        public static void disconnect(SerialPhyLayer serial, DlmsClient dlms)
        {
            dlms.disconnect(serial);
            serial.close();


        }
    }
}
