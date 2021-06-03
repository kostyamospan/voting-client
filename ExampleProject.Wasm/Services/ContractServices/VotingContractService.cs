using Microsoft.AspNetCore.Components;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingClient.Services.ContractServices
{
    public class VotingContractService
    {
        public Web3 web3 { get; set; }

        public AbiService AbiService{ get; set; }

        public string ContractAddress { get; private set; }


        public VotingContractService(Web3 web3, AbiService abiService,string contractAddress)
        {
            this.web3 = web3;
            this.AbiService = abiService;
            this.ContractAddress = contractAddress;
        }
        public async Task EndVoting(int votingId, string selectedAccount)
        {
            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            await Web3HelperService.CreateTransactionAsync(web3, selectedAccount, ContractAddress, abi, "endVoting", votingId);
        }

        public async Task<string> GetWinnerProposal(int id)
        {
            string abi = await AbiService.GetAbiContractAsync(AbiService.VotingContractFileName);

            return await Web3HelperService.CallAsync<string>(web3, ContractAddress, abi, "getWinnerProposal", id);
        }

        public void ChangeWeb3Provider(Web3 web3)
        {
            this.web3 = web3;
        }
    }
}
