
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


namespace yadi.dlms
{
	public class Obis
	{

		private readonly byte[] value;

		/// <summary>
		/// Creates a OBIS using the byte value for each group </summary>
		/// <param name="A"> byte value of group A </param>
		/// <param name="B"> byte value of group B </param>
		/// <param name="C"> byte value of group C </param>
		/// <param name="D"> byte value of group D </param>
		/// <param name="E"> byte value of group E </param>
		/// <param name="F"> byte value of group F </param>
		public Obis(int A, int B, int C, int D, int E, int F)
		{
			value = new byte[] {(byte)A, (byte)B, (byte)C, (byte)D, (byte)E, (byte)F};
		}

		/// <summary>
		/// Creates a OBIS using a string representation </summary>
		/// <param name="obis"> a String representing the OBIS in the form A.B.C.D.E.F </param>
		public Obis(string obis)
		{
            string[] data = obis.Split('.');
            if (data.Length != 6)
			{
				Console.WriteLine(data.Length);
				throw new System.ArgumentException("OBIS must have 6 bytes");
			}
			value = new byte[] {(byte)int.Parse(data[0]), (byte)int.Parse(data[1]), (byte)int.Parse(data[2]), (byte)int.Parse(data[3]), (byte)int.Parse(data[4]), (byte)int.Parse(data[5])};
		}

		/// <summary>
		/// Retrieves the byte array representing the value of each group of the OBIS </summary>
		/// <returns> byte array of the OBIS value </returns>
		public  byte[] getValue()
        {
			return value;
		}

	}

}
