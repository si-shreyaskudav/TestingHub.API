using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHub.API.Core.Models
{

    public class HTTPResponse
    {
        public HTTPResponse()
        {
            this.Meta = new HTTPMeta();
        }

        public Object Data { get; set; }
        public HTTPMeta Meta { get; set; }
    }

    public class HTTPMeta
    {
        public String Message { get; set; }
        public Int64 RetVal { get; set; }
        public bool Success { get; set; }
        public FeedTime Timestamp { get; set; }
    }
    public class FeedTime
    {
        public String UTCtime { get; set; }
        public String ISTtime { get; set; }
    }
}
