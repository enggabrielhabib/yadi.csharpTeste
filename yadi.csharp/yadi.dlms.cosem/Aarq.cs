
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
using System.IO;
namespace yadi.dlms.cosem
{

	using DlmsExceptionReason = yadi.dlms.DlmsException.DlmsExceptionReason;
	using AuthenticationType = yadi.dlms.cosem.CosemParameters.AuthenticationType;
	using ReferenceType = yadi.dlms.cosem.CosemParameters.ReferenceType;
	using SecurityType = yadi.dlms.cosem.CosemParameters.SecurityType;

	internal class Aarq
	{

		internal static byte[] request(CosemParameters parameters, CosemConnection connection)
		{
			const int BASE = Constants.Ber.CLASS_CONTEXT | Constants.Ber.CONSTRUCTED;
			byte[] applicationContextName = generateApplicationContextName(parameters.referenceType, parameters.securityType);
			connection.proposedContextName = applicationContextName;
			System.IO.MemoryStream data = new System.IO.MemoryStream();

			//TODO
			if (parameters.authenticationType == AuthenticationType.PUBLIC)
			{
                //data.WriteByte(new byte[] {(byte)0x80, 0x02, 0x07, (byte)0x80});
                byte[] aux = new byte[] { (byte)0x80, 0x02, 0x07, (byte)0x80 };
                data.Write(aux, 0, aux.Length);
			}

			data.WriteByte(BASE | Constants.AarqApdu.APPLICATION_CONTEXT_NAME);
			data.WriteByte((byte)(applicationContextName.Length + 2));
			data.WriteByte(Constants.Ber.OBJECT_IDENTIFIER);
			data.WriteByte((byte)(applicationContextName.Length));
			data.Write(applicationContextName, 0, applicationContextName.Length); //big or little endian?

			if (parameters.securityType != SecurityType.NONE || parameters.authenticationType == AuthenticationType.HLS_GMAC)
			{
				data.WriteByte(BASE | Constants.Ber.OBJECT_IDENTIFIER);
				data.WriteByte((byte)(parameters.systemTitle.Length + 2));
				data.WriteByte(Constants.Ber.OCTET_STRING);
				data.WriteByte((byte)(parameters.systemTitle.Length));
                //data.WriteByte((byte)(parameters.systemTitle));
                byte[] aux = parameters.systemTitle;
                data.Write(aux, 0, aux.Length);
			}
			if (parameters.authenticationType != AuthenticationType.PUBLIC)
			{
				data.WriteByte(Constants.Ber.CLASS_CONTEXT | Constants.AarqApdu.SENDER_ACSE_REQUIREMENTS);
				data.WriteByte(2);
				data.WriteByte(Constants.Ber.BIT_STRING | Constants.Ber.OCTET_STRING);
				data.WriteByte(0x80);
				data.WriteByte(Constants.Ber.CLASS_CONTEXT | Constants.AarqApdu.MECHANISM_NAME);
				data.WriteByte(7);
                //data.WriteByte(new byte[]{0x60, (byte)0x85, 0x74, 0x05, 0x08, 0x02, (byte)parameters.authenticationType.value});
                byte[] aux = new byte[] { 0x60, (byte)0x85, 0x74, 0x05, 0x08, 0x02, (byte)parameters.authenticationType.value };
                data.Write(aux, 0, aux.Length);

                data.WriteByte(BASE | Constants.AarqApdu.CALLING_AUTHENTICATION_VALUE);
				if (parameters.authenticationType == AuthenticationType.LLS)
				{
					data.WriteByte((byte)(parameters.llsHlsSecret.Length + 2));
					data.WriteByte(Constants.Ber.CLASS_CONTEXT);
					data.WriteByte((byte)(parameters.llsHlsSecret.Length));
                    //data.WriteByte(parameters.llsHlsSecret);
                    byte[] aux2 = parameters.llsHlsSecret;
                    data.Write(aux2, 0, aux2.Length);
                    
				}
				else if (parameters.authenticationType != AuthenticationType.PUBLIC)
				{
					connection.challengeClientToServer = Security.generateChallanger(parameters);
					data.WriteByte((byte)(connection.challengeClientToServer.Length + 2));
					data.WriteByte(Constants.Ber.CLASS_CONTEXT);
					data.WriteByte((byte)(connection.challengeClientToServer.Length));
					//data.WriteByte(connection.challengeClientToServer);
                    byte[] aux2 = connection.challengeClientToServer;
                    data.Write(aux2, 0, aux2.Length);
                }
			}
			data.WriteByte(Constants.Ber.CONTEXT_CONSTRUCTED | Constants.AarqApdu.USER_INFORMATION);
			byte[] initiateRequest = generateInitiateRequest(parameters);
			data.WriteByte((byte)(initiateRequest.Length + 2));
			data.WriteByte(Constants.Ber.OCTET_STRING);
			data.WriteByte((byte)(initiateRequest.Length));
            data.Write(initiateRequest, 0, initiateRequest.Length);
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			stream.WriteByte(Constants.Ber.CLASS_APPLICATION | Constants.Ber.CONSTRUCTED);
			stream.WriteByte((byte)(data.Length));
            stream.Write(data.ToArray(), 0, data.ToArray().Length);

			return stream.ToArray();
		}

