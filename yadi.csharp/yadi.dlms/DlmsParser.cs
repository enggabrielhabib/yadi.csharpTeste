
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
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;


namespace yadi.dlms
{

	using DlmsExceptionReason = yadi.dlms.DlmsException.DlmsExceptionReason;
    using helper = yadi.csharp.Extensions;

    public class DlmsParser
	{

		/// <summary>
		/// Retrieves the DlmsType from an array of bytes </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> The DlmsType of the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static DlmsType getTypeFromRawBytes(byte[] data)
		{
			verify(data);
			return DlmsType.fromTag(data[0]);
		}

		/// <summary>
		/// Retrieves the String representation of the raw bytes.
		/// The string will include the type and the size of the element parsed. </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A String that represents the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static DlmsItem getDlmsItem(byte[] data)
		{
			verify(data);
			DlmsType type = DlmsType.fromTag(data[0]);
			DlmsItem item = new DlmsItem(type, getString(data));
			int numberOfItems = getNumberOfItems(type,data);
			for (int i = 0; i < numberOfItems; ++i)
			{
				parseItems(item, getNextData(data));
			}
			return item;
		}

        private static byte[] parseItems(DlmsItem parent, byte[] data)
		{
			if (data.Length == 0)
			{
				return data;
			}
			DlmsType type = DlmsType.fromTag(data[0]);
			DlmsItem item = new DlmsItem(type, getString(data));
			parent.addChildren(item);
			int numberOfItems = getNumberOfItems(type,data);
			for (int i = 0; i < numberOfItems; ++i)
			{
				parseItems(item, getNextData(data));
			}
            return data;
		}

        private static byte[] getNextData(byte[] data)
		{
            if (data == null || data.Length == 0)
			{
				return new byte[0];
			}
			DlmsType type = DlmsType.fromTag(data[0]);
            int offset;
			if (getNumberOfItems(type,data) == 0)
			{
				offset = type.size == 0 ? getSize(data) + getOffset(data) : type.size + 1;
				return helper.extensions.copyOfRange(data, offset, data.Length);
			}
			offset = getOffset(data);
			return helper.extensions.copyOfRange(data, offset, data.Length);
		}

		/// <summary>
		/// Retrieves the String representation of the element in the array of bytes </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A Strig that represents the element in the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static string getString(byte[] data)
		{
			verify(data);
			DlmsType type = DlmsType.fromTag(data[0]);
			return DlmsParser.getStringValue(type, getPayload(type, data));
		}

		/// <summary>
		/// Retrieves the String representation of the element in the array of bytes </summary>
		/// <param name="type"> the DlmsType of the byte array </param>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A Strig that represents the element in the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static string getString(DlmsType type, byte[] payload)
		{
			return DlmsParser.getStringValue(type, payload);
		}

		/// <summary>
		/// Retrieves the DateTime String representation of the element in the array of bytes </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A Strig that represents the date and time in the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static string getDateTimeString(byte[] data)
		{
			verify(data);
			DlmsType type = DlmsType.fromTag(data[0]);
			return DlmsParser.getDateTimeStringValue(getPayload(type, data));
		}

		/// <summary>
		/// Retrieves the Date String representation of the element in the array of bytes </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A Strig that represents the date in the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static string getDateString(byte[] data)
		{
			verify(data);
			DlmsType type = DlmsType.fromTag(data[0]);
			return DlmsParser.getDateStringValue(getPayload(type, data));
		}

		/// <summary>
		/// Retrieves the Time String representation of the element in the array of bytes </summary>
		/// <param name="data"> array of bytes containing data result from a dlms-get </param>
		/// <returns> A Strig that represents the time in the data </returns>
		/// <exception cref="DlmsException"> </exception>
		public static string getTimeString(byte[] data)
		{
			verify(data);
			DlmsType type = DlmsType.fromTag(data[0]);
			return DlmsParser.getTimeStringValue(getPayload(type, data));
		}

		private static int getNumberOfItems(DlmsType type, byte[] data)
		{
			if (type.Equals(DlmsType.ARRAY) || type.Equals(DlmsType.STRUCTURE))
			{
				return getSize(data);
			}
			return 0;
		}

		private static void verify(byte[] data)
		{
			if (data == null || data.Length < 2)
			{
				throw new DlmsException(DlmsExceptionReason.INVALID_DATA);
			}
		}

		public static byte[] getPayload(DlmsType type, byte[] data)
		{
			int offset = type.size == 0 ? getOffset(data) : 1;
			int size = type.size == 0 ? getSize(data) : type.size;
			return helper.extensions.copyOfRange(data, offset, offset + size);
		}

