

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

	public class LnDescriptor
	{

		private readonly byte[] classId;
		private readonly short index;
		private readonly byte[] obis;
		private byte[] requestData = new byte[0];
		private byte[] responseData = new byte[0];

		/// <summary>
		/// Creates a descriptor for a DLMS object </summary>
		/// <param name="classId"> the object class_id </param>
		/// <param name="index"> the index of the attribute/method to be accessed </param>
		/// <param name="obis"> the obis of the object </param>
		public LnDescriptor(int classId, Obis obis, int index) : this(classId, obis.getValue(), index, new byte[0])
		{
		}

		/// <summary>
		/// Creates a descriptor for a DLMS object </summary>
		/// <param name="classId"> the object class_id </param>
		/// <param name="index"> the index of the attribute/method to be accessed </param>
		/// <param name="obis"> the obis of the object </param>
		/// <param name="requestData"> the data to be used in the request </param>
		public LnDescriptor(int classId, Obis obis, int index, byte[] requestData) : this(classId, obis.getValue(), index, requestData)
		{
		}

		/// <summary>
		/// Creates a descriptor for a DLMS object </summary>
		/// <param name="classId"> the object class_id </param>
		/// <param name="index"> the index of the attribute/method to be accessed </param>
		/// <param name="obis"> the obis of the object </param>
		/// <param name="requestData"> the data to be used in the request </param>
		public LnDescriptor(int classId, byte[] obis, int index, byte[] requestData)
		{
			if (obis.Length != 6)
			{
				throw new System.ArgumentException();
			}
			this.obis = obis;
            //this.classId = ByteBuffer.allocate(2).putShort((short)classId).array();
            this.classId = (new byte[] { ((byte)(classId >> 8)), ((byte)(classId)) });
			this.index = (short)index;
			setRequestData(requestData);
		}

		/// <summary>
		/// Retrieves the classId of the descriptor </summary>
		/// <returns> byte array representing the classId </returns>
		public  byte[] getClassId()
		{
            return classId;
		}

		/// <summary>
		/// Retrieves the index of the descriptor </summary>
		/// <returns> index value </returns>
		public  int getIndex()
		{
            return index;
		}

		/// <summary>
		/// Retrieves the OBIS of the descriptor </summary>
		/// <returns> byte array representing the OBIS </returns>
		public  byte[] getObis()
		{
            return obis;
		}

        /// <summary>
        /// Retrieves the request data of the descriptor </summary>
        /// <returns> byte array representing the request data </returns>
        public  byte[] getRequestData()
        {
            return requestData;
        }

        /// <summary>
        /// Sets the data to be used in next operations </summary> 
        /// <return> data byte array of the data to be used </return>
        public  void setRequestData(byte[] data)
        {
            if (data == null)
            {
                this.requestData = new byte[0];
            }
            else
            {
                this.requestData = data;
            }
        }

        /// <summary>
        /// Retrieves the data received after an successful operation was performed </summary>
        /// <returns> byte array returned from the last operation successfully performed </returns>
        public  byte[] getResponseData()
        {
            return responseData;
        }

        /// <summary>
        /// Sets the response data of the descriptor </summary> 
        /// <return> byte array representing the response data </return>
        public  void setResponseData(byte[] data)
        {
            if (data == null)
            {
                this.responseData = new byte[0];
            }
            else
            {
                this.responseData = data;
            }
        }

	}

}