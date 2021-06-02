using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingClient.Models
{
    [FunctionOutput]
    public class GetProposalInfoResponse
    {
        [Parameter("string", "")]
        public string Proposal { get; set; }
        [Parameter("uint256", "")]
        public long VotingCount { get; set; }
    }
}
