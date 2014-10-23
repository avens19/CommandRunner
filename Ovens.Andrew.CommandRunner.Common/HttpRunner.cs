using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace Ovens.Andrew.CommandRunner.Common
{
    public static class HttpRunner
    {
        private static Dictionary<string, string> _cookies;
        private static TaskCompletionSource<string> _tcs;

        public static async Task<string> Execute(RestRequest request, string baseUrl, bool clearCookies)
        {
            _tcs = new TaskCompletionSource<string>();

            if (clearCookies || _cookies == null)
            {
                _cookies = new Dictionary<string, string>();
            }

            request.Timeout = 5000;

            foreach (var item in _cookies)
            {
                request.AddParameter(item.Key, item.Value, ParameterType.Cookie);
            }

            var client = new RestClient(baseUrl);

            client.ExecuteAsync(request, response =>
            {
                if (response.ErrorException != null)
                {
                    _tcs.SetException(response.ErrorException);
                    return;
                }

                foreach (var item in response.Cookies)
                {
                    _cookies.Add(item.Name, item.Value);
                }

                _tcs.SetResult(response.Content);
            });

            return await _tcs.Task;
        }
    }
}