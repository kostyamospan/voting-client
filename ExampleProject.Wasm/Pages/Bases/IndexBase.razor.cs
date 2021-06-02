using ExampleProject.Wasm.Models;
using ExampleProject.Wasm.Models.StateModels;
using ExampleProject.Wasm.Services;
using ExampleProject.Wasm.Services.ContractServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.UI;
using Nethereum.Web3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Pages
{

    public class IndexBase : ComponentBase
    {
        public string ContractAddress { get; set; }

        public string SelectedAccount { get; private set; }

        public bool IsLoadingIndicatorVisible { get; set; }

        protected string AuthenticatedAccount { get; set; }

        protected List<Models.Voting> Votings { get; set; }

        [Inject] public NavigationManager NavManager { get; set; }
        [Inject] public GlobalState State { get; set; }
        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] AbiService AbiService { get; set; }
        [Inject] IJSRuntime JSr { get; set; }

        private Web3 web3;


        public IndexBase()
        {
            web3 = new Web3("http://localhost:8545");
        }

        protected async Task OnCreateBtnClick()
        {
            await this.CreateNewVoting();
        }

        protected async Task OnGetVotingsBtnClick()
        {
            await this.LoadAllVotings();
        }

        protected async Task OnEndVotingsBtnClick(int id)
        {
            var s = new VotingContractService(await _ethereumHostProvider.GetWeb3Async(), AbiService);

            await s.EndVoting(id, SelectedAccount, ContractAddress);

            Votings[id].VotingStatus = (int)VotingStatus.Finished;

            this.StateHasChanged();
        }





        protected override async Task OnInitializedAsync()
        {
            _ethereumHostProvider.SelectedAccountChanged += OnSelectedAccountChanged;


            StringValues initContract;

            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("contract", out initContract))
                ContractAddress = Convert.ToString(initContract);

            base.OnInitialized();
        }

        private async Task OnSelectedAccountChanged(string arg)
        {
            SelectedAccount = arg;
        }

        private async Task CreateNewVoting()
        {
            string propPrompted = await JSr.InvokeAsync<string>("prompt", "Enter proposals, delimited by coma:");
            string durPromted = await JSr.InvokeAsync<string>("prompt", "Enter voting duration in seconds:");


            var proposals = propPrompted.Split(",").Select(t =>
                new Nethereum.ABI.Encoders.Bytes32TypeEncoder().Encode(t.Trim()));

            var duration = Convert.ToInt32(durPromted);


            var web3 = await _ethereumHostProvider.GetWeb3Async();

            await Web3HelperService.CreateTransactionAsync(
                web3,
                SelectedAccount,
                ContractAddress,
                await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName),
                "createVoting",
                proposals,
                duration);

            Votings.Add(new()
            {
                CreationTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Duration = duration,
                VotingStatus = 0,
                ProposalsCount = proposals.Count()
            });
        }


        private async Task LoadAllVotings()
        {
            IsLoadingIndicatorVisible = true;

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            Votings = await Web3HelperService.CallAsync<List<Models.Voting>>(web3, ContractAddress, abi, "getAllVotings");

            this.StateHasChanged();

            IsLoadingIndicatorVisible = false;

            var s = new VotingContractService(web3, AbiService);

            for (int i = 0; i < Votings.Count; i++)
            {
                if (Votings[i].VotingStatus != (int)VotingStatus.Finished) continue;

                Votings[i].WinnerProposal = await s.GetWinnerProposal(i, ContractAddress);
                this.StateHasChanged();
            }
        }
    }
}
