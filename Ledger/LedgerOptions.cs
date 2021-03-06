using System;

namespace Ledger
{
    public class LedgerOptions
    {
        public string Addr { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5000;
        public string Path { get; set; } = "/ledger";

        public string PrivateKey { get; set; } = Guid.NewGuid().ToString();
        public string PublicKey { get; set; } = Guid.NewGuid().ToString();
        
    }
}