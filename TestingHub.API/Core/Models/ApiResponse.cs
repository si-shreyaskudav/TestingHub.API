using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHub.API.Core.Models
{
    public class ApiResponse
    {
        public HTTPMeta Meta { get; set; }

        public JObject Data { get; set; }


        public void SuccessAsserts()
        {
            this.Meta.Success.Should().BeTrue();
            this.Meta.RetVal.Should().Be(1);
        }

        public void FailedAsserts()
        {
            this.Meta.Success.Should().BeFalse();
            this.Meta.RetVal.Should().NotBe(1);
        }

        public T ExtractFromData<T>(string propertyChain)
        {
            string[] propertyNames = propertyChain.Split('.');
            JToken token = this.Data;
            foreach (string propertyName in propertyNames)
            {
                if (token == null)
                {
                    throw new ArgumentNullException(nameof(this.Data), "The provided JSON data is null.");
                }

                token = token[propertyName];
            }

            if (token == null)
            {
                throw new ArgumentException($"Property chain '{propertyChain}' not found in the JSON data.");
            }

            try
            {
                return token.ToObject<T>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting property value to type '{typeof(T)}'.", ex);
            }
        }


    }

}
