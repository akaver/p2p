using System;
using System.Security.Cryptography;
using Crypto.Entities;
using Crypto.Helpers;

namespace Crypto
{
    public static class KeyGenerator
    {
        public static KeyPair GetPrivatePublicKeyPair()
        {
            // Random public/private key pair is generated when a new instance of the class is created.
            var rsa = new RSACryptoServiceProvider();
            
            var publicKey = rsa.ToJson(false);
            var privateKey = rsa.ToJson(true);
            
            return new KeyPair
            {
                PublicKey = publicKey,
                PrivateKey = privateKey
            };
        }
    }
}