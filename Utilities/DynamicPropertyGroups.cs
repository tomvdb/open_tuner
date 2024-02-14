using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing;

namespace opentuner.Utilities
{

    public class DynamicPropertyGroup
    {

        private delegate void UpdateTitleDelegate(CollapsibleGroupBox group_box, Object obj);
        public delegate void SliderChanged(string key, int value);


        private Control _parent;
        //private GroupBox _groupBox;
        private CollapsibleGroupBox _groupBox;
        private List<DynamicPropertyInterface> _items = new List<DynamicPropertyInterface>();

        public event SliderChanged OnSlidersChanged;

        public DynamicPropertyGroup(string GroupTitle, Control Parent)
        {
            _parent = Parent;
            _groupBox = new CollapsibleGroupBox();
            _groupBox.Dock = DockStyle.Top;
            _groupBox.AutoSize = true; 
            _groupBox.Text = GroupTitle;
            _groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            
            Parent.Controls.Add(_groupBox);
        }

        private void UpdateTitle(CollapsibleGroupBox group_box, Object obj)
        {
            if (group_box == null)
                return;

            if (group_box.InvokeRequired)
            {
                UpdateTitleDelegate ulb = new UpdateTitleDelegate(UpdateTitle);
                if (group_box != null)
                {
                    group_box?.Invoke(ulb, new object[] { group_box, obj });
                }
            }
            else
            {
                group_box.Text = obj.ToString();
            }
        }

        public void UpdateTitle(string Title)
        {
            UpdateTitle(_groupBox, Title);
        }

        public void AddItem(string Key, string Name)
        {
            _items.Add(new DynamicPropertyItem(_groupBox, Key, Name));
        }
        public void AddItem(string Key, string Name, ContextMenuStrip MenuStrip)
        {
            var item = new DynamicPropertyItem(_groupBox, Key, Name, Color.Bisque);
            item.ContextMenu = MenuStrip;
            _items.Add(item);
        }

        public void AddSlider(string Key, string Name, int min, int max)
        {
            var item = new DynamicPropertySlider(_groupBox, Key, Name, min, max);
            item.OnSliderChanged += Item_OnSliderChanged;
            _items.Add(item);
        }

        private void Item_OnSliderChanged(string key, int value)
        {
            OnSlidersChanged?.Invoke(key, value);
        }

        public void UpdateValue(string Key, string Value)
        {
            // this can probably be done more efficient, but will do for now
            for (int c = 0; c < _items.Count; c++)
            {
                if (_items[c].Key == Key)
                {
                    _items[c].UpdateValue(Value);
                    break;
                }
            }
        }

    }
}
