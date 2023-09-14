using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHub.API.Core.Models
{
    public class TestCase
    {
        public Dictionary<string, string> QueryParams { get; set; }
        public Dictionary<string, object> BodyJson { get; set; }
        public string ApiUrl { get; set; }
        public string Method { get; set; }


    }
}
