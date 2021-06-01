using Microsoft.AspNetCore.Components;
using Nethereum.Hex.HexTypes;
using Nethereum.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Pages
{
    public partial class Index : ComponentBase
    {
        bool EthereumAvailable { get; set; }
        string SelectedAccount { get; set; }
        string BlockHash { get; set; }
        string TransactionHash { get; set; }
        string ErrorTransferMessage { get; set; }
        protected string AuthenticatedAccount { get; set; }

        [Inject] IEthereumHostProvider _ethereumHostProvider { get; set; }
        [Inject] NethereumAuthenticator _nethereumAuthenticator { get; set; } 




        protected override async Task OnInitializedAsync()
        {
            _ethereumHostProvider.SelectedAccountChanged += HostProvider_SelectedAccountChanged;
            EthereumAvailable = await _ethereumHostProvider.CheckProviderAvailabilityAsync();
        }


        private async Task HostProvider_SelectedAccountChanged(string account)
        {
            SelectedAccount = account;
            this.StateHasChanged();
        }


        protected async Task GetBlockHashAsync()
        {
            var web3 = await _ethereumHostProvider.GetWeb3Async();
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(1));
            BlockHash = block.BlockHash;
        }

        protected async Task TransferEtherAsync()
        {
            try
            {
                var web3 = await _ethereumHostProvider.GetWeb3Async();

                TransactionHash = await web3.Eth.GetEtherTransferService().TransferEtherAsync("0x13f022d72158410433cbd66f5dd8bf6d2d129924", 0.001m);
            }
            catch (Exception ex)
            {
                ErrorTransferMessage = ex.Message;
            }
        }

        public async Task AuthenticateAsync()
        {

            AuthenticatedAccount = await _nethereumAuthenticator.RequestNewChallengeSignatureAndRecoverAccountAsync();

        }
    }
}
