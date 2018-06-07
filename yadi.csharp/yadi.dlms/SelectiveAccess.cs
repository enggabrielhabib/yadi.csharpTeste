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

namespace yadi.dlms
{

    using DlmsExceptionReason = yadi.dlms.DlmsException.DlmsExceptionReason;

    public class SelectiveAccess
    {
        public static byte[] getEntryDescriptor(int lineFrom, int lineTo, int colFrom, int colTo)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                stream.WriteByte(DlmsType.STRUCTURE.tag);
                stream.WriteByte(4);
                stream.WriteByte(DlmsType.UINT32.tag);

                //stream.WriteByte(ByteBuffer.allocate(4).putInt(lineFrom).array());
                stream.WriteByte((byte)(lineFrom >> 24));
                stream.WriteByte((byte)(lineFrom >> 16));
                stream.WriteByte((byte)(lineFrom >> 8));
                stream.WriteByte((byte)lineFrom);

                stream.WriteByte(DlmsType.UINT32.tag);

                //stream.WriteByte(ByteBuffer.allocate(4).putInt(lineTo).array());
                stream.WriteByte((byte)(lineTo >> 24));
                stream.WriteByte((byte)(lineTo >> 16));
                stream.WriteByte((byte)(lineTo >> 8));
                stream.WriteByte((byte)lineTo);

                stream.WriteByte(DlmsType.UINT16.tag);

                //stream.WriteByte(ByteBuffer.allocate(2).putShort((short)colFrom).array());
                stream.WriteByte((byte)(colFrom >> 8));
                stream.WriteByte((byte)colFrom);

                stream.WriteByte(DlmsType.UINT16.tag);

                //stream.WriteByte(ByteBuffer.allocate(2).putShort((short)colTo).array());
                stream.WriteByte((byte)(colTo >> 8));
                stream.WriteByte((byte)colTo);

                return stream.ToArray();
            }
            catch (IOException)
            {
                throw new DlmsException(DlmsExceptionReason.INTERNAL_ERROR);
            }
        }
    }
}
