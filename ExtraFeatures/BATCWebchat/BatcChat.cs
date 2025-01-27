using opentuner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using opentuner.MediaSources;
using Serilog;
using System.Windows.Forms;
using opentuner.ExtraFeatures.BATCSpectrum;

namespace opentuner.ExtraFeatures.BATCWebchat
{
    public class BATCChat
    {
        private WebChatForm _form;
        private WebChatSettings wc_settings;
        SettingsManager<WebChatSettings> wc_settingsManager;
        public BATCChat(OTSource Source) {
            bool reposition = false;
            Size MainScreen_Size = new Size(new Point(640, 480));

            // webchat settings
            wc_settings = new WebChatSettings();
            wc_settingsManager = new SettingsManager<WebChatSettings>("qo100_webchat_settings");
            wc_settings = (wc_settingsManager.LoadSettings(wc_settings));

            _form = new WebChatForm(wc_settings, Source);
            _form.FormClosing += _form_FormClosing;
            _form.Resize += _form_Resize;
            _form.LocationChanged += _form_LocationChanged;

            Log.Information("Restoring BATCWebChat Window Position:");
            Log.Information(" Size: (h = " + wc_settings.gui_chat_height.ToString() + ", w = " + wc_settings.gui_chat_width.ToString() + ")");
            Log.Information(" Position: (x = " + wc_settings.gui_chat_x.ToString() + ", y = " + wc_settings.gui_chat_y.ToString() + ")");

            if (wc_settings.gui_chat_width > 0)
            {
                _form.Size = new Size(wc_settings.gui_chat_width, wc_settings.gui_chat_height);
            }
            _form.Location = new Point(wc_settings.gui_chat_x, wc_settings.gui_chat_y);

            //first: create a virtual screen area as a combination of all screens
            Rectangle vScreenRect = new Rectangle(0, 0, 0, 0);
            int vScreenRight = 0;
            int vScreenBottom = 0;
            Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            foreach (Screen s in screens)
            {
                Log.Information(s.ToString());
                if (s.WorkingArea.Top == 0)
                    MainScreen_Size = s.WorkingArea.Size;
                if (s.WorkingArea.Top < vScreenRect.Top)
                    vScreenRect.Y = s.WorkingArea.Top;
                if (s.WorkingArea.Left < vScreenRect.Left)
                    vScreenRect.X = s.WorkingArea.Left;
                if (s.WorkingArea.Bottom > vScreenBottom)
                    vScreenBottom = s.WorkingArea.Bottom;
                if (s.WorkingArea.Right > vScreenRight)
                    vScreenRight = s.WorkingArea.Right;

            }
            vScreenRect.Height = vScreenBottom - vScreenRect.Top;
            vScreenRect.Width = vScreenRight - vScreenRect.Left;

            //second: if necessary align the gui window to fit into the virual screen area
            if (_form.Top < vScreenRect.Top)
                reposition = true;
            if (_form.Top > vScreenBottom)
                reposition = true;
            if (_form.Left < vScreenRect.Left)
                reposition = true;
            if (_form.Left > vScreenRight)
                reposition = true;
            if (_form.Right > vScreenRight)
                reposition = true;

            if (reposition)
            {
                _form.Top = 20;
                _form.Left = 20;
                _form.Height = MainScreen_Size.Height / 2;
                _form.Width = MainScreen_Size.Width / 2;
                _form.WindowState = FormWindowState.Normal;
            }
            else
            {
                _form.WindowState = (FormWindowState)wc_settings.gui_chat_windowstate;
            }

            Log.Information("Aligned BATCWebChat Window Position:");
            Log.Information(" Size: (h = " + _form.Height.ToString() + ", w = " + _form.Width.ToString() + ")");
            Log.Information(" Position: (x = " + _form.Left.ToString() + ", y = " + _form.Top.ToString() + ")");

        }

        private void _form_LocationChanged(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Normal)
            {
                wc_settings.gui_chat_x = _form.Left;
                wc_settings.gui_chat_y = _form.Top;
            }
        }

        private void _form_Resize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Normal)
            {
                wc_settings.gui_chat_width = _form.Size.Width;
                wc_settings.gui_chat_height = _form.Size.Height;
            }
        }

        private void _form_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            wc_settings.gui_chat_windowstate = (int)_form.WindowState;
            if (_form.WindowState == FormWindowState.Normal)
            {
                wc_settings.gui_chat_width = _form.Size.Width;
                wc_settings.gui_chat_height = _form.Size.Height;
                wc_settings.gui_chat_x = _form.Left;
                wc_settings.gui_chat_y = _form.Top;
            }
            else if (_form.WindowState == FormWindowState.Minimized)
            {
                wc_settings.gui_chat_windowstate = (int)FormWindowState.Normal;
            }
            wc_settingsManager.SaveSettings(wc_settings);
        }

        public void Show()
        {
            _form.Show();
            _form.Focus();
        }

       

        public void Close()
        {
            wc_settingsManager.SaveSettings(wc_settings);
            _form.Close();
        }


    }
}
