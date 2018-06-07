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

namespace yadi.dlms.cosem
{

    using helper = yadi.csharp.Extensions;

    /// <summary>
    /// COSEM connection parameters
    /// </summary>
    public class CosemConnection
	{
		private bool InstanceFieldsInitialized = false;

		public CosemConnection()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			datablock = new DataBlock(this);
		}


		internal class DataBlock
		{
			private readonly CosemConnection outerInstance;

			internal bool lastBlock;
			internal int blockNum;
			internal readonly System.IO.MemoryStream data;
			internal byte[] blocks;
			internal int blockSize;
			internal int nextBlockNum;

			internal DataBlock(CosemConnection outerInstance)
			{
				this.outerInstance = outerInstance;
				data = new System.IO.MemoryStream();
			}

			internal  void reset()
			{
				blockNum = 0;
				blocks = new byte[0];
				blockSize = 0;
				nextBlockNum = 1;
				data.SetLength(0);
			}

			public  void setData(byte[] data, int len)
			{
				blocks = data;
				blockSize = len;
			}

			public  byte[] getBlock(int blockNum)
			{
				int offset = (blockNum - 1) * blockSize;
				if (offset > blocks.Length)
				{
					return null;
				}
				int to = offset + blockSize;
				if (to > blocks.Length)
				{
					to = blocks.Length;
				}
				return helper.extensions.copyOfRange(blocks, offset, to);
			}

			public  void ackBlock(int blockNum)
			{
				if (blockNum == nextBlockNum)
				{
					nextBlockNum++;
				}
			}

			public  byte[] getNextBlock()
			{
                return getBlock(nextBlockNum);
			}

			public  byte[] getNextBlockNum()
			{
                //byte[] data = ByteBuffer.allocate(4).putInt(nextBlockNum).array();
                byte[] data = new byte[] { (byte)(nextBlockNum >> 24), (byte)(nextBlockNum >> 16), (byte)(nextBlockNum >> 8), (byte)nextBlockNum };
				Console.WriteLine("block num = " + nextBlockNum);
				StringBuilder sb = new StringBuilder();
				foreach (byte b in data)
				{
					sb.Append(string.Format("{0:X2} ", b));
				}
				Console.WriteLine(sb);
				return data;
			}

			public  bool thisIsLast()
			{
				return getBlock(nextBlockNum) == null;
			}

			public  bool nextIsLast()
			{
				return getBlock(nextBlockNum + 1) == null;
			}
		}

		internal DataBlock datablock;
		internal byte[] challengeServerToClient = new byte[0];
		internal byte[] challengeClientToServer = new byte[0];
		internal byte[] proposedContextName = new byte[0];
		internal byte[] conformanceBlock = new byte[0];
		public byte[] serverSysTitle = new byte[0];
		internal int maxPduSize;
		internal int serverInvocationCounter;

		internal void reset()
		{
			challengeServerToClient = new byte[0];
			challengeClientToServer = new byte[0];
			proposedContextName = new byte[0];
			conformanceBlock = new byte[0];
			serverSysTitle = new byte[0];
			maxPduSize = 0;
			serverInvocationCounter = 0;
			datablock.reset();
		}
	}

}