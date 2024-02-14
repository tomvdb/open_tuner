using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.Utilities
{
    public abstract class DynamicPropertyInterface
    {
        public abstract string Key { get; set; }
        public abstract void UpdateValue(string Value);
    }
}
