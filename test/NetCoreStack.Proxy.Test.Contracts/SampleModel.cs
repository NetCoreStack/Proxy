using System;
using System.Collections.Generic;

namespace NetCoreStack.Proxy.Test.Contracts
{
    public class SampleModel
    {
        public string String { get; set; }
        public int Number { get; set; }
        public bool Boolean { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<int> Enumerable { get; set; }
        public DateTime Date { get; set; }
    }
}
