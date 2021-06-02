using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Models
{
    [FunctionOutput]
    public class Voting
    {
        [Parameter("uint")]
        public int VotingStatus { get; set; }

        [Parameter("uint256")]
        public long CreationTimestamp { get; set; }

        [Parameter("uint256")]
        public long Duration { get; set; }

        [Parameter("uint256")]
        public int ProposalsCount { get; set; }


        public string WinnerProposal { get; set; }
    }
}
