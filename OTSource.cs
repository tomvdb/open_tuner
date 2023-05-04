using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public abstract class OTSource
    {
        public abstract void Initialize(CircularBuffer TSDataQueue);
    }
}
