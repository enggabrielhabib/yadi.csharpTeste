
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

using System.Collections.Generic;

namespace yadi.dlms.cosem
{

	public class CosemParameters
	{

		public class DataBlock
		{

			internal bool lastBlock;
			internal int blockNum;
			internal readonly System.IO.MemoryStream data;

			public DataBlock()
			{
				data = new System.IO.MemoryStream();
			}

			public void reset()
			{
				lastBlock = false;
				blockNum = 0;
				data.SetLength(0);
			}
		}

		/// <summary>
		/// COSEM connection parameters
		/// </summary>
		public class Connection
		{
            DataBlock datablock = new DataBlock();
			internal byte[] challengeServerToClient = new byte[0];
			internal byte[] challengeClientToServer = new byte[0];
			internal byte[] proposedContextName = new byte[0];
			internal byte[] conformanceBlock = new byte[0];
			internal byte[] serverSysTitle = new byte[0];
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

		/// <summary>
		/// Possible types of attributes references
		/// </summary>
		public enum ReferenceType
		{
			LOGICAL_NAME,
			SHORT_NAME
		}

		/// <summary>
		/// Possible types of security
		/// </summary>
		public enum SecurityType
		{
			NONE,
			AUTHENTICATION,
			ENCRYPTION,
			AUTHENTICATION_ENCRYPTION
		}

		/// <summary>
		/// Authentication types
		/// </summary>
		public class AuthenticationType
		{
			public static readonly AuthenticationType PUBLIC = new AuthenticationType("PUBLIC", InnerEnum.PUBLIC, 0);
			public static readonly AuthenticationType LLS = new AuthenticationType("LLS", InnerEnum.LLS, 1);
			public static readonly AuthenticationType HLS = new AuthenticationType("HLS", InnerEnum.HLS, 2);
			public static readonly AuthenticationType HLS_MD5 = new AuthenticationType("HLS_MD5", InnerEnum.HLS_MD5, 3);
			public static readonly AuthenticationType HLS_SHA1 = new AuthenticationType("HLS_SHA1", InnerEnum.HLS_SHA1, 4);
			public static readonly AuthenticationType HLS_GMAC = new AuthenticationType("HLS_GMAC", InnerEnum.HLS_GMAC, 5);

			private static readonly IList<AuthenticationType> valueList = new List<AuthenticationType>();

			static AuthenticationType()
			{
				valueList.Add(PUBLIC);
				valueList.Add(LLS);
				valueList.Add(HLS);
				valueList.Add(HLS_MD5);
				valueList.Add(HLS_SHA1);
				valueList.Add(HLS_GMAC);
			}

