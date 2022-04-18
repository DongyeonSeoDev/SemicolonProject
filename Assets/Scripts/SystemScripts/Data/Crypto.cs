using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;

public static class Crypto 
{
    public static string Encrypt(string text, string key)
    {
        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;

        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if(len>keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);
        rijndael.Key = keyBytes;
        rijndael.IV = keyBytes;
        ICryptoTransform transform = rijndael.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    public static string Decrypt(string text, string key)
    {
        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;

        rijndael.KeySize = 128;
        rijndael.BlockSize = 128;
        byte[] encryptedData = Convert.FromBase64String(text);
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if(len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);
        rijndael.Key = keyBytes;
        rijndael.IV = keyBytes;
        byte[] plainText = rijndael.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(plainText);
    }
}
