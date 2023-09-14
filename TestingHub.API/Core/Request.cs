using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHub.API.Core
{

    public class Request
    {

        internal static HttpContent CreateBody(Dictionary<string, object> bodyJson)
        {
            if (bodyJson.Count == 0)
                return null;

            string serializedBody = JsonConvert.SerializeObject(bodyJson);
            return new StringContent(serializedBody, Encoding.UTF8, "application/json");
        }

        internal static string CreateQueryParameters(Dictionary<string, string> parameters)
        {
            var queryParams = new List<string>();

            foreach (var parameter in parameters)
            {
                queryParams.Add($"{parameter.Key}={Uri.EscapeDataString(parameter.Value)}");
            }

            return "?" + string.Join("&", queryParams);
        }
    }
}
