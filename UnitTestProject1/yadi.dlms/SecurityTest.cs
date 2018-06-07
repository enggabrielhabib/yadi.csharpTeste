using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using yadi.dlms.cosem;
using static yadi.dlms.cosem.CosemParameters;

namespace UnitTestProject1.yadi.dlms
{

    [TestClass]
    public class SecurityTest
    {
        private static readonly object cipherLocker = new object();

        [TestMethod]
        public void aesGcmRevTest()
        {
            byte[] encrypted = { 0x0D, 0xB6, 0x59, 0xB3, 0x0B, 0x7A, 0x91, 0x5C, 0x46, 0x9F, 0x01, 0x01, 0x83, 0x63, 0x1F, 0x20, 0xCB, 0x28, 0xD2, 0x6D, 0xD0, 0x5E, 0xC9, 0xB5, 0x2F, 0x6E };
            byte[] authData = { 0x30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var MacBitSize = 96;

            //byte[] iv = getIv(connection.serverSysTitle, connection.serverInvocationCounter);
            byte[] iv = Security.getIv(new byte[] { 0x48, 0x58, 0x45, 0x03, 0x37, 0x4A, 0x1B, 0x08 }, 15582);
            //byte[] iv = { 0x48, 0x58, 0x45, 0x03, 0x37, 0x4A, 0x1B, 0x08, 0x00, 0x00, 0x3C, 0xDE };

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
                    var aux = plainText;
                    CollectionAssert.AreEqual(new byte[] { 8, 0, 6, 95, 31, 4, 0, 0, 0, 31, 1, 0, 0, 7 }, plainText);
                }   
            }


        }

