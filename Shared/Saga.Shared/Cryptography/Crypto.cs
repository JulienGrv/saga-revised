using System;
using System.Security.Cryptography;
using System.Text;

namespace Saga.Shared.PacketLib
{
    /// <summary>
    /// The encryption class provides static methods for encrypting/decrypting data or to generate expanded keys
    /// given a certain key. Encryption method is based on AES (With some bytes swapped here and there) and
    /// XOR.
    /// </summary>
    public class Encryption
    {
        private static byte[] XorKey = { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10, 0x0F, 0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78 };

        /// <summary>
        /// The static key is the key used to encrypt the first messages between client and server.
        /// In these messages a key exchange will take place which will be used for further communication.
        /// </summary>
        public static byte[] StaticKey = { 0x40, 0x21, 0xBF, 0xE4, 0xB0, 0xC7, 0xB8, 0xF0, 0xB8, 0xA3, 0xB0, 0xDA, 0xC1, 0xF6, 0x24, 0x00 };

        /// <summary>
        /// Generate a random 16 bytes AES key.
        /// </summary>
        /// <returns>16byte aes key.</returns>
        public static byte[] GenerateKey()
        {
            Random r = new Random();
            byte[] key = new byte[16];
            r.NextBytes(key);
            return key;
        }

        private static void SwapBytes(byte[] buffer, int len, int c)
        {
            for (int i = 0; i < len / 4; i++)
            {
                buffer[c + i * 4] ^= buffer[c + i * 4 + 3];
                buffer[c + i * 4 + 3] ^= buffer[c + i * 4];
                buffer[c + i * 4] ^= buffer[c + i * 4 + 3];

                buffer[c + i * 4 + 1] ^= buffer[c + i * 4 + 2];
                buffer[c + i * 4 + 2] ^= buffer[c + i * 4 + 1];
                buffer[c + i * 4 + 1] ^= buffer[c + i * 4 + 2];
            }
        }

        private static byte[] Crypt(byte[] data, int offset, byte[] orgkey, bool encrypt)
        {
            Rijndael aes = Rijndael.Create();

            byte[] aeskey = new byte[16];
            orgkey.CopyTo(aeskey, 0);
            byte[] iv = new byte[16]; // empty - not used for ECB
            int aesLen = 0;

            SwapBytes(aeskey, aeskey.Length, 0);

            aesLen = data.Length - offset;
            aesLen -= aesLen % 16;

            SwapBytes(data, aesLen, offset);

            aes.Mode = CipherMode.ECB;
            aes.KeySize = 128;
            aes.Padding = PaddingMode.None;

            if (encrypt)
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(aeskey, iv);
                if (aesLen > 0) encryptor.TransformBlock(data, offset, aesLen, data, offset);
            }
            else
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(aeskey, iv);
                if (aesLen > 0) decryptor.TransformBlock(data, offset, aesLen, data, offset);
            }
            SwapBytes(data, aesLen, offset);

            // Apply XOR to the rest of buffer
            for (int i = 0; i < data.Length - aesLen - offset; i++)
            {
                data[offset + aesLen + i] ^= XorKey[i];
            }

