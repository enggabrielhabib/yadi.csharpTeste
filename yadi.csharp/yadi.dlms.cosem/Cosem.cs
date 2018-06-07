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


namespace yadi.dlms.cosem
{

	using DlmsExceptionReason = yadi.dlms.DlmsException.DlmsExceptionReason;
	using AuthenticationType = yadi.dlms.cosem.CosemParameters.AuthenticationType;
	using SecurityType = yadi.dlms.cosem.CosemParameters.SecurityType;
    using DlmsClass = yadi.dlms.cosem.CosemClasses;
    using System.IO;
    using helper = yadi.csharp.Extensions;
    using System;

    public class Cosem
	{

		private enum ConnectionState
		{
			DISCONNECTED,
			CONNECTED,
			AUTHENTICATED
		}

		private readonly CosemParameters parameters;
		private readonly CosemConnection connection;
		private ConnectionState state = ConnectionState.DISCONNECTED;

		/// <summary>
		/// Creates a Cosem object
		/// </summary>
		public Cosem() : this(new CosemParameters())
		{
		}

		/// <summary>
		/// Creates a Cosem object </summary>
		/// <param name="params"> CosemParameters for this Cosem object </param>
		public Cosem(CosemParameters parameters)
		{
			this.parameters = parameters;
			this.connection = new CosemConnection();
		}

		public CosemParameters getParameters()
		{
            return parameters;
		}

		/// <summary>
		/// Resets the internal connection state, must be called before each connection attempt
		/// </summary>
		public void reset()
		{
			state = ConnectionState.DISCONNECTED;
		}

