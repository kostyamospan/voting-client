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
        public string ContractAddress { get; set; }

        public List<string> Votings { get; set; }

        protected string AuthenticatedAccount { get; set; }

        protected int VotingsCount { get; set; }

        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; }
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AbiService AbiService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            StringValues initContract;

            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("contract", out initContract))
                ContractAddress = Convert.ToString(initContract);

            //     await GetVotings();

            base.OnInitialized();
        }


        public async Task GetVotings()
        {
            var web3 = new Web3("http://localhost:8545");

            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            /*var contract = web3.Eth.GetContract(abi, ContractAddress);

            var func = contract.GetFunction("getVotingsCount");

            var res = await func.CallAsync<int>();*/

            VotingsCount = await Web3HelperService.CallAsync<int>(web3, ContractAddress, abi, "getVotingsCount");
        }
    }
}
