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

namespace yadi.dlms.linklayer
{
    public class HdlcParameters
    {

        internal readonly int windowSizeTx = 1; //windowSizeTx != 1 not supported
        internal readonly int windowSizeRx = 1; //windowSizeTx != 1 not supported
        public int timeoutMillis = 1500;
        public int maxInformationFieldLengthTx = 128;
        public int maxInformationFieldLengthRx = 128;
        public byte[] serverAddress = new byte[] { (byte)0x00, (byte)0x02, (byte)0xFE, (byte)0xFF };
        public byte clientAddress = (byte)0x03;
        
        public void setMaxInformationFieldLength(int maxLength)
        {
            if (maxLength < 0)
            {
                throw new System.ArgumentException("Information field length must be positive");
            }
            maxInformationFieldLengthTx = maxInformationFieldLengthRx = maxLength;
        }

        public void setTimeout(int millis)
        {
            if (millis < 0)
            {
                throw new System.ArgumentException("Timeout must be positive");
            }
            this.timeoutMillis = millis;
        }

        public void setClientAddress(int clientAddress)
        {
            if (clientAddress < 0 || clientAddress > 0x7F)
            {
                throw new System.ArgumentException("Maximum client address is 0x7F");
            }
            this.clientAddress = (byte)((clientAddress << 1) | 0x01);
        }

        public void setServerAddress(int serverAddr)
        {
            if (serverAddr < 0 || serverAddr > 0x7F)
            {
                throw new System.ArgumentException("Maximum client address is 0x7F");
            }
            this.serverAddress = new byte[] { (byte)((serverAddr << 1) | 0x01) };
        }

        public void setServerAddress(int upperAddress, int lowerAddress)
        {
            if (lowerAddress < 0 || lowerAddress > 0x3FFF || upperAddress < 0 || upperAddress > 0x3FFF)
            {
                throw new System.ArgumentException("Maximum address is 0x3FFF");
            }
            this.serverAddress = new byte[] {   (byte)(((uint)upperAddress >> 6) & 0xFE),
                                                (byte)((upperAddress << 1)  & 0xFE),
                                                (byte)(((uint)lowerAddress >> 6) & 0xFE),
                                                (byte)((lowerAddress << 1)  | 0x01)};
        }

    }

}


