using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Serilog;

namespace opentuner.SettingsManagement
{
    public class SettingsFormBuilder
    {
        private readonly Type _type;
        private readonly object _instance;

        public SettingsFormBuilder(object instance)
        {
            _type = instance.GetType();
            _instance = instance;

            // Get all public fields of the class
            FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var group_attrib = (GroupAttribute)Attribute.GetCustomAttribute(field, typeof(GroupAttribute));

                Log.Information($"Field : {field.Name} {field.FieldType} { (group_attrib != null ? group_attrib.GroupName : "Misc") }");
            }
        
        }
        /*
        // Method to print field information
        public void PrintFieldInfo()
        {
            // Get all public fields of the class
            FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // Print the name and type of each public field
            foreach (var field in fields)
            {
                Console.WriteLine($"Field Name: {field.Name}");
                Console.WriteLine($"Field Type: {field.FieldType}");
                Console.WriteLine();
            }
        }

        // Method to get field values
        public void PrintFieldValues()
        {
            // Get all public fields of the class
            FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            // Print the name and value of each public field
            foreach (var field in fields)
            {
                object value = field.GetValue(_instance);
                Console.WriteLine($"Field Name: {field.Name}");
                Console.WriteLine($"Field Value: {value}");
                Console.WriteLine();
            }
        }
        */
    }

}
