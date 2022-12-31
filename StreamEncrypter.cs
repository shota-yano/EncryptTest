using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace EncryptTest
{
    class StreamEncrypter
    {
        // コンストラクター
        public StreamEncrypter(byte[] key = null, byte[] iv = null)
        {
            using (var aes = new AesManaged())
            {
                Key = key;
                if (key == null)
                {
                    aes.GenerateKey();
                    Key = aes.Key;
                }
                IV = iv;
                if (iv == null)
                {
                    aes.GenerateIV();
                    IV = aes.IV;
                }
            }
        }
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        // 暗号化する
        public void Encrypt(Stream inStream, Stream outStream)
        {
            using (var rmCrypto = new AesManaged())
            {
                rmCrypto.BlockSize = 128;
                rmCrypto.KeySize = 128;

                var encryptor = rmCrypto.CreateEncryptor(Key, IV);
                using (var cryptoStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                {
                    // 暗号化されてcryptoStreamにデータが書き込まれる
                    inStream.CopyTo(cryptoStream);


                }
            }
        }
        // 復号する
        public void Decrypt(Stream inStream, Stream outStream , byte[] inIV)
        {
            using (var rmCrypto = new AesManaged())
            {
                rmCrypto.BlockSize = 128;
                rmCrypto.KeySize = 128;

                //var decryptor = rmCrypto.CreateDecryptor(Key, IV);
                var decryptor = rmCrypto.CreateDecryptor(Key, inIV);
                using (var cryptoStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                {
                    // 復号されてoutStreamにデータが書き込まれる
                    //cryptoStream.CopyTo(outStream);
                    byte[] buffer = new byte[8192];
                    int len = 0;
                    while ((len = cryptoStream.Read(buffer, 0, 8192)) > 0)
                    {
                        outStream.Write(buffer, 0, len);
                    }
                }
            }
        }

    }
}
