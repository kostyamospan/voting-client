using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Services
{
    public static class Web3HelperService
    {

        public static async Task<T> CallAsync<T>(Web3 web3, string contractAddress, string abi, string methodName, params object[] args)
        {
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var func = contract.GetFunction(methodName);
            return await func.CallAsync<T>(args);
        }
    } 
}
