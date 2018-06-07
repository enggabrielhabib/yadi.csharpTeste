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
using yadi.dlms.phylayer;

namespace yadi.dlms.linklayer
{

    using LinkLayerExceptionReason = yadi.dlms.linklayer.LinkLayerException.LinkLayerExceptionReason;
	using PhyLayer = yadi.dlms.phylayer.PhyLayer;
    using helper = yadi.csharp.Extensions;
    using PhyLayerException = yadi.dlms.phylayer.PhyLayerException;
    class WrapperLinkLayer : LinkLayer
    {

        private static readonly short WRAPPER_VERSION = 1;
        private readonly WrapperParameters parameters;
        
        /// <summary>
		/// Creates a WrapperLinkLayer object </summary>
		/// <param name="params"> the WrapperParameters for this object </param>
		public WrapperLinkLayer(WrapperParameters parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Creates a WrapperLinkLayer object
        /// </summary>
        public WrapperLinkLayer() : this(new WrapperParameters())
        {

        }

        /// <summary>
        /// Wrapper doesn't have a connection procedure, this function doesn't need to be called when
        /// using the Wrapper protocol as link layer for the COSEM APDU's. </summary>
        /// <param name="phy"> the PhyLayer to transmit and receive bytes </param>
        public void connect(PhyLayer phy)
        {
            //no connection necessary
        }

        /// <summary>
        /// Wrapper doesn't have a disconnection procedure, this function doesn't need to be called when
        /// using the Wrapper protocol as link layer for the COSEM APDU's. </summary>
        /// <param name="phy"> the PhyLayer to transmit and receive bytes </param>
        public void disconnect(PhyLayer phy)
        {
            //no disconnection necessary
        }


        /// <summary>
        /// Encapsulates data inside a Wrapper frame and sends it </summary>
        /// <param name="phy"> the PhyLayer to transmit and receive bytes </param>
        /// <param name="data"> the array of bytes to be encapsulated and transmitted </param>
        public void send(PhyLayer phy, byte[] data)
        {
            
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            stream.WriteByte((byte)(WRAPPER_VERSION >> 8));
            stream.WriteByte((byte)WRAPPER_VERSION);
            stream.WriteByte((byte)(parameters.wPortSource >> 8));
            stream.WriteByte((byte)parameters.wPortSource);
            stream.WriteByte((byte)(parameters.wPortDestination >> 8));
            stream.WriteByte((byte)parameters.wPortDestination);
            stream.WriteByte((byte)data.Length);
            
            foreach (byte b in data)
            {
                   stream.WriteByte(b);
            }
            phy.sendData(stream.ToArray());
            
        }

        /// <summary>
        /// Retrieves the data encapsulated inside a Wrapper frame </summary>
        /// <param name="phy"> the PhyLayer to receive bytes </param>
        /// <returns> array of bytes with the application data unit contents inside the Wrapper frame </returns>
        public byte[] read(PhyLayer phy)
        {
            byte[] data = phy.readData(parameters.timeoutMillis, new PhyParserIsFrameComplete());
            short version = BitConverter.ToInt16(new byte[2] { data[1], data[0] },0);
            short wPortSource = BitConverter.ToInt16(new byte[2] { data[3], data[2] }, 0);
            short wPortDestination = BitConverter.ToInt16(new byte[2] { data[5], data[4] }, 0);

            if (version != WRAPPER_VERSION)
            {
                throw new LinkLayerException(LinkLayerExceptionReason.RECEIVED_INVALID_FRAME_FORMAT);
            }

            if (wPortSource != parameters.wPortDestination || wPortDestination != parameters.wPortSource)
            {
                throw new LinkLayerException(LinkLayerExceptionReason.RECEIVED_INVALID_ADDRESS);
            }
            return helper.extensions.copyOfRange(data, 8, data.Length);
        }

        public class PhyParserIsFrameComplete : PhyLayerParser
        {
            public bool isFrameComplete(byte[] data)
            {
                if (data.Length < 8)
                {
                    return false;
                }
                short size = BitConverter.ToInt16(new byte[2] { data[1], data[0] }, 0);

                if (data.Length < size + 8)
                {
                    return false;
                }
                return true;
            }
        }

    }

}
