using System;
using System.Threading.Tasks;
using RestSharp;

namespace iHubAPI.FunctionalTests
{
    public class RestUtilities
    {
        private readonly IRestClient _client;

        public RestUtilities(IRestClient restClient) => _client = restClient;

        public IRestResponse SendRequest(RestRequest request) => _client.Execute(request);

        public T Execute<T>(RestRequest request) where T : new()
        {
            try
            {
                request.AddHeader("Accept", "application/json");
                var response = _client.Execute<T>(request);
                if (response.ErrorException != null)
                {
                    const string message = "Error retrieving response.";
                    throw new ArgumentException(message, response.ErrorException);
                }
                return response.Data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            try
            {
                request.AddHeader("Accept", "application/json");
                var response = await _client.ExecuteTaskAsync<T>(request);
                if (response.ErrorException != null)
                {
                    const string message = "Error retrieving response.";
                    throw new ArgumentException(message, response.ErrorException);
                }
                return response.Data;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}