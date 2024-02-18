using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Winterhill
{
    public partial class WinterhillSettingsForm : Form
    {
        private WinterhillSettings _settings;

        public WinterhillSettingsForm(WinterhillSettings Settings)
        {
            InitializeComponent();

            _settings = Settings;
        }
    }
}
