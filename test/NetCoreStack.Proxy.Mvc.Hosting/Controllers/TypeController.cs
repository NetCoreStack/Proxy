using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Proxy.Test.Contracts;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Mvc.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class TypeController : Controller
    {
        [HttpGet(nameof(GetComplexType))]
        public IEnumerable<string> GetComplexType(ComplexTypeModel model)
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost(nameof(PostComplexType))]
        public void PostComplexType([FromBody]ComplexTypeModel model)
        {
        }

        // POST api/values
        [HttpPost(nameof(PostComplextTypeWithFiles))]
        public void PostComplextTypeWithFiles(ComplextTypeModelWithFiles model)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
