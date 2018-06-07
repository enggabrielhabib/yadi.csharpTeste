
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

    using ImageTransferExceptionReason = yadi.dlms.ImageTransferException.ImageTransferExceptionReason;
    using CosemClasses = yadi.dlms.cosem.CosemClasses;
    using LnDescriptor = yadi.dlms.cosem.LnDescriptor;
    using LinkLayerException = yadi.dlms.linklayer.LinkLayerException;
    using PhyLayer = yadi.dlms.phylayer.PhyLayer;
    using PhyLayerException = yadi.dlms.phylayer.PhyLayerException;
    using System.IO;
    using System;

    public class ImageTransfer
    {

        /// <summary>
        /// Class ID
        /// </summary>
        private static readonly int CLASS_ID = CosemClasses.IMAGE_TRANSFER.id;

        /// <summary>
        /// Attributes
        /// </summary>
        private const int attBlockSize = 2;
        private const int attTransferredBlockStatus = 3;
        private const int attFirstNotTransferredBlockNumber = 4;
        private const int attTransferEnabled = 5;
        private const int attTransferStatus = 6;
        private const int attImageToActiveInfo = 7;

        /// <summary>
        /// Methods
        /// </summary>
        private const int mtdTransferInitiate = 1;
        private const int mtdImageBlockTransfer = 2;
        private const int mtdImageVerify = 3;
        private const int mtdImageActivate = 4;

        private readonly Obis obis;

        public ImageTransfer() : this(new Obis("0.0.44.0.0.255"))
        {
        }

        public ImageTransfer(Obis obis)
        {
            this.obis = obis;
        }

        public  bool isTransferEnabled(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor att = createDesc(attTransferEnabled);
            dlms.get(phy, att);
            return DlmsParser.getBoolean(att.getResponseData());
        }

        public  int getImageBlockSize(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor att = createDesc(attBlockSize);
            dlms.get(phy, att);
            return DlmsParser.getInteger(att.getResponseData());
        }

        public  void initiateImageTransfer(DlmsClient dlms, PhyLayer phy, ImageInformation imageInfo)
        {
            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                stream.WriteByte(DlmsType.STRUCTURE.tag);
                stream.WriteByte(2); // size
                stream.WriteByte(DlmsType.OCTET_STRING.tag);
                stream.WriteByte((byte)(imageInfo.getIdentifier().Length));


                //stream.WriteByte(imageInfo.getIdentifier());
                byte[] aux = imageInfo.getIdentifier();
                //Array.Reverse(aux);
                stream.Write(aux, 0, aux.Length);

                stream.WriteByte(DlmsType.UINT32.tag);
                //stream.WriteByte(ByteBuffer.allocate(4).putInt(imageInfo.getImage().Length).array());
                stream.Write(new byte[] { (byte)(imageInfo.getImage().Length >> 24), (byte)(imageInfo.getImage().Length >> 16), (byte)(imageInfo.getImage().Length >> 8), (byte)(imageInfo.getImage().Length) }, 0, imageInfo.getImage().Length);
                dlms.action(phy, createDesc(mtdTransferInitiate, stream.ToArray()));
            }
            catch (IOException)
            {
                throw new ImageTransferException(ImageTransferExceptionReason.INTERNAL_ERROR);
            }

        }

        public  void transferBlocks(DlmsClient dlms, PhyLayer phy, ImageInformation imageInfo)
        {
            int offset = 0;
            int nblock = 1;
            LnDescriptor att = createDesc(mtdImageBlockTransfer);
            while (offset < imageInfo.getImage().Length)
            {
                att.setRequestData(getTransferBlockData(nblock, imageInfo));
                dlms.action(phy, att);
                nblock++;
                offset += imageInfo.getBlockSize();
            }
        }

        public  void checkCompleteness(DlmsClient dlms, PhyLayer phy, ImageInformation info)
        {
            //TODO
        }

        public  void verifyImage(DlmsClient dlms, PhyLayer phy)
        {
            dlms.action(phy, createDesc(mtdImageVerify, new byte[] { 0x0F, 0x00 }));
        }

        public  bool checkImageInformation(DlmsClient dlms, PhyLayer phy, ImageInformation info)
        {
            return false;
        }

        public  void activateImage(DlmsClient dlms, PhyLayer phy)
        {
            dlms.action(phy, createDesc(mtdImageActivate, new byte[] { 0x0F, 0x00 }));
        }

        public  void execute(DlmsClient dlms, PhyLayer phy, ImageInformation imageInfo)
        {

            /// Precondition: image transfer must be enabled
            if (!isTransferEnabled(dlms, phy))
            {
                throw new ImageTransferException(ImageTransferExceptionReason.TRANSFER_DISABLED);
            }

            /// Step 1: if image block size is unknown, get block size
            if (imageInfo.getBlockSize() == 0)
            {
                imageInfo.setBlockSize(getImageBlockSize(dlms, phy));
            }

            /// Step 2: Initiate image transfer
            initiateImageTransfer(dlms, phy, imageInfo);

            /// Step 3: Transfer image blocks
            transferBlocks(dlms, phy, imageInfo);

            /// Step 4: Check completeness of the image
            checkCompleteness(dlms, phy, imageInfo);

            /// Step 5: Verifies the image
            verifyImage(dlms, phy);

            /// Step 6: Check information of image to activate
            if (!checkImageInformation(dlms, phy, imageInfo))
            {
                throw new ImageTransferException(ImageTransferExceptionReason.INVALID_IMAGE_TO_ACTIVATE);
            }

            /// Step 7: Activates image
            activateImage(dlms, phy);
        }

        private byte[] getTransferBlockData(int nblock, ImageInformation imageInfo)
        {
            try
            {
                int len = imageInfo.getBlockSize();
                byte[] image = imageInfo.getImage();
                int offset = (nblock - 1) * len;
                if ((offset + len) > image.Length)
                {
                    len = image.Length - offset;
                }
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                stream.WriteByte(DlmsType.STRUCTURE.tag);
                stream.WriteByte(2); // size
                stream.WriteByte(DlmsType.UINT32.tag);
                //stream.WriteByte(ByteBuffer.allocate(4).putInt(nblock).array());
                stream.Write(new byte[] { (byte)(nblock >> 24), (byte)(nblock >> 16), (byte)(nblock >> 8), (byte)(nblock)}, 0, nblock.ToString().Length);
                stream.WriteByte(DlmsType.OCTET_STRING.tag);
                if (len >= 0x80 && len <= 0xFF)
                {
                    stream.WriteByte(0x81);
                }
                else if (len > 0xFF)
                {
                    throw new ImageTransferException(ImageTransferExceptionReason.INVALID_BLOCK_SIZE);
                }
                stream.WriteByte((byte)len);
                for (int i = 0; i < len; ++i)
                {
                    stream.WriteByte(image[offset + i]);
                }
                return stream.ToArray();
            }
            catch (IOException)
            {
                throw new ImageTransferException(ImageTransferExceptionReason.INTERNAL_ERROR);
            }
        }

        private LnDescriptor createDesc(int index)
        {
            return new LnDescriptor(CLASS_ID, obis, index);
        }

        private LnDescriptor createDesc(int index, byte[] data)
        {
            LnDescriptor att = createDesc(index);
            att.setRequestData(data);
            return att;
        }
    }
}
