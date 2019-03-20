using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Crypto;
using Newtonsoft.Json;

namespace Domain
{
    public class Block
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string BlockId { get; set; } // hash fom everything except child block id and LocalCreatedAt
        public DateTime LocalCreatedAt { get; set; } // local timestamp, when this record was created

        public string ParentBlockId { get; set; } // hash of parent
        
        [JsonIgnore]
        public Block ParentBlock { get; set; }

        // hash of parent

        [JsonIgnore]
        public string ChildBlockId { get; set; } 
        
        [JsonIgnore]
        public Block ChildBlock { get; set; }


        #region payload

        public DateTime CreatedAt { get; set; }
        public string Originator { get; set; }

        // bunch of meta-data can be extracted from here into separate fields - app id, contract id, ...
        public string Content {get; set;} 

        #endregion


        #region payload signature

        public string Signature { get; set; }

        #endregion

        [JsonIgnore]
        public string GetPayload => 
            CreatedAt.ToLongDateString() +
            Originator + Content;

        [JsonIgnore]
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
            var inputBytes = Encoding.UTF8.GetBytes(block.GetPayload);
            
            return SignatureProvider.GetSignature(inputBytes, signatureKey);
        }


        public static string GetHash(this Block block)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(block.GetContentForBlockHashing);
            var hasher = SHA256.Create();
            var hashBytes = hasher.ComputeHash(bytesToHash);

            return Convert.ToBase64String(hashBytes);
        }
        
        public static string ToJson(this Block block)
        {
            var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            return JsonConvert.SerializeObject(block, settings);
        }
        
    }
}