            return data;
        }

        /// <summary>
        /// Encrypt/Decrypt a given message with a given key.
        /// </summary>
        /// <param name="msg">msg to encrypt/decrypt</param>
        /// <param name="key">aes key to use</param>
        /// <param name="encrypt">true: message will be encrypted. false: message will be decrypted.</param>
        /// <returns>Encrypted/Decrypted message</returns>
        public static string Crypt(string msg, byte[] key, bool encrypt)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] crypted = Crypt(encoding.GetBytes(msg), 0, key, encrypt);
            return encoding.GetString(crypted);
        }

        /// <summary>
        /// Encrypt a byte array with a given key.
        /// </summary>
        /// <param name="data">bytes to encrypt</param>
        /// <param name="key">aes key to use</param>
        /// <returns>encrypted bytes</returns>
        public static byte[] Encrypt(byte[] data, int offset, byte[] key)
        {
            return Crypt(data, offset, key, true);
        }

        /// <summary>
        /// Decrypt a byte array with a given key.
        /// </summary>
        /// <param name="data">bytes to decrypt</param>
        /// <param name="key">aes key to use</param>
        /// <returns>decrypted bytes</returns>
        public static byte[] Decrypt(byte[] data, int offset, byte[] key)
        {
            return Crypt(data, offset, key, false);
        }

        /// <summary>
        /// Generate the expanded key that will be used during decryption.
        /// </summary>
        /// <param name="key">AES key to expand.</param>
        /// <returns>Expanded decryption key.</returns>
        public static byte[] GenerateDecExpKey(byte[] key)
        {
            int Nb = (128 >> 5);
            int Nk = (128 >> 5);
            int Nr = Nk + 6;
            int exKeySize = Nb * (Nr + 1);

            Int32[] tk = new Int32[Nk];
            Int32[] exKey = new Int32[exKeySize];
            int pos = 0, idx = 0, i = 0, j = 0;

            //Copy user material bytes into temporary ints
            for (i = 0; i < Nk; i++)
            {
                tk[i] = key[pos++] |
                        (key[pos++] << 8) |
                        (key[pos++] << 16) |
                        (key[pos++] << 24);
                exKey[i] = tk[i];
            }
            while (i < exKeySize)
            {
                //Extrapolate using phi (the round key evolution function)
                Int32 tt = tk[Nk - 1];
                tk[0] ^= sbox[(tt >> 16) & 0xFF] << 24 ^
                    sbox[(tt >> 8) & 0xFF] << 16 ^
                    sbox[tt & 0xFF] << 8 ^
                    sbox[(tt >> 24) & 0xFF] ^
                    rcon[idx++] << 24;

                for (j = 0; j < Nk - 1; j++)
                    tk[j + 1] ^= tk[j];

                for (j = 0; j < Nk && i < exKeySize; j++, i++)
                {
                    tt = tk[j];
                    if (i < exKeySize - Nk)
                    {
                        //Inverse MixColumn
                        UInt32 U0, U1, U2, U3;

                        U0 = imbox[(tt >> 24) & 0xFF];
                        U1 = imbox[(tt >> 16) & 0xFF];
                        U2 = imbox[(tt >> 8) & 0xFF];
                        U3 = imbox[tt & 0xFF];

                        U1 = (U1 >> 8) | (U1 << 24);
                        U2 = (U2 >> 16) | (U2 << 16);
                        U3 = (U3 >> 24) | (U3 << 8);

                        tt = (Int32)(U0 ^ U1 ^ U2 ^ U3);
                    }
                    exKey[i] = tt;
                }
            }

            byte[] expandedKey = new byte[exKey.Length << 2];
            j = 0;
            for (i = 0; i < exKey.Length; i++)
            {
                expandedKey[j++] = (byte)(exKey[i] & 0xff);
                expandedKey[j++] = (byte)((exKey[i] >> 8) & 0xff);
                expandedKey[j++] = (byte)((exKey[i] >> 16) & 0xff);
                expandedKey[j++] = (byte)(exKey[i] >> 24);
            }
            return expandedKey;
        }

        private static byte[] sbox =
        {
	        0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5,
	        0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
	        0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,
	        0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
	        0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc,
	        0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
	        0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a,
	        0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
	        0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0,
	        0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
	        0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b,
	        0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
	        0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85,
	        0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
	        0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5,
	        0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
	        0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17,
	        0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
	        0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88,
	        0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
	        0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,
	        0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
	        0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9,
	        0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
	        0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6,
	        0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
	        0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e,
	        0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
	        0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94,
	        0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
	        0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68,
	        0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
        };

        private static byte[] rcon =
        {
	        0x01, 0x02, 0x04, 0x08, 0x10, 0x20,
	        0x40, 0x80, 0x1b, 0x36, 0x6c, 0xc0,
	        0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc,
	        0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4,
	        0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91
        };

        private static UInt32[] imbox =
        {
	        0x00000000, 0x0e090d0b, 0x1c121a16, 0x121b171d,
	        0x3824342c, 0x362d3927, 0x24362e3a, 0x2a3f2331,
	        0x70486858, 0x7e416553, 0x6c5a724e, 0x62537f45,
	        0x486c5c74, 0x4665517f, 0x547e4662, 0x5a774b69,
	        0xe090d0b0, 0xee99ddbb, 0xfc82caa6, 0xf28bc7ad,
	        0xd8b4e49c, 0xd6bde997, 0xc4a6fe8a, 0xcaaff381,
	        0x90d8b8e8, 0x9ed1b5e3, 0x8ccaa2fe, 0x82c3aff5,
	        0xa8fc8cc4, 0xa6f581cf, 0xb4ee96d2, 0xbae79bd9,
	        0xdb3bbb7b, 0xd532b670, 0xc729a16d, 0xc920ac66,
	        0xe31f8f57, 0xed16825c, 0xff0d9541, 0xf104984a,
	        0xab73d323, 0xa57ade28, 0xb761c935, 0xb968c43e,
	        0x9357e70f, 0x9d5eea04, 0x8f45fd19, 0x814cf012,
	        0x3bab6bcb, 0x35a266c0, 0x27b971dd, 0x29b07cd6,
	        0x038f5fe7, 0x0d8652ec, 0x1f9d45f1, 0x119448fa,
	        0x4be30393, 0x45ea0e98, 0x57f11985, 0x59f8148e,
	        0x73c737bf, 0x7dce3ab4, 0x6fd52da9, 0x61dc20a2,
	        0xad766df6, 0xa37f60fd, 0xb16477e0, 0xbf6d7aeb,
	        0x955259da, 0x9b5b54d1, 0x894043cc, 0x87494ec7,
	        0xdd3e05ae, 0xd33708a5, 0xc12c1fb8, 0xcf2512b3,
	        0xe51a3182, 0xeb133c89, 0xf9082b94, 0xf701269f,
	        0x4de6bd46, 0x43efb04d, 0x51f4a750, 0x5ffdaa5b,
	        0x75c2896a, 0x7bcb8461, 0x69d0937c, 0x67d99e77,
	        0x3daed51e, 0x33a7d815, 0x21bccf08, 0x2fb5c203,
	        0x058ae132, 0x0b83ec39, 0x1998fb24, 0x1791f62f,
	        0x764dd68d, 0x7844db86, 0x6a5fcc9b, 0x6456c190,
	        0x4e69e2a1, 0x4060efaa, 0x527bf8b7, 0x5c72f5bc,
	        0x0605bed5, 0x080cb3de, 0x1a17a4c3, 0x141ea9c8,
	        0x3e218af9, 0x302887f2, 0x223390ef, 0x2c3a9de4,
	        0x96dd063d, 0x98d40b36, 0x8acf1c2b, 0x84c61120,
	        0xaef93211, 0xa0f03f1a, 0xb2eb2807, 0xbce2250c,
	        0xe6956e65, 0xe89c636e, 0xfa877473, 0xf48e7978,
	        0xdeb15a49, 0xd0b85742, 0xc2a3405f, 0xccaa4d54,
	        0x41ecdaf7, 0x4fe5d7fc, 0x5dfec0e1, 0x53f7cdea,
	        0x79c8eedb, 0x77c1e3d0, 0x65daf4cd, 0x6bd3f9c6,
	        0x31a4b2af, 0x3fadbfa4, 0x2db6a8b9, 0x23bfa5b2,
	        0x09808683, 0x07898b88, 0x15929c95, 0x1b9b919e,
	        0xa17c0a47, 0xaf75074c, 0xbd6e1051, 0xb3671d5a,
	        0x99583e6b, 0x97513360, 0x854a247d, 0x8b432976,
	        0xd134621f, 0xdf3d6f14, 0xcd267809, 0xc32f7502,
	        0xe9105633, 0xe7195b38, 0xf5024c25, 0xfb0b412e,
	        0x9ad7618c, 0x94de6c87, 0x86c57b9a, 0x88cc7691,
	        0xa2f355a0, 0xacfa58ab, 0xbee14fb6, 0xb0e842bd,
	        0xea9f09d4, 0xe49604df, 0xf68d13c2, 0xf8841ec9,
	        0xd2bb3df8, 0xdcb230f3, 0xcea927ee, 0xc0a02ae5,
	        0x7a47b13c, 0x744ebc37, 0x6655ab2a, 0x685ca621,
	        0x42638510, 0x4c6a881b, 0x5e719f06, 0x5078920d,
	        0x0a0fd964, 0x0406d46f, 0x161dc372, 0x1814ce79,
	        0x322bed48, 0x3c22e043, 0x2e39f75e, 0x2030fa55,
	        0xec9ab701, 0xe293ba0a, 0xf088ad17, 0xfe81a01c,
	        0xd4be832d, 0xdab78e26, 0xc8ac993b, 0xc6a59430,
	        0x9cd2df59, 0x92dbd252, 0x80c0c54f, 0x8ec9c844,
	        0xa4f6eb75, 0xaaffe67e, 0xb8e4f163, 0xb6edfc68,
	        0x0c0a67b1, 0x02036aba, 0x10187da7, 0x1e1170ac,
	        0x342e539d, 0x3a275e96, 0x283c498b, 0x26354480,
	        0x7c420fe9, 0x724b02e2, 0x605015ff, 0x6e5918f4,
	        0x44663bc5, 0x4a6f36ce, 0x587421d3, 0x567d2cd8,
	        0x37a10c7a, 0x39a80171, 0x2bb3166c, 0x25ba1b67,
	        0x0f853856, 0x018c355d, 0x13972240, 0x1d9e2f4b,
	        0x47e96422, 0x49e06929, 0x5bfb7e34, 0x55f2733f,
	        0x7fcd500e, 0x71c45d05, 0x63df4a18, 0x6dd64713,
	        0xd731dcca, 0xd938d1c1, 0xcb23c6dc, 0xc52acbd7,
	        0xef15e8e6, 0xe11ce5ed, 0xf307f2f0, 0xfd0efffb,
	        0xa779b492, 0xa970b999, 0xbb6bae84, 0xb562a38f,
	        0x9f5d80be, 0x91548db5, 0x834f9aa8, 0x8d4697a3
        };
    }
}