			public enum InnerEnum
			{
				PUBLIC,
				LLS,
				HLS,
				HLS_MD5,
				HLS_SHA1,
				HLS_GMAC
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal = 0;
			internal int value;
			internal AuthenticationType(string name, InnerEnum innerEnum, int value)
			{
				this.value = value;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			public static IList<AuthenticationType> values()
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

			public static AuthenticationType valueOf(string name)
			{
				foreach (AuthenticationType enumInstance in AuthenticationType.valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new System.ArgumentException(name);
			}
		}

		/// <summary>
		/// Priorities
		/// </summary>
		public enum PriorityType
		{
			NORMAL,
			HIGH
		}

		/// <summary>
		/// Service class type
		/// </summary>
		public enum ServiceClassType
		{
			UNCONFIRMED,
			CONFIRMED
		}

		///Connection connection = new Connection();
		internal AuthenticationType authenticationType = AuthenticationType.PUBLIC;
		internal SecurityType securityType = SecurityType.NONE;
		internal ReferenceType referenceType = ReferenceType.LOGICAL_NAME;

        internal object locker = new object();
		internal static int invocationCounter = 0;
		internal int challengerSize = 8;
		internal int priority = Constants.PRIORITY_HIGH;
		internal int serviceClass = Constants.SERVICE_CLASS_CONFIRMED;
		internal short maxPduSize = unchecked((short)0xFFFF);

		internal byte[] llsHlsSecret = new byte[8];
		internal byte[] systemTitle = new byte[] {0x48, 0x45, 0x43, 0x00, 0x05, 0x00, 0x00, 0x01};
		internal byte[] ak = new byte[16];
		internal byte[] ek = new byte[16];

		/// <summary>
		/// Configures the number of bytes for the generated challengers </summary>
		/// <param name="size"> - number of bytes for generated challengers </param>
		public void setChallengerSize(int size)
		{
            if (size < 8 || size > 64)
            {
                throw new System.ArgumentException("Challenger must be between 8 and 64 bytes long");
            }
            this.challengerSize = size;
        }

        /// <summary>
        /// Sets the invocation counter value, it starts at zero by default. </summary>
        /// <param name="counter"> - invocation counter value </param>
        public void setInvocationCounter(int counter)
        {
            CosemParameters.invocationCounter = counter;
        }

        public int getInvocationCounter()
        {
            lock (locker)
            {
                CosemParameters.invocationCounter++;
                return CosemParameters.invocationCounter;
            }
        }

        /// <summary>
        /// Sets the secret used for LLS and HLS authentication </summary>
        /// <param name="llsPassword"> - LLS password value </param>
        public void setSecret(byte[] llsPassword)
		{
            this.llsHlsSecret = llsPassword;
		}

		/// <summary>
		/// Set the client system title </summary>
		/// <param name="systemTitle"> - system title value </param>
		public void setSystemTitle(byte[] systemTitle)
		{
            if (systemTitle.Length != 8)
            {
                throw new System.ArgumentException("System Title must be 8 bytes long");
            }
            this.systemTitle = systemTitle;
        }

		/// <summary>
		/// Sets the authentication key </summary>
		/// <param name="ak"> - authentication key value </param>
		public void setAk(byte[] ak)
        {
            if (ak.Length != 16)
            {
            throw new System.ArgumentException("AK must be 16 bytes long");
            }
            this.ak = ak;
        }

		/// <summary>
		/// Sets the encryption key </summary>
		/// <param name="ek"> - encryption key value </param>
		public void setEk(byte[] ek)
		{
            if (ek.Length != 16)
            {
                throw new System.ArgumentException("EK must be 16 bytes long");
            }
            this.ek = ek;
        }

		/// <summary>
		/// Sets the security type to be used </summary>
		/// <param name="securityType"> </param>
		public void setSecurityType(SecurityType securityType)
		{
			this.securityType = securityType;
		}

		/// <summary>
		/// Sets the authentication value to be used </summary>
		/// <param name="authenticationType"> </param>
		public void setAuthenticationType(AuthenticationType authenticationType)
		{
			this.authenticationType = authenticationType;
		}

		/// <summary>
		/// Sets the attribute reference type to be used </summary>
		/// <param name="type"> </param>
		public void setReferenceType(ReferenceType type)
		{
			this.referenceType = type;
		}

		/// <summary>
		/// Sets the maximum PDU size </summary>
		/// <param name="pduSize"> - pdu size in bytes </param>
		public void MaxPduSize(int pduSize)
		{
            this.maxPduSize = (short)pduSize;
		}

		/// <summary>
		/// Sets the service priority </summary>
		/// <param name="priority"> </param>
		public void setPriorityType(PriorityType priority)
		{
            switch (priority)
            {
                case yadi.dlms.cosem.CosemParameters.PriorityType.HIGH:
                    this.priority = Constants.PRIORITY_HIGH;
                    break;
                case yadi.dlms.cosem.CosemParameters.PriorityType.NORMAL:
                    this.priority = Constants.PRIORITY_NORMAL;
                    break;
                default:
                    throw new System.ArgumentException();
            }
        }

		/// <summary>
		/// Sets the service class type </summary>
		/// <param name="serviceClass"> </param>
		public void setServiceClassType(ServiceClassType serviceClass)
		{
            switch (serviceClass)
            {
                case yadi.dlms.cosem.CosemParameters.ServiceClassType.CONFIRMED:
                    this.serviceClass = Constants.SERVICE_CLASS_CONFIRMED;
                    break;
                case yadi.dlms.cosem.CosemParameters.ServiceClassType.UNCONFIRMED:
                    this.serviceClass = Constants.SERVICE_CLASS_UNCONFIRMED;
                    break;
                default:
                    throw new System.ArgumentException();
            }
        }

	}

}