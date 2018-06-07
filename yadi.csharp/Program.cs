using System;
using System.Text;
using System.Threading;
using yadi.dlms;
using yadi.dlms.cosem;
using yadi.dlms.linklayer;
using yadi.dlms.phylayer;
using static yadi.dlms.cosem.CosemParameters;
using static yadi.dlms.DlmsException;


namespace yadi.csharp.app
{
    class getActiveEnergyTest
    {
        static void Main(string[] args)
        {
            SerialPhyLayer serial = new SerialPhyLayer();
            HdlcLinkLayer hdlc = new HdlcLinkLayer();
            DlmsClient dlms = new DlmsClient(hdlc);
            LnDescriptor data = new LnDescriptor(18, new Obis("0.0.44.0.0.255"), 3);

        

            try
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
                //hdlc.getParameters().setServerAddress(0x0001, 0x3FFF);
                hdlc.getParameters().setServerAddress(0x01); //emode meter addr

                //dlms.getParameters().setAuthenticationType(AuthenticationType.LLS);

                dlms.getParameters().setAuthenticationType(AuthenticationType.HLS_GMAC);
                dlms.getParameters().setSecurityType(SecurityType.AUTHENTICATION_ENCRYPTION);

                dlms.connect(serial);

                for (int i = 0; i < 1; ++i)
                {
                    Console.WriteLine(i);
                    //data.getRequestData();
                    dlms.get(serial, data);
                    Console.Write(ByteArrayToString(data.getResponseData()));
                    Thread.Sleep(4000);
                }
                dlms.disconnect(serial);

            }
            catch (PhyLayerException e)
            {
                Console.WriteLine(e.getReason());
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (LinkLayerException e)
            {
                Console.WriteLine(e.getReason());
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (DlmsException e)
            {
                foreach (DlmsExceptionReason r in e.getReason())
                {
                    Console.WriteLine(r);
                }
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            finally
            {
                serial.close();
            }
            Console.ReadLine();

        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2} ", b);
            return hex.ToString();
        }
    }
}

