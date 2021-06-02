using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExampleProject.Wasm.Services
{
    public class AbiService
    {

        public AbiService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string VotingContractFileName => "VoterERC20.json";

        public string IERC20ContractFileName => "IERC20.json";


        public async Task<string> GetAbiContractAsync(string abiFileName)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

                return  JsonConvert.SerializeObject(JObject.Parse(await httpClient.GetStringAsync($"{_baseUrl}/{abiFileName}")).GetValue("abi"));

            }
        }

        private string _baseUrl;
    }
}
