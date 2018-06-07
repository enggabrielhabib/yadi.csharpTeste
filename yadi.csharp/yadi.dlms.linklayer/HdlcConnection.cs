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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yadi.dlms.linklayer
{
    internal class HdlcConnection
    {
        internal int windowSizeRx = 1;
        internal int windowSizeTx = 1;
        internal int maxInformationFieldRx = 128;
        internal int maxInformationFieldTx = 128;
        internal int sequenceNumberRX = 0;
        internal int sequenceNumberTX = 0;
        internal int receivedControl;
        internal int receivedRrr;
        internal int receivedSss;
        internal int sss;
        internal bool isFinalPoll;
        internal bool lastFrameHadSss;
        internal byte[] receivedData;

        internal void reset()
        {
            sequenceNumberRX = sequenceNumberTX = 0;
            windowSizeRx = windowSizeTx = 1;
            maxInformationFieldRx = maxInformationFieldTx = 128;
            sss = receivedRrr = receivedSss = 0;
            receivedControl = 0;
            isFinalPoll = false;
            lastFrameHadSss = false;
        }

        internal void incSss()
        {
            sss += 1;
            sss &= 0x07;
        }

        public void insReceivedSss()
        {
            if (lastFrameHadSss)
            {
                receivedSss += 1;
                receivedSss &= 0x07;
            }
        }

    }

}