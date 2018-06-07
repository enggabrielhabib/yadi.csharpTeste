
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
using System.Linq;
using System.Security.Cryptography;
using static yadi.dlms.cosem.CosemParameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace yadi.dlms.cosem
{
    using SecurityType = yadi.dlms.cosem.CosemParameters.SecurityType;
    using helper = yadi.csharp.Extensions;

    public class Security
    {
        private const int SC_ENCRYPTION = 0x20;
        private const int SC_AUTHENTICATION = 0x10;
        private const int SC_AUTHENTICATION_ENCRYPTION = 0x30;
        private static readonly object cipherLocker = new object();
        private static readonly object randomLocker = new object();
        private static SecureRandom sr = new SecureRandom();
        public static readonly int MacBitSize = 96;

        static Security()
        {
            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                sr.SetSeed(sr.GenerateSeed(16));
            }
            catch (NoSuchAlgorithmException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }

        public static byte[] authenticatedEncryption(CosemParameters parameters, byte[] data)
        {
            if (parameters.securityType == SecurityType.NONE)
            {
                return data;
            }
            int ivCounter = parameters.getInvocationCounter();
            int sc = 0;
            switch (parameters.securityType)
            {
                case SecurityType.AUTHENTICATION:
                    sc = SC_AUTHENTICATION;
                    byte[] authData = new byte[parameters.ak.Length + data.Length + 1];
                    authData[0] = SC_AUTHENTICATION;
                    Array.Copy(parameters.ak, 0, authData, 1, parameters.ak.Length);
                    Array.Copy(data, 0, authData, parameters.ak.Length + 1, data.Length);
                    byte[] mac = aesGcm(new byte[0], authData, parameters, ivCounter);
                    byte[] data_ = new byte[data.Length + mac.Length];
                    Array.Copy(data, 0, data_, 0, data.Length);
                    Array.Copy(mac, 0, data_, data.Length, mac.Length);
                    data = data_;
                    break;
                case SecurityType.AUTHENTICATION_ENCRYPTION:
                    sc = SC_AUTHENTICATION_ENCRYPTION;
                    authData = new byte[parameters.ak.Length + 1];
                    authData[0] = SC_AUTHENTICATION_ENCRYPTION;
                    Array.Copy(parameters.ak, 0, authData, 1, parameters.ak.Length);
                    data = aesGcm(data, authData, parameters, ivCounter);
                    break;
                case SecurityType.ENCRYPTION:
                    sc = SC_ENCRYPTION;
                    data = aesGcm(data, new byte[0], parameters, ivCounter);
                    break;
                default:
                    throw new System.InvalidOperationException();
            }

            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                stream.WriteByte((byte)sc);
                stream.WriteByte((byte)(ivCounter >> 24));
                stream.WriteByte((byte)(ivCounter >> 16));
                stream.WriteByte((byte)(ivCounter >> 8));
                stream.WriteByte((byte)(ivCounter));
                stream.Write(data, 0, data.Length);
                return stream.ToArray();
            }
            catch (IOException)
            {
                throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
            }
        }

        public static byte[] reverseAuthenticatedEncryption(CosemParameters parameters, CosemConnection connection, byte[] data)
        {
            switch (data[0] & 0xff)
            {
                case SC_AUTHENTICATION:
                    byte[] aux = helper.extensions.copyOfRange(data, 1, 5);
                    connection.serverInvocationCounter = BitConverter.ToInt32(new byte[] { (aux[3]), (aux[2]), (aux[1]), (aux[0]) }, 0);
                    return aesGcmReverse(new byte[0], helper.extensions.copyOfRange(data, 5, data.Length), parameters, connection);
                case SC_AUTHENTICATION_ENCRYPTION:
                    byte[] authData = new byte[parameters.ak.Length + 1];
                    authData[0] = SC_AUTHENTICATION_ENCRYPTION;
                    Array.Copy(parameters.ak, 0, authData, 1, parameters.ak.Length);
                    aux = helper.extensions.copyOfRange(data, 1, 5);
                    connection.serverInvocationCounter = BitConverter.ToInt32(new byte[] { (aux[3]), (aux[2]), (aux[1]), (aux[0]) }, 0);
                    return aesGcmReverse(helper.extensions.copyOfRange(data, 5, data.Length), authData, parameters, connection);
                case SC_ENCRYPTION:
                    aux = helper.extensions.copyOfRange(data, 1, 5);
                    connection.serverInvocationCounter = BitConverter.ToInt32(new byte[] { (aux[3]), (aux[2]), (aux[1]), (aux[0]) }, 0);
                    return aesGcmReverse(helper.extensions.copyOfRange(data, 5, data.Length), new byte[0], parameters, connection);
                default:
                    return data;
            }
        }

        public static byte[] aesGcm(byte[] data, byte[] authData, CosemParameters parameters, int ivCounter)
        {
            try
            {
                byte[] iv = getIv(parameters.systemTitle, ivCounter);
                lock (cipherLocker)
                {
                    var cipher = new GcmBlockCipher(new AesEngine());
                    cipher.Init(true, new AeadParameters(new KeyParameter(parameters.ek), MacBitSize, iv));
                    var cipherText = new byte[cipher.GetOutputSize(data.Length)];
                    var len = cipher.ProcessBytes(data, 0, data.Length, cipherText, 0);
                    cipher.ProcessAadBytes(authData, 0, authData.Length);
                    cipher.DoFinal(cipherText, len);
                    return cipherText;
                }
            }
            catch (InvalidKeyException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (InvalidParameterException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            //catch (AEADBadTagException)
            //{
            //    throw new DlmsException(DlmsException.DlmsExceptionReason.SECURITY_FAIL);
            //}
            //catch (BadPaddingException e)
            //{
            //    Console.WriteLine(e.ToString());
            //    Console.Write(e.StackTrace);
            //}
            throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
        }


        public static byte[] aesGcmReverse(byte[] encrypted, byte[] authData, CosemParameters parameters, CosemConnection connection)
        {
            try
            {
                byte[] iv = getIv(connection.serverSysTitle, connection.serverInvocationCounter);
                lock (cipherLocker)
                {
                    using (var cipherStream = new MemoryStream(encrypted))
                    using (var cipherReader = new BinaryReader(cipherStream))
                    {
                        var cipher = new GcmBlockCipher(new AesEngine());
                        cipher.Init(false, new AeadParameters(new KeyParameter(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }), MacBitSize, iv));
                        var cipherText = cipherReader.ReadBytes(encrypted.Length);
                        var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
                        var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
                        cipher.ProcessAadBytes(authData, 0, authData.Length);
                        cipher.DoFinal(plainText, len);
                        return plainText;
                    }
                }
            }
            catch (InvalidKeyException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            //catch (InvalidAlgorithmParameterException e)
            catch (InvalidParameterException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            //catch (IllegalBlockSizeException e)
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            //catch (AEADBadTagException)
            //{
            //    throw new DlmsException(DlmsException.DlmsExceptionReason.SECURITY_FAIL);
            //}
            //catch (BadPaddingException e)
            //{
            //    Console.WriteLine(e.ToString());
            //    Console.Write(e.StackTrace);
            //}
            throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
        }

        public static byte[] getIv(byte[] sysTitle, int invocationCounter)
        {
            byte[] iv = new byte[sysTitle.Length + 4];
            byte[] iCounter = new byte[] { (byte)(invocationCounter >> 24), (byte)(invocationCounter >> 16), (byte)(invocationCounter >> 8), (byte)(invocationCounter) };
            Array.Copy(sysTitle, 0, iv, 0, sysTitle.Length);
            Array.Copy(iCounter, 0, iv, sysTitle.Length, 4);
            return iv;
        }

        internal static byte[] generateChallanger(CosemParameters parameters)
        {
            byte[] random = new byte[parameters.challengerSize];
            lock (randomLocker)
            {
                sr.NextBytes(random);
            }
            return random;
        }

        internal static byte[] processChallanger(CosemParameters parameters, CosemConnection connection)
        {
            try
            {
                switch (parameters.authenticationType.innerEnumValue)
                {
                    case AuthenticationType.InnerEnum.PUBLIC:
                    case AuthenticationType.InnerEnum.LLS:
                        throw new System.InvalidOperationException();
                    case AuthenticationType.InnerEnum.HLS:
                        return aes128(connection.challengeServerToClient, parameters.llsHlsSecret);
                    case AuthenticationType.InnerEnum.HLS_MD5:
                        return md5(connection.challengeServerToClient, parameters.llsHlsSecret);
                    case AuthenticationType.InnerEnum.HLS_SHA1:
                        return sha1(connection.challengeServerToClient, parameters.llsHlsSecret);
                    case AuthenticationType.InnerEnum.HLS_GMAC:
                        int ivCounter = parameters.getInvocationCounter();
                        System.IO.MemoryStream data = new System.IO.MemoryStream();
                        data.WriteByte(SC_AUTHENTICATION);
                        data.Write(parameters.ak, 0, parameters.ak.Length);
                        data.Write(connection.challengeServerToClient, 0, connection.challengeServerToClient.Length);
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        stream.WriteByte(SC_AUTHENTICATION);
                        stream.WriteByte((byte)(ivCounter >> 24));
                        stream.WriteByte((byte)(ivCounter >> 16));
                        stream.WriteByte((byte)(ivCounter >> 8));
                        stream.WriteByte((byte)(ivCounter));
                        byte[] aux = Security.aesGcm(new byte[0], data.ToArray(), parameters, ivCounter);
                        stream.Write(aux, 0, aux.Length);
                        return stream.ToArray();
                    default:
                        throw new System.ArgumentException();
                }
            }
            catch (IOException)
            {
                throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
            }
        }

        internal static bool verifyChallenger(CosemParameters parameters, CosemConnection connection, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return false;
            }
            try
            {
                byte[] calculated = new byte[0];
                switch (parameters.authenticationType.innerEnumValue)
                {
                    case AuthenticationType.InnerEnum.PUBLIC:
                    case AuthenticationType.InnerEnum.LLS:
                        throw new System.InvalidOperationException();
                    case AuthenticationType.InnerEnum.HLS:
                        calculated = aes128(connection.challengeServerToClient, parameters.llsHlsSecret);
                        break;
                    case AuthenticationType.InnerEnum.HLS_MD5:
                        calculated = md5(connection.challengeClientToServer, parameters.llsHlsSecret);
                        break;
                    case AuthenticationType.InnerEnum.HLS_SHA1:
                        calculated = sha1(connection.challengeClientToServer, parameters.llsHlsSecret);
                        break;
                    case AuthenticationType.InnerEnum.HLS_GMAC:
                        if (data[0] != SC_AUTHENTICATION)
                        {
                            return false;
                        }
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        stream.WriteByte(SC_AUTHENTICATION);
                        stream.Write(parameters.ak, 0, parameters.ak.Length);
                        stream.Write(connection.challengeClientToServer, 0, connection.challengeClientToServer.Length);
                        //connection.serverInvocationCounter = BitConverter.ToInt32(helper.extensions.copyOfRange(data, 1, 5), 0);
                        var aux = helper.extensions.copyOfRange(data, 1, 5);
                        connection.serverInvocationCounter = BitConverter.ToInt32(new byte[] { (aux[3]), (aux[2]), (aux[1]), (aux[0]) }, 0);
                        data = helper.extensions.copyOfRange(data, 5, data.Length);
                        CosemParameters cosemParams = new CosemParameters();
                        cosemParams.setSystemTitle(connection.serverSysTitle);
                        cosemParams.setEk(parameters.ek);
                        calculated = Security.aesGcm(new byte[0], stream.ToArray(), cosemParams, connection.serverInvocationCounter);
                        break;
                    default:
                        throw new System.ArgumentException();
                }
                return Enumerable.SequenceEqual(data, calculated);
            }
            catch (IOException)
            {
                throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
            }
        }

        private static byte[] aes128(byte[] challenger, byte[] secret)
        {
            try
            {
                int len = Math.Max(challenger.Length, secret.Length);
                while ((len & 0x0F) != 0)
                {
                    len++;
                }
                byte[] key = new byte[len];
                byte[] data = new byte[len];
                Array.Copy(secret, 0, key, 0, secret.Length);
                Array.Copy(challenger, 0, data, 0, challenger.Length);

                AesManaged aes = new AesManaged();
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                ICryptoTransform crypt = aes.CreateEncryptor();
                return crypt.TransformFinalBlock(secret, 0, secret.Length);
            }
            catch (NoSuchAlgorithmException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (InvalidKeyException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            //catch (BadPaddingException e)
            //{
            //    Console.WriteLine(e.ToString());
            //    Console.Write(e.StackTrace);
            //}
            //catch (NoSuchPaddingException e)
            //{
            //    Console.WriteLine(e.ToString());
            //    Console.Write(e.StackTrace);
            //}
            throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
        }

        private static byte[] md5(byte[] challenger, byte[] secret)
        {
            //return getDigest(challenger, secret, "MD5");
            try
            {
                byte[] data = new byte[challenger.Length + secret.Length];
                Array.Copy(challenger, 0, data, 0, challenger.Length);
                Array.Copy(secret, 0, data, challenger.Length, secret.Length);
                MD5 md5 = MD5.Create();
                return md5.ComputeHash(secret);
            }
            catch (NoSuchAlgorithmException)
            {
                throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
            }
        }

        private static byte[] sha1(byte[] challenger, byte[] secret)
        {
            //return getDigest(challenger, secret, "SHA-1");
            try
            {
                byte[] data = new byte[challenger.Length + secret.Length];
                Array.Copy(challenger, 0, data, 0, challenger.Length);
                Array.Copy(secret, 0, data, challenger.Length, secret.Length);
                SHA1 sha1 = SHA1Managed.Create();
                return sha1.ComputeHash(secret);
            }
            catch (NoSuchAlgorithmException)
            {
                throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
            }
        }

        //private static byte[] getDigest(byte[] challenger, byte[] secret, String algorithm)
        //{
        //    try
        //    {
        //        byte[] data = new byte[challenger.Length + secret.Length];
        //        Array.Copy(challenger, 0, data, 0, challenger.Length);
        //        Array.Copy(secret, 0, data, challenger.Length, secret.Length);
        //        return java.security.MessageDigest.getInstance(algorithm).digest(data);
        //    }
        //    catch (java.security.NoSuchAlgorithmException)
        //    {
        //        throw new DlmsException(DlmsException.DlmsExceptionReason.INTERNAL_ERROR);
        //    }
        //}

    }

}