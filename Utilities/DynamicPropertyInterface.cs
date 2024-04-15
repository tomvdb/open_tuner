using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.Utilities
{
    public abstract class DynamicPropertyInterface
    {
        public abstract string Key { get; set; }
        public abstract string LastValue { get;}
        public abstract void UpdateValue(string Value);
        public abstract void UpdateColor(Color Col);
        public abstract void UpdateMuteButtonColor(Color Col);
    }
}
