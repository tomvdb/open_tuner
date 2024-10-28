using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.SettingsManagement
{

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class GroupAttribute : Attribute
    {
        public string GroupName { get; }

        public GroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
