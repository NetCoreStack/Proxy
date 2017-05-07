using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    /// <summary>
    /// AgentCheck represents a check known to the agent
    /// </summary>
    public class AgentCheck
    {
        public string Node { get; set; }
        public string CheckID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Output { get; set; }
        public string ServiceID { get; set; }
        public string ServiceName { get; set; }
    }

    /// <summary>
    /// AgentMember represents a cluster member known to the agent
    /// </summary>
    public class AgentMember
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public ushort Port { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public int Status { get; set; }
        public byte ProtocolMin { get; set; }
        public byte ProtocolMax { get; set; }
        public byte ProtocolCur { get; set; }
        public byte DelegateMin { get; set; }
        public byte DelegateMax { get; set; }
        public byte DelegateCur { get; set; }

        public AgentMember()
        {
            Tags = new Dictionary<string, string>();
        }
    }
}
