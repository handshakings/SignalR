
using System.Collections.Generic;
using System.Web.Http;

namespace SignalRServerSelfhost.Controller
{
    public class ProductController : ApiController
    {
        public IEnumerable<string> Get() => new string[] { "value1", "value2" };
    }
}
