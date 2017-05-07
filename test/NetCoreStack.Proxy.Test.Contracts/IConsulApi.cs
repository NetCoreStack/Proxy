using NetCoreStack.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreStack.Proxy.Test.Contracts
{
    /// <summary>
    /// Consul API Interface [https://www.consul.io/]
    /// </summary>
    [ApiRoute("v1", regionKey: "Consul")]
    public interface IConsulApi : IApiContract
    {
        /// <summary>
        /// Sample remote API interface method definiton. Not implemented (Remote)
        /// </summary>
        /// <returns></returns>
        [HttpGetMarker(Template = "agent/members")]
        Task<IEnumerable<AgentMember>> GetMembersAsync();

        [HttpGetMarker(Template = "health/node/{id}")]
        Task<IEnumerable<AgentCheck>> CheckNode(string node);
    }
}
