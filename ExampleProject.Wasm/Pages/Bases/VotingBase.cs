using ExampleProject.Wasm.Models;
using ExampleProject.Wasm.Models.StateModels;
using ExampleProject.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Nethereum.ABI.FunctionEncoding.Attributes;
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

        public bool IsVoted { get; set; }

        public int VotingId { get; set; }

        public string ContractAddress { get; set; }

        public List<GetProposalInfoResponse> Proposals { get; set; }

        public string SelectedAccount { get; private set; }

        [Inject] public GlobalState GlobalState { get; set; }
        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AbiService AbiService { get; set; }


        private Web3 web3;

        public VotingBase()
        {
            web3 = new Web3("http://localhost:8545");
        }

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


            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            Proposals = await Web3HelperService.CallAsync<List<GetProposalInfoResponse>>(web3, ContractAddress, abi, "getAllProposals", VotingId);

            IsVoted = await this.CheckIsVoted();

            this.StateHasChanged();
        }


        private async Task<bool> CheckIsVoted()
        {

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            var res = await Web3HelperService.CallAsync<bool>(web3, ContractAddress, abi, "isVoted", VotingId);

            return res;
        }


        private async Task Vote(string prop)
        {
            Console.WriteLine(prop);

            var web3 = await _ethereumHostProvider.GetWeb3Async();

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            await Web3HelperService.CreateTransactionAsync(
                web3,
                SelectedAccount,
                ContractAddress,
                abi,
                "vote",
                VotingId,
                prop);

            Proposals.FirstOrDefault(x => x.Proposal == prop).VotingCount += await GetBalance();

            this.StateHasChanged();
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


        private async Task<long> GetBalance()
        {
            string abi = await AbiService.GetAbiContractAsync(AbiService.IERC20ContractFileName);

            Console.WriteLine(SelectedAccount);

            var res = await Web3HelperService.CallAsync<long>(web3, ContractAddress, abi, "balanceOf", SelectedAccount);

            return res;
        }
    }
}
