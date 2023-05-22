using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace opentuner.Classes;

public static class OTColorChanger
{
    private static Color _darkTheme = Color.FromArgb(63, 70, 76);
    private static Color _lightTheme = Color.LightGray;

    public static void OTChangeControlColors(Form form)
    {
        if (!Properties.Settings.Default.DarkMode)
            return;
        
        var controls = GetControls(form);
        form.BackColor = _darkTheme;

        foreach (var control in controls)
        {
            if (control is Label or MenuStrip or GroupBox or TabPage or ListBox or TabControl or Form or TrackBar
                or ComboBox)
            {
                control.BackColor = _darkTheme;
                control.ForeColor = _lightTheme;
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

    public class ToolStripRenderer : ToolStripProfessionalRenderer
    {
        public ToolStripRenderer() : base(new MyColors())
        {
        }
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (e.Item as ToolStripSeparator == null)
            {
                base.OnRenderSeparator(e);
                return;
            }
            int width = e.Item.Width;
            int height = e.Item.Height;

            Color foreColor = _lightTheme;
            Color backColor = _darkTheme;

            e.Graphics.FillRectangle(new SolidBrush(backColor), 0, 0, width, height);
            e.Graphics.DrawLine(new Pen(foreColor), 4, height / 2, width - 4, height / 2);
            
        }
    }

    private class MyColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected => _darkTheme;

        public override Color MenuItemSelectedGradientBegin => _darkTheme;

        public override Color MenuItemSelectedGradientEnd => _darkTheme;

        public override Color MenuItemPressedGradientEnd => _darkTheme;

        public override Color MenuItemPressedGradientBegin => _darkTheme;

        public override Color MenuStripGradientBegin => _darkTheme;

        public override Color MenuStripGradientEnd => _darkTheme;
    }

    public static void OTMenuItemsColoring(MenuStrip menuStrip1, ContextMenuStrip contextSpectrumMenu)
    {
        List<ToolStripMenuItem> toolSripItems = new();

        var allMenuStripItems = GetAllMenuStripItems(menuStrip1, toolSripItems);

        foreach (var toolStripMenuItem in allMenuStripItems)
        {
            toolStripMenuItem.BackColor = _darkTheme;
            toolStripMenuItem.ForeColor = _lightTheme;
        }

        var contextMenuStripItems = GetAllContextMenuStripItems(contextSpectrumMenu, toolSripItems);
        foreach (var toolStripMenuItem in contextMenuStripItems)
        {
            toolStripMenuItem.BackColor = _darkTheme;
            toolStripMenuItem.ForeColor = _lightTheme;
        }
    }

    //Extract all menu strip items
    private static List<ToolStripMenuItem> GetAllMenuStripItems(MenuStrip mnuStrip,
        List<ToolStripMenuItem> toolStripMenuItems)
    {
        foreach (ToolStripMenuItem toolStripItem in mnuStrip.Items)
        {
            GetAllSubMenuStripItems(toolStripItem, toolStripMenuItems);
        }

        return toolStripMenuItems;
    }

    //Extract all context menu strip items
    private static List<ToolStripMenuItem> GetAllContextMenuStripItems(ContextMenuStrip contextSpectrumMenu,
        List<ToolStripMenuItem> toolStripMenuItems)
    {
        foreach (ToolStripMenuItem toolStripItem in contextSpectrumMenu.Items)
        {
            GetAllSubMenuStripItems(toolStripItem, toolStripMenuItems);
        }

        return toolStripMenuItems;
    }

    //This method is called recursively inside to loop through all menu items
    private static void GetAllSubMenuStripItems(ToolStripMenuItem mnuItem, List<ToolStripMenuItem> toolStripMenuItems)
    {
        toolStripMenuItems.Add(mnuItem);

        // if sub menu contain child dropdown items
        if (mnuItem.HasDropDownItems)
        {
            foreach (ToolStripItem toolSripItem in mnuItem.DropDownItems)
            {
                if (toolSripItem is ToolStripMenuItem)
                {
                    //call the method recursively to extract further.
                    GetAllSubMenuStripItems(toolSripItem as ToolStripMenuItem, toolStripMenuItems);
                }
            }
        }
    }
}