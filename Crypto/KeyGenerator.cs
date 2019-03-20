using System;
using System.Security.Cryptography;
using Crypto.Entities;

namespace Crypto
{
    public static class KeyGenerator
    {
        public static KeyPair GetPrivatePublicKeyPair()
        {
            // Random public/private key pair is generated when a new instance of the class is created.
            var rsa = new RSACryptoServiceProvider();
            var rsaKeyInfo = rsa.ExportParameters(true);

            var publicKeyBytes = rsaKeyInfo.Exponent;
            var privateKeyBytes = rsaKeyInfo.D;
            
            return new KeyPair
            {
                PublicKey = Convert.ToBase64String(publicKeyBytes),
                PrivateKey = Convert.ToBase64String(privateKeyBytes)
            };
        }

        public static bool IsValidPublicKey(string keyCandidate)
        {
            return keyCandidate.Length > 50;
        }
        
        public static bool IsValidPrivateKey(string keyCandidate)
        {
            return keyCandidate.Length > 50;
        }
    }
}