		public static byte[] pack(DlmsType type, byte[] data)
		{
			int size = type.size == 0 ? data.Length : type.size; //TODO size > 1 byte
			int offset = type.size == 0 ? 1 : 0;
			byte[] retval = new byte[data.Length + 1 + offset];
			retval[0] = type.tag;
			if (offset != 0)
			{
				retval[1] = (byte)data.Length;
			}
			Array.Copy(data, 0, retval, offset + 1, data.Length);
			return retval;
		}
        public static int getInteger(byte[] data)
        {
            int val = 0;
            for (int i = 1; i < data.Length; ++i)
            {
                val <<= 8;
                val += (data[i] & 0xFF);
            }

            return val;
        }

        public static bool getBoolean(byte[] responseData)
        {
            if (responseData.Length != 2)
            {
                throw new DlmsException(DlmsExceptionReason.INVALID_DATA);
            }
            if (responseData[0] != DlmsType.BOOLEAN.tag)
            {
                throw new DlmsException(DlmsExceptionReason.INVALID_DATA);
            }
            return 0 != responseData[1];
        }


        private static string getStringValue(DlmsType type, byte[] payload)
		{
			switch (type.innerEnumValue)
			{
			case yadi.dlms.DlmsType.InnerEnum.ARRAY:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.BCD:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.BITSTRING:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.BOOLEAN:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.DATE:
				return getDateStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.DATE_TIME:
				return getDateTimeStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.ENUM:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.FLOAT32:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToSingle(payload, 0), new System.Globalization.CultureInfo("en-US"));
			case yadi.dlms.DlmsType.InnerEnum.FLOAT64:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToDouble(payload, 0), new System.Globalization.CultureInfo("en-US"));
			case yadi.dlms.DlmsType.InnerEnum.INT16:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToInt16(payload, 0));
			case yadi.dlms.DlmsType.InnerEnum.INT32:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToInt32(payload, 0));
			case yadi.dlms.DlmsType.InnerEnum.INT64:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToInt64(payload, 0));
			case yadi.dlms.DlmsType.InnerEnum.INT8:
                return Convert.ToString((sbyte)payload[0]);
            case yadi.dlms.DlmsType.InnerEnum.OCTET_STRING:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.STRING:
                return new String(Encoding.ASCII.GetChars(payload));
			case yadi.dlms.DlmsType.InnerEnum.STRUCTURE:
				return bytesToHex(payload);
			case yadi.dlms.DlmsType.InnerEnum.TIME:
				return getTimeStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.UINT16:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToUInt16(payload, 0));
			case yadi.dlms.DlmsType.InnerEnum.UINT32:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToUInt32(payload, 0));
            case yadi.dlms.DlmsType.InnerEnum.UINT64:
                Array.Reverse(payload);
                return Convert.ToString(BitConverter.ToUInt64(payload, 0));
			case yadi.dlms.DlmsType.InnerEnum.UINT8:
                return Convert.ToString(payload[0]);
            case yadi.dlms.DlmsType.InnerEnum.UTF8_STRING:
                return new String(Encoding.UTF8.GetChars(payload));
            }
			throw new DlmsException(DlmsExceptionReason.NO_SUCH_TYPE);
		}

		public static byte[] getByteValue(DlmsType type, string value)
		{
			switch (type.innerEnumValue)
			{
			case yadi.dlms.DlmsType.InnerEnum.ARRAY:
			case yadi.dlms.DlmsType.InnerEnum.BCD:
			case yadi.dlms.DlmsType.InnerEnum.BITSTRING:
			case yadi.dlms.DlmsType.InnerEnum.BOOLEAN:
			case yadi.dlms.DlmsType.InnerEnum.ENUM:
			case yadi.dlms.DlmsType.InnerEnum.OCTET_STRING:
			case yadi.dlms.DlmsType.InnerEnum.STRUCTURE:
				return pack(type, hexStringToBytes(value));
			case yadi.dlms.DlmsType.InnerEnum.DATE:
				//TODO return getDateStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.DATE_TIME:
				//TODO return getDateTimeStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.FLOAT32:
				//TODO return pack(type, );
			case yadi.dlms.DlmsType.InnerEnum.FLOAT64:
				//TODO return Double.toString(ByteBuffer.wrap(payload).getDouble());
			case yadi.dlms.DlmsType.InnerEnum.INT16:
			case yadi.dlms.DlmsType.InnerEnum.INT32:
			case yadi.dlms.DlmsType.InnerEnum.INT64:
			case yadi.dlms.DlmsType.InnerEnum.INT8:
				return pack(type, getIntegerBytes(type,value));
			case yadi.dlms.DlmsType.InnerEnum.STRING:
                return pack(type, Encoding.ASCII.GetBytes(value));
			case yadi.dlms.DlmsType.InnerEnum.TIME:
				//TODO return getTimeStringValue(payload);
			case yadi.dlms.DlmsType.InnerEnum.UINT16:
			case yadi.dlms.DlmsType.InnerEnum.UINT32:
			case yadi.dlms.DlmsType.InnerEnum.UINT64:
			case yadi.dlms.DlmsType.InnerEnum.UINT8:
				return pack(type, getIntegerBytes(type,value));
			case yadi.dlms.DlmsType.InnerEnum.UTF8_STRING:
                return pack(type, Encoding.UTF8.GetBytes(value));
            }
			throw new DlmsException(DlmsExceptionReason.NO_SUCH_TYPE);
		}

		private static int getOffset(byte[] data)
		{
			if ((data[1] & 0xFF) <= 0x80)
			{
				return 2;
			}
			return (data[1] & 0x0F) + 2;
		}

		private static int getSize(byte[] data)
		{
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            if ((data[1] & 0xFF) <= 0x80)
			{
				return data[1] & 0xFF;
			}
			if (data[1] == (byte)0x81)
			{
				return data[2] & 0xFF;
			}
			if (data[1] == (byte)0x82)
			{
                return BitConverter.ToInt16(new byte[] { data[3], data[2] }, 0);
            }
			if (data[1] == (byte)0x83)
			{
                return BitConverter.ToInt32(new byte[] { data[4], data[3], data[2], (byte)0x00 }, 0);
            }
			if (data[1] == (byte)0x84)
			{
                return BitConverter.ToInt32(new byte[] { data[5], data[4], data[3], data[2] }, 0);
            }
			throw new System.ArgumentException();
		}

		private static string bytesToHex(byte[] data)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in data)
			{
				sb.Append(string.Format("{0:X2}", b));
			}
			return sb.ToString();
		}

		private static byte[] hexStringToBytes(string s)
		{
			s = s.Replace(" ", "");
			int len = s.Length;
			if ((len & 0x01) != 0)
			{
				s = "0" + s;
				++len;
			}
			byte[] data = new byte[len / 2];
			for (int i = 0; i < len; i += 2)
			{
                data[i / 2] = (byte)((Convert.ToInt32(s[i].ToString(), 16) << 4) + Convert.ToInt32(s[i + 1].ToString(), 16));
            }
			return data;
		}

		private static string getTimeStringValue(byte[] bytes)
		{
			if (bytes.Length < 4)
			{
				throw new System.ArgumentException();
			}
			string hour = getDateValue(bytes[0], "HH");
			string min = getDateValue(bytes[1], "mm");
			string sec = getDateValue(bytes[2], "SS");
			return hour + ":" + min + ":" + sec;
		}

		private static string getDateStringValue(byte[] bytes)
		{
			if (bytes.Length < 5)
			{
				throw new System.ArgumentException();
			}
			string year = getYear(bytes);
			string month = getDateValue(bytes[2], "MM");
			string day = getDateValue(bytes[3], "DD");
			return year + "/" + month + "/" + day;
		}

		private static string getDateTimeStringValue(byte[] bytes)
		{
			if (bytes.Length < 8)
			{
				throw new System.ArgumentException();
			}
			string year = getYear(bytes);
			string month = getDateValue(bytes[2], "MM");
			string day = getDateValue(bytes[3], "DD");
			string hour = getDateValue(bytes[5], "HH");
			string min = getDateValue(bytes[6], "mm");
			string sec = getDateValue(bytes[7], "SS");
			return year + "/" + month + "/" + day + " " + hour + ":" + min + ":" + sec;
		}

		private static string getYear(byte[] bytes)
		{
			if (bytes[0] == (byte)0xFF && bytes[1] == (byte)0xFF)
			{
				return "YY";
			}
            return string.Format("{0:D4}", BitConverter.ToInt16(new byte[] { bytes[1], bytes[0] }, 0)); //BIG ENDIAN
        }

		private static string getDateValue(byte val, string replacement)
		{
			if (val == unchecked((byte)0xFF))
			{
				return replacement;
			}
			return string.Format("{0:D2}", val & 0xFF);
		}

		private static byte[] getIntegerBytes(DlmsType type, string str)
		{
			byte[] bytes = new byte[type.size];
			bytes[0] = type.tag;
			int val = 0;
			try
			{
				val = int.Parse(str);
			}
			catch (Exception)
			{
				val = 0;
			}
			for (int i = 0; i < type.size; ++i)
			{
				bytes[type.size - i] = (byte)(val & 0x00FF);
				val = (int)((uint)val >> 8);
			}
			return bytes;
		}

	}

}