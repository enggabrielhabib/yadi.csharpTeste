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

namespace yadi.dlms
{
	using DlmsExceptionReason = yadi.dlms.DlmsException.DlmsExceptionReason;

	public sealed class DlmsType
	{
		public static readonly DlmsType ARRAY = new DlmsType("ARRAY", InnerEnum.ARRAY, 1,0);
		public static readonly DlmsType STRUCTURE = new DlmsType("STRUCTURE", InnerEnum.STRUCTURE, 2,0);
		public static readonly DlmsType BCD = new DlmsType("BCD", InnerEnum.BCD, 13,0);
		public static readonly DlmsType BITSTRING = new DlmsType("BITSTRING", InnerEnum.BITSTRING, 4,0);
		public static readonly DlmsType BOOLEAN = new DlmsType("BOOLEAN", InnerEnum.BOOLEAN, 3,1);
		public static readonly DlmsType DATE = new DlmsType("DATE", InnerEnum.DATE, 26,5);
		public static readonly DlmsType DATE_TIME = new DlmsType("DATE_TIME", InnerEnum.DATE_TIME, 25,12);
		public static readonly DlmsType ENUM = new DlmsType("ENUM", InnerEnum.ENUM, 22,1);
		public static readonly DlmsType FLOAT32 = new DlmsType("FLOAT32", InnerEnum.FLOAT32, 23,4);
		public static readonly DlmsType FLOAT64 = new DlmsType("FLOAT64", InnerEnum.FLOAT64, 24,8);
		public static readonly DlmsType INT16 = new DlmsType("INT16", InnerEnum.INT16, 16,2);
		public static readonly DlmsType INT32 = new DlmsType("INT32", InnerEnum.INT32, 5,4);
		public static readonly DlmsType INT64 = new DlmsType("INT64", InnerEnum.INT64, 20,8);
		public static readonly DlmsType INT8 = new DlmsType("INT8", InnerEnum.INT8, 15,1);
		public static readonly DlmsType OCTET_STRING = new DlmsType("OCTET_STRING", InnerEnum.OCTET_STRING, 9,0);
		public static readonly DlmsType STRING = new DlmsType("STRING", InnerEnum.STRING, 10,0);
		public static readonly DlmsType UTF8_STRING = new DlmsType("UTF8_STRING", InnerEnum.UTF8_STRING, 12,0);
		public static readonly DlmsType TIME = new DlmsType("TIME", InnerEnum.TIME, 27,4);
		public static readonly DlmsType UINT8 = new DlmsType("UINT8", InnerEnum.UINT8, 17,1);
		public static readonly DlmsType UINT16 = new DlmsType("UINT16", InnerEnum.UINT16, 18,2);
		public static readonly DlmsType UINT32 = new DlmsType("UINT32", InnerEnum.UINT32, 6,4);
		public static readonly DlmsType UINT64 = new DlmsType("UINT64", InnerEnum.UINT64, 21,8);

		private static readonly IList<DlmsType> valueList = new List<DlmsType>();

		static DlmsType()
		{
			valueList.Add(ARRAY);
			valueList.Add(STRUCTURE);
			valueList.Add(BCD);
			valueList.Add(BITSTRING);
			valueList.Add(BOOLEAN);
			valueList.Add(DATE);
			valueList.Add(DATE_TIME);
			valueList.Add(ENUM);
			valueList.Add(FLOAT32);
			valueList.Add(FLOAT64);
			valueList.Add(INT16);
			valueList.Add(INT32);
			valueList.Add(INT64);
			valueList.Add(INT8);
			valueList.Add(OCTET_STRING);
			valueList.Add(STRING);
			valueList.Add(UTF8_STRING);
			valueList.Add(TIME);
			valueList.Add(UINT8);
			valueList.Add(UINT16);
			valueList.Add(UINT32);
			valueList.Add(UINT64);
		}

		public enum InnerEnum
		{
			ARRAY,
			STRUCTURE,
			BCD,
			BITSTRING,
			BOOLEAN,
			DATE,
			DATE_TIME,
			ENUM,
			FLOAT32,
			FLOAT64,
			INT16,
			INT32,
			INT64,
			INT8,
			OCTET_STRING,
			STRING,
			UTF8_STRING,
			TIME,
			UINT8,
			UINT16,
			UINT32,
			UINT64
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		public readonly byte tag;
		public readonly int size;

		public DlmsType(string name, InnerEnum innerEnum, int tag, int size)
		{
			this.tag = (byte)tag;
			this.size = size;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		internal static DlmsType fromTag(byte tag)
		{
			foreach (DlmsType type in values())
			{
				if (type.tag == tag)
				{
					return type;
				}
			}
			throw new DlmsException(DlmsExceptionReason.NO_SUCH_TYPE, "Tag: " + tag);
		}

		public static IList<DlmsType> values()
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

		public static DlmsType valueOf(string name)
		{
			foreach (DlmsType enumInstance in DlmsType.valueList)
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