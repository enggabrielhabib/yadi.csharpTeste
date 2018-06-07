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

using System.IO;
using System.IO.Ports;
using PhyLayerExceptionReason = yadi.dlms.phylayer.PhyLayerException.PhyLayerExceptionReason;
using helper = yadi.csharp.Extensions;
using System.Collections.Generic;
using System;

namespace yadi.dlms.phylayer
{

    public sealed class SerialPhyLayer : PhyLayer
	{
		private  SerialPort serialPort ;
		private readonly List<PhyLayerListener> listeners = new List<PhyLayerListener>();
		private readonly System.IO.MemoryStream stream = new System.IO.MemoryStream();
        

        /// <summary>
        /// Retrieves the list of serial ports in the system </summary>
        /// <returns> an array of Strings with the name of each serial port in the system </returns>
        public static string[] getListOfAvailableSerialPorts()
        {
            return (string[])SerialPort.GetPortNames();
        }

        /// <summary>
        /// Sets the RTS pin of the serial port </summary>
        /// <param name="enabled"> true if the RTS pin should be set, false if it should be cleared </param>
        /// <exception cref="PhyLayerException"> </exception>
        public void setRTS(bool enabled) {
            try {
                serialPort.RtsEnable = enabled;
            }
            catch (Exception) {
                throw new PhyLayerException(PhyLayerExceptionReason.INTERNAL_ERROR);
            }
        }

        /// <summary>
        /// Opens the serial port </summary>
        /// <param name="serialName"> name of the serial port to be opened </param>
        /// <exception cref="PhyLayerException"> </exception>
        public void open(string serialName)
        {
            try
            {
                serialPort = new SerialPort(serialName);
                serialPort.Open();
            }
            catch (Exception)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.BUSY_CHANNEL);
            }
        }

        //public void open(string serialName)
        //{
        //    try
        //    {
        //        serialPort = new SerialPort(serialName, 9600, Parity.None, 8, StopBits.One);
        //        serialPort.Open();
        //    }
        //    catch (Exception)
        //    {
        //        throw new PhyLayerException(PhyLayerExceptionReason.BUSY_CHANNEL);
        //    }
        //}

        /// <summary>
		/// Closes the serial port
		/// </summary>
        /// <exception cref="PhyLayerException"> </exception>
		public void close()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.INTERNAL_ERROR);
            }
        }

        /// <summary>
        /// Configures the serial port parameters </summary>
        /// <param name="baudRate"> the desired baudrate im bps </param>
        /// <param name="dataBits"> the number of data bits in each byte </param>
        /// <param name="parity"> the type of parity to be used </param>
        /// <param name="stopBits"> the number of stop bits </param>
        /// <exception cref="PhyLayerException"> </exception>
        public void config(int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            try
            {
                serialPort.BaudRate = baudRate;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = stopBits;
                serialPort.Parity = parity;
                  
            }
            catch (PhyLayerException)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.INTERNAL_ERROR);
            }
            catch (IOException)
            {
                throw new IOException();
            }
        }

        /// <summary>
        /// Sends data through the serial port </summary>
        /// <param name="data"> array of bytes to be sent </param>
        public void sendData(byte[] data)
        {
            try
            {
                serialPort.Write(data, 0, data.Length);
                
                foreach (PhyLayerListener listener in listeners)
                {
                    listener.dataSent(data);
                }
            }
            catch (Exception)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.INTERNAL_ERROR);
            }
        }

        /// <summary>
        /// Read data from the serial port </summary>
        /// <param name="timeoutMillis"> maximum time to wait for a complete frame, in milliseconds </param>
        /// <param name="parser"> a PhyLayerParser to determine when a complete frame was received </param>
        public byte[] readData(int timeoutMillis, PhyLayerParser parser)
        {
            if (timeoutMillis < 0 || parser == null)
            {
                throw new System.ArgumentException();
            }
            try
            {
                byte[] data;
                helper.extensions.resetMemoryStream(stream);
                long timeLimit = helper.extensions.nanoTime() + (timeoutMillis * 1000000L);
                while (timeLimit > helper.extensions.nanoTime())
                {
                    data = new byte[serialPort.BytesToRead];
                    serialPort.Read(data, 0, data.Length);
                    if (data != null)
                    {
                        stream.Write(data, 0, data.Length);
                        if (parser.isFrameComplete(stream.ToArray()))
                        {
                            foreach (PhyLayerListener listener in listeners)
                            {
                                listener.dataReceived(stream.ToArray());
                            }
                            return stream.ToArray();
                        }
                    }
                }
                throw new PhyLayerException(PhyLayerExceptionReason.TIMEOUT);
            }
            catch (IOException)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.BUSY_CHANNEL);
            }
            catch (PhyLayerException)
            {
                throw new PhyLayerException(PhyLayerExceptionReason.TIMEOUT);
            }
        }

        /// <summary>
        /// Adds a listener to the serial port.
        /// Each listener will receive an array of bytes containing each frame that is sent and received
        /// through the serial port
        /// </summary>
        public void addListener(PhyLayerListener listener)
		{
			listeners.Add(listener);
		}
      
    }
}