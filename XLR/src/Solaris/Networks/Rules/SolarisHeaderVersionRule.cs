using Blockcore.Consensus;
using Blockcore.Consensus.Rules;
using Blockcore.Features.Consensus.Rules.CommonRules;
using Blockcore.Utilities;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace Solaris.Networks.Rules
{
   /// <summary>
   /// Checks if <see cref="SolarisMain"/> network block's header has a valid block version.
   /// </summary>
   public class SolarisHeaderVersionRule : HeaderVersionRule
   {
      /// <inheritdoc />
      /// <exception cref="ConsensusErrors.BadVersion">Thrown if block's version is outdated or otherwise invalid.</exception>
      public override void Run(RuleContext context)
      {
         Guard.NotNull(context.ValidationContext.ChainedHeaderToValidate, nameof(context.ValidationContext.ChainedHeaderToValidate));

         ChainedHeader chainedHeader = context.ValidationContext.ChainedHeaderToValidate;

         // A version of precisely 7 is what is currently generated by the legacy C++ nodes.

         // The stratisX block validation rules mandate if (!IsProtocolV3(nTime)) && (nVersion > 7), then reject block.
         // Further, if (IsProtocolV2(nHeight) && nVersion < 7), then reject block.
         // And lastly, if (!IsProtocolV2(nHeight) && nVersion > 6), then reject block.

         // Protocol version determination is based on either the block height or timestamp as shown:
         // IsProtocolV2(nHeight) { return TestNet() || nHeight > 0; }
         // IsProtocolV3(nTime) { return TestNet() || nTime > 1470467000; }

         // The mainnet genesis block has nTime = 1470713393, so V3 is applied immediately and this supersedes V2.
         // The block versions have therefore been version 7 since genesis on Stratis mainnet.

         // Whereas BIP9 mandates that the top bits of version be 001. So a standard node should never generate
         // block versions above 7 and below 0x60008000.

         // The acceptable common subset of the rules is therefore that the block version must be >= 7.

         if (chainedHeader.Header.Version < 7)
         {
            Logger.LogTrace("(-)[BAD_VERSION]");

            ConsensusErrors.BadVersion.Throw();
         }
      }
   }
}
