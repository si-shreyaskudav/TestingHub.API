using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestingHub.API.Core.Models;

namespace TestingHub.API.Core
{

    public static class HttpClientExtensions
    {
        public static async Task<ApiResponse> CallAsync(this HttpClient client, TestCase testCase)
        {
            HttpResponseMessage httpResponseMessage = null;

            if (testCase.Method.ToUpper() == "POST")
                httpResponseMessage = await client.PostAsync($"{testCase.ApiUrl}{Request.CreateQueryParameters(testCase.QueryParams)}", Request.CreateBody(testCase.BodyJson));

            else if (testCase.Method.ToUpper() == "GET")
                httpResponseMessage = await client.GetAsync($"{testCase.ApiUrl}{Request.CreateQueryParameters(testCase.QueryParams)}");

            if (httpResponseMessage.StatusCode.Equals(HttpStatusCode.MethodNotAllowed))
                httpResponseMessage.StatusCode.Should().NotBe(HttpStatusCode.MethodNotAllowed);

            return await Common.ParseHttpResponse(httpResponseMessage);
        }
    }
}
