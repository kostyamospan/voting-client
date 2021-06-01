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

        public string VotingContractFileName => "";

        public async Task<string> GetAbiContractAsync(string abiFileName)
        {
            if (_votingContractAbi != null) return _votingContractAbi;

            using (var httpClient = new HttpClient())
                _votingContractAbi = JsonConvert.SerializeObject(JObject.Parse(await httpClient.GetStringAsync(_baseUrl + "/VoterERC20.json")).GetValue("abi"));

            return _votingContractAbi;
        }


        private string _baseUrl;

        private string _votingContractAbi;
    }
}
