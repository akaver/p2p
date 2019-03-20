using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Crypto;
using Newtonsoft.Json;

namespace Domain
{
    public class Block
    {
        public string BlockId { get; set; } // hash fom everything except child block id and LocalCreatedAt
        public DateTime LocalCreatedAt { get; set; } // local timestamp, when this record was created

        public string ParentBlockId { get; set; } // hash of parent
        [JsonIgnore] public Block ParentBlock { get; set; }

        [JsonIgnore] public string ChildBlockId { get; set; } // hash of parent
        [JsonIgnore] public Block ChildBlock { get; set; }


        /*
        public string ChildBlockId { get; set; } // hash of child - for easier navigation. maybe not needed. requires configuration in EF and double updates
        public Block ChildBlock { get; set; }
        */

        #region payload

        public DateTime CreatedAt { get; set; }
        public string Originator { get; set; }

        public string
            Content
        {
            get;
            set;
        } // bunch of meta-data can be extracted from here into separate fields - app id, contract id, ...

        #endregion


        #region payload signature

        public string Signature { get; set; }

        #endregion


        public string GetPayload => CreatedAt.ToLongDateString() + Originator + Content;

        public string GetContentForBlockHashing =>
            ParentBlockId +
            CreatedAt.ToLongDateString() +
            Originator +
            Content +
            Signature;
    }

    public static class BlockExtensions
    {
        public static string GetPayloadSignature(this Block block, string signatureKey)
        {
            var inputString =
                block.CreatedAt.ToLongDateString() +
                block.Originator +
                block.Content;
            
            var inputBytes = Encoding.UTF8.GetBytes(inputString);
            
            return SignatureProvider.GetSignature(inputBytes, signatureKey);
        }


        public static string GetHash(this Block block)
        {
            var bytesToHash = System.Text.Encoding.UTF8.GetBytes(block.GetContentForBlockHashing);
            var hasher = SHA256.Create();
            var hashBytes = hasher.ComputeHash(bytesToHash);

            return Convert.ToBase64String(hashBytes);
        }
    }
}