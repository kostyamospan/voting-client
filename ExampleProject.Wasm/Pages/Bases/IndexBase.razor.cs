using ExampleProject.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
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
using static System.Net.WebRequestMethods;

namespace ExampleProject.Wasm.Pages
{
    public class IndexBase : ComponentBase
    {
        public string ContractAddress { get; set; }

        public string SelectedAccount { get; private set; }


        protected string AuthenticatedAccount { get; set; }

        protected int VotingsCount { get; set; }

        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AbiService AbiService { get; set; }
        [Inject] IJSRuntime JSr { get; set; }

        protected async Task OnCreateBtnClick()
        {
            await this.CreateNewVoting();
        }

        protected async Task OnGetVotingsBtnClick()
        {
            await this.LoadAllVotings();
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

            Console.WriteLine(propPrompted);
        }


        private async Task LoadAllVotings()
        {
            var web3 = new Web3("http://localhost:8545");

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            VotingsCount = await Web3HelperService.CallAsync<int>(web3, ContractAddress, abi, "getVotingsCount");
        }
    }
}
