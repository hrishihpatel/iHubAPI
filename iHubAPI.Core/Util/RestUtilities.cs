using System;
using System.Threading.Tasks;
using RestSharp;

namespace iHubAPI
{
    /// <summary>
    /// RestUtility Class for ease of executing HTTP request.
    /// </summary>
    public class RestUtilities
    {
        private readonly IRestClient _client;

        public RestUtilities(IRestClient restClient) => _client = restClient;

        public IRestResponse SendRequest(RestRequest request) => _client.Execute(request);

        /// <summary>
        /// Execute HTTP Request using RestSharp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Execute HTTP Request Async using RestSharp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
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