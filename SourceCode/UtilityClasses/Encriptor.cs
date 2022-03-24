using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Encriptor
{
    //static byte[] key = {
    //                0x01, 0x04, 0x03, 0x08, 0x05, 0x12, 0x07, 0x16,
    //                0x09, 0x20, 0x11, 024, 0x13, 0x28, 0x15, 0x32
    //            };
    public static bool EncryptDataToFile(string path, string message)
    {
        try
        {
            byte[] key = {
                    0x01, 0x04, 0x03, 0x08, 0x05, 0x12, 0x07, 0x16,
                    0x09, 0x20, 0x11, 024, 0x13, 0x28, 0x15, 0x32
                };
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;

                    byte[] IV = aes.IV;
                    fileStream.Write(IV, 0, IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                        {
                            encryptWriter.WriteLine(message);
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool DecryptDataFromFile(string path, out string decryptedMessage)
    {
        try
        {
            byte[] key = {
                    0x01, 0x04, 0x03, 0x08, 0x05, 0x12, 0x07, 0x16,
                    0x09, 0x20, 0x11, 024, 0x13, 0x28, 0x15, 0x32
                };
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] iv = new byte[aes.IV.Length];
                    int numBytesToRead = aes.IV.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                        if (n == 0) break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader decryptReader = new StreamReader(cryptoStream))
                        {
                            decryptedMessage = decryptReader.ReadToEnd();
                        }
                    }
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            decryptedMessage = "";
            return false;
        }
    }
}
