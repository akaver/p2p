using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ledger.Helpers
{
    public static class BlockHelper
    {
        public static async Task GenerateNewBlockAsync(AppDbContext dbContext, IAppLogger logger,
            LedgerOptions options)
        {
            // get the last block from db?
            var parentBlock = await dbContext.Blocks.SingleAsync(b => b.ChildBlockId == null);
            
            var childBlock = new Block();
            childBlock.ParentBlockId = parentBlock.BlockId;

            // payload
            childBlock.CreatedAt = DateTime.Now;
            childBlock.Originator = options.PublicKey;
            childBlock.Content = "CHILD BLOCK " + Guid.NewGuid().ToString();

            // payload signature
            childBlock.Signature = childBlock.GetPayloadSignature(options.PublicKey);

            childBlock.LocalCreatedAt = childBlock.CreatedAt;
            childBlock.BlockId = childBlock.GetHash();

            parentBlock.ChildBlockId = childBlock.BlockId;

            dbContext.Blocks.Add(childBlock);

            await dbContext.SaveChangesAsync();
            
        }
    }
}