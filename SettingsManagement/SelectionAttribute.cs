using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.SettingsManagement
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SelectionAttribute : Attribute
    {

        public SelectionAttribute(string[] options)
        {
            
        }
    }
}
