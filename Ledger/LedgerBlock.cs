using System;

namespace Ledger
{
        public class LedgerBlock
        {
            public string BlockId { get; set; }
            public DateTime LocalCreatedAt { get; set; }
            public string ParentBlockId { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Originator { get; set; }
            public string Content { get; set; }
            public string Signature { get; set; }
        }
}