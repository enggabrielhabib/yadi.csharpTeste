using System.Collections.Generic;

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
	internal class Constants
	{

		internal const int DLMS_VERSION = 6;
		internal const int INVOKE_ID = 1;
		internal static readonly int PRIORITY_HIGH = 1 << 7;
		internal const int PRIORITY_NORMAL = 0;
		internal const int SERVICE_CLASS_UNCONFIRMED = 0;
		internal static readonly int SERVICE_CLASS_CONFIRMED = 1 << 6;

		internal class Ber
		{
			internal const int CLASS_UNIVERSAL = 0x00;
			internal const int CLASS_APPLICATION = 0x40;
			internal const int CLASS_CONTEXT = 0x80;
			internal const int CLASS_PRIVATE = 0xC0;
			internal const int PRIMITIVE = 0x00;
			internal const int CONSTRUCTED = 0x20;
			internal const int OBJECT_IDENTIFIER = 0x06;
			internal const int OCTET_STRING = 0x04;
			internal const int BIT_STRING = 0x03;
			internal const int INTEGER_8 = 1;
			internal const int INTEGER = 2;
			internal const int CONTEXT_CONSTRUCTED = CLASS_CONTEXT | CONSTRUCTED;
		}

		internal class CiPdu
		{
			internal const int PING_REQUEST_PDU = 25;
			internal const int PING_RESPONSE_PDU = 26;
			internal const int REGISTER_PDU = 28;
			internal const int DISCOVER_PDU = 29;
			internal const int DISCOVER_REPORT_PDU = 30;
			internal const int REPEATER_CALL_PDU = 31;
			internal const int CLEAR_ALARM_PDU = 57;
		}

		internal class DataType
		{
			internal const int OCTET_STRING = 9;
		}

		internal class xDlmsApdu
		{

			internal class NoCiphering
			{
				internal const int INITIATE_REQUEST = 1;
				internal const byte INITIATE_RESPONSE = 8;
				internal const int GET_REQUEST = 192;
				internal const int SET_REQUEST = 193;
				internal const int EVENT_NOTIFICATION_REQUEST = 194;
				internal const int ACTION_REQUEST = 195;
				internal const int GET_RESPONSE = 196;
				internal const int SET_RESPONSE = 197;
				internal const int ACTION_RESPONSE = 199;
			}

			internal class GlobalCiphering
			{
				internal const int INITIATE_REQUEST = 33;
				internal const byte INITIATE_RESPONSE = 40;
				internal const int GET_REQUEST = 200;
				internal const int SET_REQUEST = 201;
				internal const int EVENT_NOTIFICATION_REQUEST = 202;
				internal const int ACTION_REQUEST = 203;
				internal const int GET_RESPONSE = 204;
				internal const int SET_RESPONSE = 205;
				internal const int ACTION_RESPONSE = 207;
			}

			internal class DedicatedCiphering
			{
				internal const int GET_REQUEST = 208;
				internal const int SET_REQUEST = 209;
				internal const int EVENT_NOTIFICATION_REQUEST = 210;
				internal const int ACTION_REQUEST = 211;
				internal const int GET_RESPONSE = 212;
				internal const int SET_RESPONSE = 213;
				internal const int ACTION_RESPONSE = 215;
			}

			internal class Exception
			{
				internal const int ExceptionResponse = 216;
			}
		}

		internal class AarqApdu
		{
			internal const int PROTOCOL_VERSION = 0;
			internal const int APPLICATION_CONTEXT_NAME = 1;
			internal const int CALLED_AP_TITLE = 2;
			internal const int CALLED_AE_QUALIFIER = 3;
			internal const int CALLED_AP_INVOCATION_ID = 4;
			internal const int CALLED_AE_INVOCATION_ID = 5;
			internal const int CALLING_AP_TITLE = 6;
			internal const int CALLING_AE_QUALIFIER = 7;
			internal const int CALLING_AP_INVOCATION_ID = 8;
			internal const int CALLING_AE_INVOCATION_ID = 9;
			internal const int SENDER_ACSE_REQUIREMENTS = 10;
			internal const int MECHANISM_NAME = 11;
			internal const int CALLING_AUTHENTICATION_VALUE = 12;
			internal const int IMPLEMENTATION_INFORMATION = 29;
			internal const int USER_INFORMATION = 30;
		}

		internal class AareApdu
		{
			internal const int APPLICATION_1 = 97;
			internal const int PROTOCOL_VERSION = 0;
			internal const int APPLICATION_CONTEXT_NAME = 1;
			internal const int RESULT = 2;
			internal const int RESULT_SOURCE_DIAGNOSTIC = 3;
			internal const int RESPONDING_AP_TITLE = 4;
			internal const int RESPONDING_AE_QUALIFIER = 5;
			internal const int RESPONDING_AP_INVOCATION_ID = 6;
			internal const int RESPONDING_AE_INVOCATION_ID = 7;
			internal const int RESPONDER_ACSE_REQUIREMENTS = 8;
			internal const int MECHANISM_NAME = 9;
			internal const int RESPONDING_AUTHENTICATION_VALUE = 10;
			internal const int IMPLEMENTATION_INFORMATION = 29;
			internal const int USER_INFORMATION = 30;
		}

		internal class ConformanceBlock
		{
			internal const int TAG = 95; //TODO where?
			internal static readonly int READ = 1 << 3;
			internal static readonly int WRITE = 1 << 4;
			internal static readonly int UNCONFIRMED_WRITE = 1 << 5;
			internal static readonly int ATTRIBUTE_0_SUPPORTED_WITH_SET = 1 << 8;
			internal static readonly int PRIORITY_MGMT_SUPPORTED = 1 << 9;
			internal static readonly int ATTRIBUTE_0_SUPPORTED_WITH_GET = 1 << 10;
			internal static readonly int BLOCK_TRANSFER_WITH_GET_OR_READ = 1 << 11;
			internal static readonly int BLOCK_TRANSFER_WITH_SET_OR_WRITE = 1 << 12;
			internal static readonly int BLOCK_TRANSFER_WITH_ACTION = 1 << 13;
			internal static readonly int MULTIPLE_REFERENCES = 1 << 14;
			internal static readonly int INFORMATION_REPORT = 1 << 15;
			internal static readonly int PARAMETERIZED_ACCESS = 1 << 18;
			internal static readonly int GET = 1 << 19;
			internal static readonly int SET = 1 << 20;
			internal static readonly int SELECTIVE_ACCESS = 1 << 21;
			internal static readonly int EVENT_NOTIFICATION = 1 << 22;
			internal static readonly int ACTION = 1 << 23;
		}

		internal class ApplicationContextName
		{
			internal static readonly byte[] LOGICAL_NAME_NO_CIPHERING = new byte[] {0x60, ((byte) 0x85), 0x74, 0x05, 0x08, 0x01, 0x01};
			internal static readonly byte[] SHORT_NAME_NO_CIPHERING = new byte[] {0x60, ((byte) 0x85), 0x74, 0x05, 0x08, 0x01, 0x02};
			internal static readonly byte[] LOGICAL_NAME_WITH_CIPHERING = new byte[] {0x60, ((byte) 0x85), 0x74, 0x05, 0x08, 0x01, 0x03};
			internal static readonly byte[] SHORT_NAME_WITH_CIPHERING = new byte[] {0x60, ((byte) 0x85), 0x74, 0x05, 0x08, 0x01, 0x04};
		}

		internal class GetRequest
		{
			internal const int NORMAL = 1;
			internal const int NEXT = 2;
			internal const int WITH_LIST = 3;
		}

		internal class GetResponse
		{
			internal const int NORMAL = 1;
			internal const int DATA_BLOCK = 2;
			internal const int WITH_LIST = 3;
		}

		internal class SetResponse
		{
			internal const int NORMAL = 1;
			internal const int DATA_BLOCK = 2;
			internal const int LAST_DATA_BLOCK = 3;
			internal const int LAST_DATA_BLOCK_WITH_LIST = 4;
			internal const int WITH_LIST = 5;
		}

		internal class AssociateSourceDiagnostic
		{
			internal const int NULL = 0;
			internal const int NO_REASON = 1;
			internal const int CONTEXT_NAME_NOT_SUPPORTED = 2;
			internal const int AUTHENTICATION_MECHANISM_NOT_RECOGNISED = 11;
			internal const int AUTHENTICATION_MECHANISM_REQUIRED = 12;
			internal const int AUTHENTICATION_FAILURE = 13;
			internal const int AUTHENTICATION_REQUIRED = 14;
		}

		internal sealed class AccessResult
		{
			public static readonly AccessResult ACCESS_RESULT_HARDWARE_FAULT = new AccessResult("ACCESS_RESULT_HARDWARE_FAULT", InnerEnum.ACCESS_RESULT_HARDWARE_FAULT, 1);
			public static readonly AccessResult ACCESS_RESULT_TEMPORARY_FAILURE = new AccessResult("ACCESS_RESULT_TEMPORARY_FAILURE", InnerEnum.ACCESS_RESULT_TEMPORARY_FAILURE, 2);
			public static readonly AccessResult ACCESS_RESULT_READ_WRITE_DENIED = new AccessResult("ACCESS_RESULT_READ_WRITE_DENIED", InnerEnum.ACCESS_RESULT_READ_WRITE_DENIED, 3);
			public static readonly AccessResult ACCESS_RESULT_OBJECT_UNDEFINED = new AccessResult("ACCESS_RESULT_OBJECT_UNDEFINED", InnerEnum.ACCESS_RESULT_OBJECT_UNDEFINED, 4);
			public static readonly AccessResult ACCESS_RESULT_OBJECT_CLASS_INCONSISTENT = new AccessResult("ACCESS_RESULT_OBJECT_CLASS_INCONSISTENT", InnerEnum.ACCESS_RESULT_OBJECT_CLASS_INCONSISTENT, 9);
			public static readonly AccessResult ACCESS_RESULT_OBJECT_UNAVAILABLE = new AccessResult("ACCESS_RESULT_OBJECT_UNAVAILABLE", InnerEnum.ACCESS_RESULT_OBJECT_UNAVAILABLE, 11);
			public static readonly AccessResult ACCESS_RESULT_TYPE_UNMATCHED = new AccessResult("ACCESS_RESULT_TYPE_UNMATCHED", InnerEnum.ACCESS_RESULT_TYPE_UNMATCHED, 12);
			public static readonly AccessResult ACCESS_RESULT_SCOPE_OF_ACCESS_VIOLATED = new AccessResult("ACCESS_RESULT_SCOPE_OF_ACCESS_VIOLATED", InnerEnum.ACCESS_RESULT_SCOPE_OF_ACCESS_VIOLATED, 13);
			public static readonly AccessResult ACCESS_RESULT_DATA_BLOCK_UNAVAILABLE = new AccessResult("ACCESS_RESULT_DATA_BLOCK_UNAVAILABLE", InnerEnum.ACCESS_RESULT_DATA_BLOCK_UNAVAILABLE, 14);
			public static readonly AccessResult ACCESS_RESULT_LONG_GET_ABORTED = new AccessResult("ACCESS_RESULT_LONG_GET_ABORTED", InnerEnum.ACCESS_RESULT_LONG_GET_ABORTED, 15);
			public static readonly AccessResult ACCESS_RESULT_NO_LONG_GET_IN_PROGRESS = new AccessResult("ACCESS_RESULT_NO_LONG_GET_IN_PROGRESS", InnerEnum.ACCESS_RESULT_NO_LONG_GET_IN_PROGRESS, 16);
			public static readonly AccessResult ACCESS_RESULT_LONG_SET_ABORTED = new AccessResult("ACCESS_RESULT_LONG_SET_ABORTED", InnerEnum.ACCESS_RESULT_LONG_SET_ABORTED, 17);
			public static readonly AccessResult ACCESS_RESULT_NO_LONG_SET_IN_PROGRESS = new AccessResult("ACCESS_RESULT_NO_LONG_SET_IN_PROGRESS", InnerEnum.ACCESS_RESULT_NO_LONG_SET_IN_PROGRESS, 18);
			public static readonly AccessResult ACCESS_RESULT_DATA_BLOCK_NUMBER_INVALID = new AccessResult("ACCESS_RESULT_DATA_BLOCK_NUMBER_INVALID", InnerEnum.ACCESS_RESULT_DATA_BLOCK_NUMBER_INVALID, 19);
			public static readonly AccessResult ACCESS_RESULT_OTHER_REASON = new AccessResult("ACCESS_RESULT_OTHER_REASON", InnerEnum.ACCESS_RESULT_OTHER_REASON, 250);

			private static readonly IList<AccessResult> valueList = new List<AccessResult>();

			static AccessResult()
			{
				valueList.Add(ACCESS_RESULT_HARDWARE_FAULT);
				valueList.Add(ACCESS_RESULT_TEMPORARY_FAILURE);
				valueList.Add(ACCESS_RESULT_READ_WRITE_DENIED);
				valueList.Add(ACCESS_RESULT_OBJECT_UNDEFINED);
				valueList.Add(ACCESS_RESULT_OBJECT_CLASS_INCONSISTENT);
				valueList.Add(ACCESS_RESULT_OBJECT_UNAVAILABLE);
				valueList.Add(ACCESS_RESULT_TYPE_UNMATCHED);
				valueList.Add(ACCESS_RESULT_SCOPE_OF_ACCESS_VIOLATED);
				valueList.Add(ACCESS_RESULT_DATA_BLOCK_UNAVAILABLE);
				valueList.Add(ACCESS_RESULT_LONG_GET_ABORTED);
				valueList.Add(ACCESS_RESULT_NO_LONG_GET_IN_PROGRESS);
				valueList.Add(ACCESS_RESULT_LONG_SET_ABORTED);
				valueList.Add(ACCESS_RESULT_NO_LONG_SET_IN_PROGRESS);
				valueList.Add(ACCESS_RESULT_DATA_BLOCK_NUMBER_INVALID);
				valueList.Add(ACCESS_RESULT_OTHER_REASON);
			}

			public enum InnerEnum
			{
				ACCESS_RESULT_HARDWARE_FAULT,
				ACCESS_RESULT_TEMPORARY_FAILURE,
				ACCESS_RESULT_READ_WRITE_DENIED,
				ACCESS_RESULT_OBJECT_UNDEFINED,
				ACCESS_RESULT_OBJECT_CLASS_INCONSISTENT,
				ACCESS_RESULT_OBJECT_UNAVAILABLE,
				ACCESS_RESULT_TYPE_UNMATCHED,
				ACCESS_RESULT_SCOPE_OF_ACCESS_VIOLATED,
				ACCESS_RESULT_DATA_BLOCK_UNAVAILABLE,
				ACCESS_RESULT_LONG_GET_ABORTED,
				ACCESS_RESULT_NO_LONG_GET_IN_PROGRESS,
				ACCESS_RESULT_LONG_SET_ABORTED,
				ACCESS_RESULT_NO_LONG_SET_IN_PROGRESS,
				ACCESS_RESULT_DATA_BLOCK_NUMBER_INVALID,
				ACCESS_RESULT_OTHER_REASON
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;

			internal int val;

			internal AccessResult(string name, InnerEnum innerEnum, int val)
			{
				this.val = val;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public static IList<AccessResult> values()
			{
				return valueList;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static AccessResult valueOf(string name)
			{
				foreach (AccessResult enumInstance in AccessResult.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

	}

}