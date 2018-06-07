﻿/*
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
namespace yadi.dlms.classes
{
    using CosemClasses = yadi.dlms.cosem.CosemClasses;
    using LnDescriptor = yadi.dlms.cosem.LnDescriptor;
    using LinkLayerException = yadi.dlms.linklayer.LinkLayerException;
    using PhyLayer = yadi.dlms.phylayer.PhyLayer;
    using PhyLayerException = yadi.dlms.phylayer.PhyLayerException;

    public class RegisterObject
    {
        private const int attValue = 2;
        private const int attScalarUnit = 3;
        private const int mtdReset = 1;

        private readonly Obis obis;

        /// <summary>
        /// Creates a Register class (class_id=2) object </summary>
        /// <param name="obis"> the object obis </param>
        public RegisterObject(Obis obis)
        {
            this.obis = obis;
        }

        public  void reset(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor desc = new LnDescriptor(CosemClasses.REGISTER.id, obis, mtdReset);
            dlms.action(phy, desc);
        }

        public  byte[] getUnityScalar(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor desc = new LnDescriptor(CosemClasses.REGISTER.id, obis, attScalarUnit);
            dlms.get(phy, desc);
            return desc.getResponseData();
        }

        public  byte[] getValue(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor desc = new LnDescriptor(CosemClasses.REGISTER.id, obis, attValue);
            dlms.get(phy, desc);
            return desc.getResponseData();
        }

        public  string getStringValue(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor desc = new LnDescriptor(CosemClasses.REGISTER.id, obis, attValue);
            dlms.get(phy, desc);
            return DlmsParser.getString(desc.getResponseData());
        }

        public  int getIntegerValue(DlmsClient dlms, PhyLayer phy)
        {
            LnDescriptor desc = new LnDescriptor(CosemClasses.REGISTER.id, obis, attValue);
            dlms.get(phy, desc);
            return DlmsParser.getInteger(desc.getResponseData());
        }
    }
}
