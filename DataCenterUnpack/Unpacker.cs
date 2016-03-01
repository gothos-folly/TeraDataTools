using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataCenterUnpack
{
    class Unpacker
    {
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string HashStream(Stream stream)
        {
            stream.Position = 0;
            using (var sha2 = SHA256.Create())
            {
                var hash = sha2.ComputeHash(stream);
                stream.Position = 0;

                var hexHash = BitConverter.ToString(hash).Replace("-", "");
                return hexHash;
            }

        }

        public static void Unpack(string inputFileName, string outputBase, byte[] key, byte[] iv)
        {
            var info = new List<string>();

            if (key.Length != 16)
                throw new ApplicationException("Invalid key length");
            if (iv.Length != 16)
                throw new ApplicationException("Invalid IV length");
            if (!File.Exists(inputFileName))
                throw new ApplicationException("Input file not found");


            info.Add("name " + Path.GetFileName(inputFileName));
            info.Add("key " + BitConverter.ToString(key).Replace("-", ""));
            info.Add("iv " + BitConverter.ToString(iv).Replace("-", ""));

            // decrypt
            var decryptedData = new MemoryStream();
            var aes = new RijndaelManaged();
            aes.Mode = CipherMode.CFB;
            aes.Key = key;
            aes.IV = iv;
            aes.Padding = PaddingMode.None;
            var decryptor = new NoPaddingTransformWrapper(aes.CreateDecryptor());

            using (var inputFile = File.OpenRead(inputFileName))
            {
                info.Add("original.size " + inputFile.Length);
                info.Add("original.sha256 " + HashStream(inputFile));
                using (var cryptoStream = new CryptoStream(inputFile, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(decryptedData);
                }
                info.Add("decrypted.size " + decryptedData.Length);
                info.Add("decrypted.sha256 " + HashStream(decryptedData));
            }

            var outputDecryptedName = Path.ChangeExtension(outputBase, "decrypted");
            File.WriteAllBytes(outputDecryptedName, decryptedData.ToArray());

            // First 4 bytes are unknown. The next 2 bytes are the zlib header 789C. After that the raw deflate data follows.
            var header = new byte[6];
            if (decryptedData.Read(header, 0, header.Length) != header.Length)
                throw new IOException("Did not read the full header");
            if (header[4] != 0x78 || header[5] != 0x9C)//zlib header
                throw new ApplicationException("Incorrect key/iv");

            decryptedData.Position = 6;

            int revision;

            // decompress using deflate
            var tempFileName = Path.Combine(Path.GetDirectoryName(outputBase), "TempDataCenter");
            using (var unpackedData = File.Create(tempFileName))
            {
                using (var deflateStream = new DeflateStream(decryptedData, CompressionMode.Decompress, true))
                {
                    deflateStream.CopyTo(unpackedData);
                }
                info.Add("unpacked.size " + unpackedData.Length);
                info.Add("unpacked.sha256 " + HashStream(unpackedData));

                using (var reader = new BinaryReader(unpackedData, Encoding.Unicode, true))
                {
                    unpackedData.Position = 0x0C;
                    revision = reader.ReadInt32();
                    info.Insert(1, "revision " + revision);
                }
            }
            File.WriteAllLines(Path.ChangeExtension(outputBase, revision + ".dcinfo"), info);
            File.Copy(tempFileName,Path.ChangeExtension(outputBase, revision + ".unpacked"),true);
            File.Delete(tempFileName);
            File.Delete(outputDecryptedName);
        }

    }
}
