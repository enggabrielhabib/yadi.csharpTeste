
/*
 * YADI (Yet Another DLMS Implementation)
 * Copyright (C) 2018 Paulo Faco (paulofaco@gmail.com)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


/*
* YADI.CSHARP
* Ported from Java to C# by Gabriel.Habib (enggabrielhabib@gmail.com) & Enzo Furlan (enzo.furlan@live.com)
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace yadi.dlms.linklayer
{

    using PhyLayer = yadi.dlms.phylayer.PhyLayer;
    using PhyLayerParser = yadi.dlms.phylayer.PhyLayerParser;
    using PhyLayerException = yadi.dlms.phylayer.PhyLayerException;

    public class Emode
    {
                /// <summary>
        /// Performs the steps necessary for a connection in the MODE-E </summary>
        /// <param name="com"> PhyLayer object to be used for data tx/rx </param>
        /// <exception cref="PhyLayerException"> </exception>

        public static void connect(PhyLayer com)
        {
            //PhyParserIsFrameComplete phyLayerPaser = new PhyParserIsFrameComplete();
            try
            {
                com.sendData(Encoding.ASCII.GetBytes("/?!\r\n"));
                byte[] rxBuff = com.readData(1000, new PhyParserIsFrameComplete());
                byte baud = ((byte)rxBuff[4] & (byte)0xFF) > (byte)0x35 ? (byte)0x35 : (byte)rxBuff[4];
                com.sendData(new byte[] { 0x06, 0x32, baud, 0x32, 0x0D, 0x0A });
                Thread.Sleep(550);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }


        public class PhyParserIsFrameComplete : PhyLayerParser {
            public bool isFrameComplete(byte[] data)
            {
                return data.Length >= 15 && data[data.Length - 2] == 0x0D && data[data.Length - 1] == 0x0A;
            }
        }

    }
  
}
