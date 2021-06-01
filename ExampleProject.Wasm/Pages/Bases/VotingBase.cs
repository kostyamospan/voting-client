using ExampleProject.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Nethereum.UI;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Pages.Bases
{

    public class VotingBase : ComponentBase
    {
        public int VotingId { get; set; }

        public string ContractAddress { get; set; }

        public List<string> Proposals { get; set; }

        public string SelectedAccount { get; private set; }

        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AbiService AbiService { get; set; }


        public async Task OnVoteClick(string prop)
        {
            await this.Vote(prop);
        }


        private async Task OnSelectedAccountChanged(string arg)
        {
            SelectedAccount = arg;
        }

        public async Task LoadAllProposalsAsync()
        {
            var web3 = new Web3("http://localhost:8545");

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            Proposals = await Web3HelperService.CallAsync<List<string>>(web3, ContractAddress, abi, "getAllProposals", VotingId);
        }


        private async Task Vote(string prop)
        {
            var web3 = new Web3("http://localhost:8545");

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            await Web3HelperService.CreateTransactionAsync(
                web3,
                SelectedAccount,
                ContractAddress,
                abi,
                "vote",
                VotingId,
                new Nethereum.ABI.Encoders.Bytes32TypeEncoder().Encode(prop));
        }

        protected override async Task OnInitializedAsync()
        {
            _ethereumHostProvider.SelectedAccountChanged += OnSelectedAccountChanged;


            StringValues initId;
            StringValues initContract;


            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out initId))
                VotingId = Convert.ToInt32(initId);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("contract", out initContract))
                ContractAddress = Convert.ToString(initContract);

            await this.LoadAllProposalsAsync();

            base.OnInitialized();
        }
    }
}
