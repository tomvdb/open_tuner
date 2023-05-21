using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace opentuner.Classes;

public static class OTColorChanger
{
    private static Color darkTheme = Color.FromArgb(63, 70, 76);
    private static Color lightTheme = Color.LightGray;
    
    public static void OTChangeControlColors(Form form)
    {
        if (!Properties.Settings.Default.DarkMode)
            return;
        var controls = GetControls(form);
        form.BackColor = darkTheme;

        foreach (var control in controls)
        {
            if (control is Label or MenuStrip or GroupBox or TabPage or ListBox or TabControl or Form or TrackBar
                or ComboBox)
            {
                control.BackColor = darkTheme;
                control.ForeColor = lightTheme;
            }
            if (control is Button)
            {
                control.BackColor = Color.FromName("Control");
                control.ForeColor = Color.FromName("ControlText");
            }
        }
    }
    public static List<Control> GetControls(Control form)
    {
        var controlList = new List<Control>();

        foreach (Control childControl in form.Controls)
        {
            // Recurse child controls.
            controlList.AddRange(GetControls(childControl));
            controlList.Add(childControl);
        }

        return controlList;
    }
}