		/// <summary>
		/// Generates the next APDU for establishment of a association with the metering devices
		/// The number and content of the APDU will vary according to the HdlcParams configuration </summary>
		/// <returns> the array of bytes that represents the next APDU to be sent for connection establishment </returns>
		/// <exception cref="DlmsException"> </exception>
		public byte[] connectionRequest()
		{
			try
			{
				switch (state)
				{
				case yadi.dlms.cosem.Cosem.ConnectionState.DISCONNECTED:
					connection.reset();
					return Aarq.request(parameters, connection); //OK
				case yadi.dlms.cosem.Cosem.ConnectionState.CONNECTED:
					LnDescriptor att = new LnDescriptor(DlmsClass.ASSOCIATION_LN.id, new Obis("0.0.40.0.0.255"), 1);
					byte[] data = Security.processChallanger(parameters, connection);
					System.IO.MemoryStream stream = new System.IO.MemoryStream();
					stream.WriteByte(Constants.DataType.OCTET_STRING);
					stream.WriteByte((byte)data.Length);

                    //Array.Reverse(data);
					stream.Write(data, 0, data.Length);

					att.setRequestData(stream.ToArray());
					return requestAction(att);
				case yadi.dlms.cosem.Cosem.ConnectionState.AUTHENTICATED:
					throw new System.InvalidOperationException();
				default:
					throw new System.InvalidOperationException();
				}
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Parses the response of the last connection APDU sent </summary>
		/// <param name="data"> array of bytes with the APDU received from the metering device </param>
		/// <returns> true if the connection is established and false if more steps are necessary </returns>
		/// <exception cref="DlmsException"> </exception>
		public bool parseConnectionResponse(byte[] data)
		{
			switch (state)
			{
			case yadi.dlms.cosem.Cosem.ConnectionState.DISCONNECTED:
				Aare.parseResponse(parameters, connection, data);
				state = ConnectionState.CONNECTED;
				return parameters.authenticationType == AuthenticationType.PUBLIC || parameters.authenticationType == AuthenticationType.LLS;
			case yadi.dlms.cosem.Cosem.ConnectionState.CONNECTED:
				LnDescriptor att = new LnDescriptor(DlmsClass.ASSOCIATION_LN.id, new Obis("0.0.40.0.0.255"), 1);
				parseActionResponse(att, data);
				byte[] receivedData = att.getResponseData();
				if (receivedData == null || receivedData.Length < 3 || receivedData[0] != Constants.DataType.OCTET_STRING)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.CONNECTION_REJECTED);
				}
				if (receivedData[1] != receivedData.Length - 2)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.CONNECTION_REJECTED);
				}
				if (!Security.verifyChallenger(parameters, connection, helper.extensions.copyOfRange(receivedData, 2, receivedData.Length)))
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.FAIL_TO_AUTHENTICATE_SERVER);
				}
				state = ConnectionState.AUTHENTICATED;
				return true;
			case yadi.dlms.cosem.Cosem.ConnectionState.AUTHENTICATED:
				throw new System.InvalidOperationException();
			default:
				throw new System.InvalidOperationException();
			}
		}

		/// <summary>
		/// Generates the APDU for a GET request </summary>
		/// <param name="att"> LnDescriptor describing the object to be accessed </param>
		/// <returns> byte array representation of the APDU </returns>
		/// <exception cref="DlmsException"> </exception>
		public byte[] requestGet(LnDescriptor att)
		{
			try
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream();
				stream.WriteByte(Constants.xDlmsApdu.NoCiphering.GET_REQUEST);
				stream.WriteByte((byte)(connection.datablock.blockNum == 0 ? 1 : 2));
				stream.WriteByte((byte)(parameters.priority | parameters.serviceClass | Constants.INVOKE_ID));
				if (connection.datablock.blockNum != 0)
				{
                    //stream.WriteByte(ByteBuffer.allocate(4).putInt(connection.datablock.blockNum).array());
                    stream.Write(new byte[] { (byte)(connection.datablock.blockNum >> 24), (byte)(connection.datablock.blockNum >> 16), (byte)(connection.datablock.blockNum >> 8), (byte)(connection.datablock.blockNum) }, 0, 4);
				}
				else
				{
                    //Array.Reverse(att.getClassId());
                    stream.Write(att.getClassId(), 0, att.getClassId().Length);
                    //Array.Reverse(att.getObis());
					stream.Write(att.getObis(), 0, att.getObis().Length);
					stream.WriteByte((byte)(att.getIndex()));
					stream.WriteByte((byte)(att.getRequestData().Length == 0 ? 0 : 1));
                    //Array.Reverse(att.getRequestData());
                    stream.Write(att.getRequestData(), 0, att.getRequestData().Length);
				}
				return packFrame(Constants.xDlmsApdu.GlobalCiphering.GET_REQUEST, stream.ToArray());
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Generates the APDU for a SET request </summary>
		/// <param name="att"> LnDescriptor describing the object to be accessed </param>
		/// <returns> byte array representation of the APDU </returns>
		/// <exception cref="DlmsException"> </exception>
		public byte[] requestSet(LnDescriptor att)
		{
			try
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream();
				stream.WriteByte(Constants.xDlmsApdu.NoCiphering.SET_REQUEST);
				byte[] data = att.getRequestData();
				if (data.Length > (connection.maxPduSize - 50))
				{
					if (connection.datablock.nextBlockNum == 1)
					{
						connection.datablock.setData(data, connection.maxPduSize - 50);
						//Set-Request-With-First-Datablock
						stream.WriteByte(2);
						stream.WriteByte((byte)(parameters.priority | parameters.serviceClass | Constants.INVOKE_ID));
                        //Array.Reverse(att.getClassId());
                        stream.Write(att.getClassId(), 0, att.getClassId().Length);
                        //Array.Reverse(att.getObis());
                        stream.Write(att.getObis(), 0, att.getObis().Length);
                        stream.WriteByte((byte)(att.getIndex()));
                        stream.WriteByte(0);
						stream.WriteByte(0);
                        //Array.Reverse(connection.datablock.getNextBlockNum());
						stream.Write(connection.datablock.getNextBlockNum(), 0, connection.datablock.getNextBlockNum().Length);
						byte[] blockdata = connection.datablock.getNextBlock();
						if (blockdata.Length > 128)
						{
							stream.WriteByte(0x81);
						}
						stream.WriteByte((byte)(blockdata.Length));
                        //Array.Reverse(blockdata);
						stream.Write(blockdata, 0, blockdata.Length);
					}
					else
					{
						//Set-Request-With-Datablock
						stream.WriteByte(3);
						stream.WriteByte((byte)(parameters.priority | parameters.serviceClass | Constants.INVOKE_ID));
						if (connection.datablock.nextIsLast())
						{
							stream.WriteByte(1);
						}
						else
						{
							stream.WriteByte(0);
						}
                        //Array.Reverse(connection.datablock.getNextBlockNum());
                        stream.Write(connection.datablock.getNextBlockNum(), 0, connection.datablock.getNextBlockNum().Length);
                        byte[] blockdata = connection.datablock.getNextBlock();
                        if (blockdata.Length > 128)
						{
							stream.WriteByte(0x81);
						}
						stream.WriteByte((byte)(blockdata.Length));
                        //Array.Reverse(blockdata);
						stream.Write(blockdata, 0, blockdata.Length);
					}
				}
				else
				{
					//Set-Request-Normal
					stream.WriteByte(1);
					stream.WriteByte((byte)(parameters.priority | parameters.serviceClass | Constants.INVOKE_ID));
                    //Array.Reverse(att.getClassId());
                    stream.Write(att.getClassId(), 0, att.getClassId().Length);
                    //Array.Reverse(att.getObis());
                    stream.Write(att.getObis(), 0, att.getObis().Length);
                    stream.WriteByte((byte)(att.getIndex()));
                    stream.WriteByte(0);
                    //Array.Reverse(att.getRequestData());
                    stream.Write(att.getRequestData(), 0, att.getRequestData().Length);
                }

				return packFrame(Constants.xDlmsApdu.GlobalCiphering.SET_REQUEST, stream.ToArray());
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Generates the APDU for a ACTION request </summary>
		/// <param name="att"> LnDescriptor describing the object to be accessed </param>
		/// <returns> byte array representation of the APDU </returns>
		/// <exception cref="DlmsException"> </exception>
		public byte[] requestAction(LnDescriptor att)
		{
			try
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream();
				stream.WriteByte(Constants.xDlmsApdu.NoCiphering.ACTION_REQUEST);
				stream.WriteByte(1);
				stream.WriteByte((byte)(parameters.priority | parameters.serviceClass | Constants.INVOKE_ID));
                //Array.Reverse(att.getClassId());
                stream.Write(att.getClassId(), 0, att.getClassId().Length);
                //.Reverse(att.getObis());
                stream.Write(att.getObis(), 0, att.getObis().Length);
                stream.WriteByte((byte)(att.getIndex()));
                stream.WriteByte((byte)(att.getRequestData().Length == 0 ? 0 : 1));
                //Array.Reverse(att.getRequestData());
                stream.Write(att.getRequestData(), 0, att.getRequestData().Length);
                return packFrame(Constants.xDlmsApdu.GlobalCiphering.ACTION_REQUEST, stream.ToArray());
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Parses the APDU of a GET response </summary>
		/// <param name="att"> LnDescriptor describing the object accessed </param>
		/// <returns> true if the parse if finished, false if more apdu's are necessary (data block transfer) </returns>
		/// <exception cref="DlmsException"> </exception>
		public bool parseGetResponse(LnDescriptor att, byte[] data)
		{
			try
			{
				data = unpackFrame(Constants.xDlmsApdu.NoCiphering.GET_RESPONSE, Constants.xDlmsApdu.GlobalCiphering.GET_RESPONSE, data);

				if (data.Length < 4)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_GET_RESPONSE);
				}

				if (data[0] == Constants.GetResponse.NORMAL)
				{
					verifyDataAccessResult(data[2]);
					connection.datablock.lastBlock = true;
					data = helper.extensions.copyOfRange(data, 3, data.Length);
				}
				else if (data[0] == Constants.GetResponse.DATA_BLOCK)
				{
					if (data.Length < 10 || data[7] != 0)
					{ //TODO only supports raw-data for now
						throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_GET_RESPONSE);
					}
					connection.datablock.lastBlock = data[2] != 0;
                    //connection.datablock.blockNum = ByteBuffer.allocate(4).put(data,3,4).getInt(0);
                    connection.datablock.blockNum = BitConverter.ToInt32(new byte[] { data[6], data[5], data[4], data[3] }, 0);
					data = getPayload(data, 8);
				}
				else
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_GET_RESPONSE);
				}
                //Array.Reverse(data);
				connection.datablock.data.Write(data, 0, data.Length);

				if (connection.datablock.lastBlock)
				{
					att.setResponseData(connection.datablock.data.ToArray());
					connection.datablock.reset();
					return true;
				}

				return false;
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		/// <summary>
		/// Parses the APDU of a SET response </summary>
		/// <param name="att"> LnDescriptor describing the object accessed </param>
		/// <returns> true if the parse if finished, false if more apdu's are necessary (data block transfer) </returns>
		/// <exception cref="DlmsException"> </exception>
		public bool parseSetResponse(LnDescriptor att, byte[] data)
		{
			data = unpackFrame(Constants.xDlmsApdu.NoCiphering.SET_RESPONSE, Constants.xDlmsApdu.GlobalCiphering.SET_RESPONSE, data);

			if (data.Length < 3)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_SET_RESPONSE);
			}

			if (data[0] == Constants.SetResponse.NORMAL)
			{
				verifyDataAccessResult(data[2]);
				return true;
			}
			else if (data[0] == Constants.SetResponse.DATA_BLOCK)
			{
				if (data.Length < 6)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_SET_RESPONSE);
				}
                //connection.datablock.ackBlock(ByteBuffer.allocate(4).put(data,2,4).getInt(0));
                connection.datablock.ackBlock(BitConverter.ToInt32(new byte[] { data[5], data[4], data[3], data[2] }, 0));
				return connection.datablock.nextIsLast();
			}
			else if (data[0] == Constants.SetResponse.LAST_DATA_BLOCK)
			{
				if (data.Length < 7)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_SET_RESPONSE);
				}
				//if (ByteBuffer.allocate(4).put(data,3,4).getInt(0) != connection.datablock.nextBlockNum)
                if((BitConverter.ToInt32(new byte[] { data[6], data[5], data[4], data[3] }, 0)) != connection.datablock.nextBlockNum)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_SET_RESPONSE);
				}
                //connection.datablock.ackBlock(ByteBuffer.allocate(4).put(data,3,4).getInt(0));
                connection.datablock.ackBlock(BitConverter.ToInt32(new byte[] { data[6], data[5], data[4], data[3] }, 0));
                verifyDataAccessResult(data[2]);
				return true;
			}
			throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_SET_RESPONSE);
		}

		/// <summary>
		/// Parses the APDU of a ACTION response </summary>
		/// <param name="att"> LnDescriptor describing the object accessed </param>
		/// <returns> true if the parse if finished, false if more apdu's are necessary (data block transfer) </returns>
		/// <exception cref="DlmsException"> </exception>
		public bool parseActionResponse(LnDescriptor att, byte[] data)
		{
			data = unpackFrame(Constants.xDlmsApdu.NoCiphering.ACTION_RESPONSE, Constants.xDlmsApdu.GlobalCiphering.ACTION_RESPONSE, data);

			if (data.Length > 6)
			{
				att.setResponseData(helper.extensions.copyOfRange(data, 5, data.Length));
			}

			return true;
		}

		private void verifyDataAccessResult(byte result)
		{
			if (result == 0)
			{
				return;
			}
			foreach (Constants.AccessResult a in Constants.AccessResult.values())
			{
				if (a.val == result)
				{
					//throw new DlmsException(DlmsException.DlmsExceptionReason.GetValues(a.ToString()));
                }
			}
			throw new DlmsException(DlmsException.DlmsExceptionReason.UNKNOWN_ACCESS_RESULT_FAILURE);
		}

		private byte[] packFrame(int cmdGlobalCipher, byte[] payload)
		{
			try
			{
				if (parameters.securityType != SecurityType.NONE)
				{
					System.IO.MemoryStream stream = new System.IO.MemoryStream();
					byte[] data = Security.authenticatedEncryption(parameters, payload);
					stream.SetLength(0);
					stream.WriteByte((byte)cmdGlobalCipher);
                    //Array.Reverse(getSizeBytes(data.Length));
					stream.Write(getSizeBytes(data.Length), 0, getSizeBytes(data.Length).Length);
                    //Array.Reverse(data);
                    stream.Write(data, 0, data.Length);
					return stream.ToArray();
				}
				return payload;
			}
			catch (IOException)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
			}
		}

		private byte[] unpackFrame(int cmdNoCipher, int cmdGlobalCipher, byte[] data)
		{
			if ((data[0] & 0xFF) == Constants.xDlmsApdu.Exception.ExceptionResponse)
			{
				DlmsException.DlmsExceptionReason[] reasons = new DlmsException.DlmsExceptionReason[2];
				reasons[0] = DlmsException.DlmsExceptionReason.STATE_ERROR_UNKNOWN;
				reasons[1] = DlmsException.DlmsExceptionReason.SERVICE_ERROR_UNKNOWN;

				if (data[1] == 1)
				{
					reasons[0] = DlmsException.DlmsExceptionReason.STATE_ERROR_SERVICE_NOT_ALLOWED;
				}
				else if (data[1] == 2)
				{
					reasons[0] = DlmsException.DlmsExceptionReason.STATE_ERROR_SERVICE_UNKNOWN;
				}

				if (data[2] == 1)
				{
					reasons[1] = DlmsException.DlmsExceptionReason.SERVICE_ERROR_OPERATION_NOT_POSSIBLE;
				}
				else if (data[2] == 2)
				{
					reasons[1] = DlmsException.DlmsExceptionReason.SERVICE_ERROR_NOT_SUPPORTED;
				}
				else if (data[2] == 3)
				{
					reasons[1] = DlmsException.DlmsExceptionReason.SERVICE_ERROR_OTHER_REASON;
				}

				throw new DlmsException(reasons);
			}
			if (parameters.securityType != SecurityType.NONE)
			{
				if ((data[0] & 0xff) != cmdGlobalCipher)
				{
					throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_COMMAND_ID);
				}
				data = getPayload(data, 1);
				data = Security.reverseAuthenticatedEncryption(parameters, connection, data);
			}
			if ((data[0] & 0xFF) != cmdNoCipher)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_COMMAND_ID);
			}
			return helper.extensions.copyOfRange(data, 1, data.Length);
		}

		private static byte[] getSizeBytes(int size)
		{
			if (size < 0x80)
			{
				return new byte[]{(byte)size};
			}
			if (size <= 65535)
			{
				return new byte[]{unchecked((byte)0x81), (byte)((int)((uint)size >> 8)), (byte)size};
			}
            throw new InvalidDataException();
		}

		private static byte[] getPayload(byte[] data, int offset)
		{
			int nBytes = (data[offset] & 0xff) - 0x80;
			int size = data[offset] & 0xff;
			int skip = 0;
			if (nBytes > 0)
			{
				size = 0;
				skip = 1;
			}

			if (nBytes > (data.Length - offset))
			{
				throw new System.ArgumentException();
			}

			for (int i = 0; i < nBytes; ++i)
			{
				size <<= 8;
				size |= (data[offset + i + 1] & 0xff);
			}

			if (nBytes < 0)
			{
				nBytes = 1;
			}

			if (size != data.Length - nBytes - offset - skip)
			{
				throw new DlmsException(DlmsException.DlmsExceptionReason.RECEIVED_INVALID_COMMAND_ID);
			}

			return helper.extensions.copyOfRange(data, offset + nBytes + skip, data.Length);
		}

	}

}