        [TestMethod]
        public void aesGcmTest()
        {
            byte[] data = { 8, 0, 6, 95, 31, 4, 0, 0, 0, 31, 1, 0, 0, 7 };
            byte[] authData = { 0x30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var MacBitSize = 96;

            byte[] iv = Security.getIv(new byte[] { 0x48, 0x58, 0x45, 0x03, 0x37, 0x4A, 0x1B, 0x08 }, 15582);
            lock (cipherLocker)
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                cipher.Init(true, new AeadParameters(new KeyParameter(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }), MacBitSize, iv));
                var cipherText = new byte[cipher.GetOutputSize(data.Length)];
                var len = cipher.ProcessBytes(data, 0, data.Length, cipherText, 0);
                cipher.ProcessAadBytes(authData, 0, authData.Length);
                cipher.DoFinal(cipherText, len);
                byte[] expected = { 0x0D, 0xB6, 0x59, 0xB3, 0x0B, 0x7A, 0x91, 0x5C, 0x46, 0x9F, 0x01, 0x01, 0x83, 0x63, 0x1F, 0x20, 0xCB, 0x28, 0xD2, 0x6D, 0xD0, 0x5E, 0xC9, 0xB5, 0x2F, 0x6E };
                CollectionAssert.AreEqual(expected, cipherText);
            }
        }


        [TestMethod]
        public void authenticatedEncryptionTest()
        {

            byte[][] akArray = new byte[][]{    new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                                new byte[] { 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0 },
                                                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }};

            byte[][] ekArray = new byte[][]{    new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                                new byte[] { 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0 },
                                                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }};

            byte[][] dataArray = new byte[][] { new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF },
                                                new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF },
                                                new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF }};

            byte[][] expected = new byte[][] { new byte[] { 48, 0, 0, 0, 1, 112, 0xC1, 46, 0x82, 0xE5, 0xBD, 0x94, 0x94, 70, 0xBB, 61, 63, 0xF2, 0xFA, 0xCC, 0xB1, 0xB0, 60, 88, 116, 6, 0xA0, 0xFA, 0xCC, 77 },
                                               new byte[] { 48, 0, 0, 0, 1, 97, 0xC9, 0xD6, 79, 114, 100, 0xB7, 90, 21, 0xE5, 53, 34, 0x92, 45, 77, 55, 0x97, 110, 69, 89, 93, 0x9D, 0x88, 60, 33 },
                                               new byte[] { 48, 0, 0, 0, 1, 89, 0xAF, 0xFF, 0xCA, 0xA0, 88, 0xA4, 27, 55, 0x8E, 0x9A, 0xEE, 24, 46, 114, 0x91, 0xB4, 0xDC, 0x9C, 0xDD, 95, 114, 122, 0xDA, 0xE9 }};


            CosemParameters cosemparams = new CosemParameters();
            CosemConnection cosemconn = new CosemConnection();

            cosemparams.setAuthenticationType(AuthenticationType.HLS_GMAC);
            cosemparams.setSecurityType(SecurityType.AUTHENTICATION_ENCRYPTION);

            //cosemparams.setAk(akArray[0]);
            //cosemparams.setEk(ekArray[0]);
            //var actual = Security.authenticatedEncryption(cosemparams, dataArray[0]);
            //CollectionAssert.AreEqual(expected[0], actual);

            //cosemparams.setAk(akArray[1]);
            //cosemparams.setEk(ekArray[1]);
            //actual = Security.authenticatedEncryption(cosemparams, dataArray[1]);
            //CollectionAssert.AreEqual(expected[1], actual);

            cosemparams.setAk(akArray[2]);
            cosemparams.setEk(ekArray[2]);
            var actual = Security.authenticatedEncryption(cosemparams, dataArray[2]);
            CollectionAssert.AreEqual(expected[2], actual);

        }

        [TestMethod]
        public void reverseAuthenticatedEncryptionTest()
        {

            byte[][] akArray = new byte[][]{    new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                                new byte[] { 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0 },
                                                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }};

            byte[][] ekArray = new byte[][]{    new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                                new byte[] { 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0 },
                                                new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }};

            byte[][] dataArray = new byte[][] { new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF },
                                                new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF },
                                                new byte[] { 1, 0, 0, 0, 6, 95, 4, 0, 0xB8, 56, 0, 0xFF, 0xFF }};

            byte[][] expected = new byte[][] { new byte[] { 48, 0, 0, 0, 1, 112, 0xC1, 46, 0x82, 0xE5, 0xBD, 0x94, 0x94, 70, 0xBB, 61, 63, 0xF2, 0xFA, 0xCC, 0xB1, 0xB0, 60, 88, 116, 6, 0xA0, 0xFA, 0xCC, 77 },
                                               new byte[] { 48, 0, 0, 0, 1, 97, 0xC9, 0xD6, 79, 114, 100, 0xB7, 90, 21, 0xE5, 53, 34, 0x92, 45, 77, 55, 0x97, 110, 69, 89, 93, 0x9D, 0x88, 60, 33 },
                                               new byte[] { 48, 0, 0, 0, 1, 89, 0xAF, 0xFF, 0xCA, 0xA0, 88, 0xA4, 27, 55, 0x8E, 0x9A, 0xEE, 24, 46, 114, 0x91, 0xB4, 0xDC, 0x9C, 0xDD, 95, 114, 122, 0xDA, 0xE9 }};


            CosemParameters cosemparams1 = new CosemParameters();
            CosemConnection cosemconn1 = new CosemConnection();

            cosemparams1.setAuthenticationType(AuthenticationType.HLS_GMAC);
            cosemparams1.setSecurityType(SecurityType.AUTHENTICATION_ENCRYPTION);

            //cosemparams.setAk(akArray[0]);
            //cosemparams.setEk(ekArray[0]);
            //var actual = Security.authenticatedEncryption(cosemparams, dataArray[0]);
            //CollectionAssert.AreEqual(expected[0], actual);

            //cosemparams.setAk(akArray[1]);
            //cosemparams.setEk(ekArray[1]);
            //var actual = Security.authenticatedEncryption(cosemparams, dataArray[1]);
            //CollectionAssert.AreEqual(expected[1], actual);

            cosemparams1.setAk(akArray[2]);
            cosemparams1.setEk(ekArray[2]);
            var actual = Security.authenticatedEncryption(cosemparams1, dataArray[2]);
            CollectionAssert.AreEqual(expected[2], actual);

        }

    }

}