		private static byte[] generateApplicationContextName(ReferenceType referenceType, SecurityType securityType)
		{
			if (referenceType == ReferenceType.LOGICAL_NAME)
			{
				if (securityType == SecurityType.NONE)
				{
					return Constants.ApplicationContextName.LOGICAL_NAME_NO_CIPHERING;
				}
				else
				{
					return Constants.ApplicationContextName.LOGICAL_NAME_WITH_CIPHERING;
				}
			}
			if (referenceType == ReferenceType.SHORT_NAME)
			{
				if (securityType == SecurityType.NONE)
				{
					return Constants.ApplicationContextName.SHORT_NAME_NO_CIPHERING;
				}
				else
				{
					return Constants.ApplicationContextName.SHORT_NAME_WITH_CIPHERING;
				}
			}
			throw new DlmsException(DlmsException.DlmsExceptionReason.INVALID_SETTING);
		}

		private static byte[] generateInitiateRequest(CosemParameters parameters)
		{
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			stream.WriteByte(Constants.xDlmsApdu.NoCiphering.INITIATE_REQUEST);
			stream.WriteByte(0); //Dedicated key
			stream.WriteByte(0); //Response-allowed
			stream.WriteByte(0); //Proposed quality of service
			stream.WriteByte(Constants.DLMS_VERSION); //Dlms version
			stream.WriteByte(Constants.ConformanceBlock.TAG); //Conformance block tag
			byte[] conformance = generateConformanceBlock(parameters);
			stream.WriteByte((byte)(conformance.Length));
			stream.Write(conformance, 0, conformance.Length); //Conformance block

            //stream.WriteByte(ByteBuffer.allocate(2).putShort(parameters.maxPduSize).array()); 
            stream.WriteByte((byte)(parameters.maxPduSize >> 8)); //Max pdu size
            stream.WriteByte((byte)(parameters.maxPduSize));

			System.IO.MemoryStream stream2 = new System.IO.MemoryStream();
			if (parameters.securityType != SecurityType.NONE)
			{
				stream2.WriteByte(Constants.xDlmsApdu.GlobalCiphering.INITIATE_REQUEST);
				byte[] data = Security.authenticatedEncryption(parameters, stream.ToArray());
				stream2.WriteByte((byte)(data.Length));
				stream2.Write(data, 0, data.Length);
			}
			else
			{
				//stream2.WriteByte(stream.ToArray());
                byte[] aux = stream.ToArray();
                stream2.Write(aux, 0, aux.Length);
			}
			return stream2.ToArray();
		}

		private static byte[] generateConformanceBlock(CosemParameters parameters)
		{
			int conformanceBlock = 0;
			if (parameters.referenceType == ReferenceType.SHORT_NAME)
			{
				conformanceBlock |= Constants.ConformanceBlock.READ;
				conformanceBlock |= Constants.ConformanceBlock.WRITE;
			}
			if (parameters.referenceType == ReferenceType.LOGICAL_NAME)
			{
				conformanceBlock |= Constants.ConformanceBlock.GET;
				conformanceBlock |= Constants.ConformanceBlock.SET;
			}
			conformanceBlock |= Constants.ConformanceBlock.ACTION;
			conformanceBlock |= Constants.ConformanceBlock.BLOCK_TRANSFER_WITH_ACTION;
			conformanceBlock |= Constants.ConformanceBlock.BLOCK_TRANSFER_WITH_GET_OR_READ;
			conformanceBlock |= Constants.ConformanceBlock.BLOCK_TRANSFER_WITH_SET_OR_WRITE;
			conformanceBlock |= Constants.ConformanceBlock.SELECTIVE_ACCESS;
            //return ByteBuffer.allocate(4).putInt(conformanceBlock).array();
            return (new byte[] { (byte)(conformanceBlock >> 24), (byte)(conformanceBlock >> 16), (byte)(conformanceBlock >> 8), (byte)(conformanceBlock) });
		}

	}

}