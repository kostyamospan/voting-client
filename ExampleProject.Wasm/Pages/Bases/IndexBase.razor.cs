using ExampleProject.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
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
        bool EthereumAvailable { get; set; }
        string SelectedAccount { get; set; }

        public string ContractAddress { get; set; }

        public string[] Votings { get; set; }

        protected string AuthenticatedAccount { get; set; }

        protected int VotingsCount { get; set; }

        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AbiService AbiService { get; set; }







        protected override async Task OnInitializedAsync()
        {
            _ethereumHostProvider.SelectedAccountChanged += HostProvider_SelectedAccountChanged;
            EthereumAvailable = await _ethereumHostProvider.CheckProviderAvailabilityAsync();

            StringValues initContract;

            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("contract", out initContract))
                ContractAddress = Convert.ToString(initContract);

            //     await GetVotings();

            base.OnInitialized();
        }

        private async Task HostProvider_SelectedAccountChanged(string account)
        {
            SelectedAccount = account;
            this.StateHasChanged();
        }


        public async Task GetVotings()
        {
            var web3 = new Web3("http://localhost:8545");

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            var contract = web3.Eth.GetContract(abi, ContractAddress);

            var func = contract.GetFunction("getVotingsCount");

            var res = await func.CallAsync<int>();

            VotingsCount = res;

            Console.WriteLine(res);
        }

    }
}
