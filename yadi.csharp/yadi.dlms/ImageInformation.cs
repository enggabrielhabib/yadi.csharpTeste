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
    public class ImageInformation
    {
        private int blockSize;
        private readonly byte[] identifier;
        private readonly byte[] image;

        public ImageInformation(int blockSize, byte[] identifier, byte[] image)
        {
            this.blockSize = blockSize;
            this.identifier = identifier;
            this.image = image;
        }

        public ImageInformation(byte[] identifier, byte[] image) : this(0, identifier, image)
        {
        }

        public  int getBlockSize()
        {
            return blockSize;
        }

        public  void setBlockSize(int blockSize)
        {
            this.blockSize = blockSize;
        }

        public  byte[] getIdentifier()
        {
            return identifier;
        }

        public  byte[] getImage()
        {
            return image;
        }

    }

}
