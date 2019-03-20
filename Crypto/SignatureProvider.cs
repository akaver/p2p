using System;
using System.Security.Cryptography;
using Crypto.Helpers;

namespace Crypto
{
    public static class SignatureProvider
    {
        /// <summary>
        /// Gets RSA signature of the input data
        /// </summary>
        /// <param name="data">The data as byte array</param>
        /// <param name="privateKey">RSA private key as XML string</param>
        /// <returns>Signature as base64 string</returns>
        public static string GetSignature(byte[] data, string privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromJson(privateKey);

            var signature = rsa.SignData(data, new SHA1CryptoServiceProvider());

            return Convert.ToBase64String(signature);
        }

        public static bool VerifySignature(byte[] plainText, byte[] signature, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromJson(publicKey);

            return rsa.VerifyData(plainText, new SHA1CryptoServiceProvider(), signature);
        }
        
    }
}