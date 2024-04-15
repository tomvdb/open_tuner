using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using Vortice.Direct2D1.Effects;
using Newtonsoft.Json.Linq;
using static opentuner.Utilities.DynamicPropertyMediaControls;

namespace opentuner.Utilities
{

    public class DynamicPropertyGroup
    {

        private delegate void UpdateTitleDelegate(CollapsibleGroupBox group_box, Object obj);
        public delegate void SliderChanged(string key, int value);


        private Control _parent;
        private Label _big_num_label;

        //private GroupBox _groupBox;
        private CollapsibleGroupBox _groupBox;
        private List<DynamicPropertyInterface> _items = new List<DynamicPropertyInterface>();

        public event SliderChanged OnSlidersChanged;
        public event ButtonPressedCallback OnMediaButtonPressed;

        private int _id = 0;

        public void setID(int id)
        {
            _id = id;
            _groupBox.Tag = id;
        }

        public int getID()
        {
            return _id;
        }

        public DynamicPropertyGroup(string GroupTitle, Control Parent)
        {
            _parent = Parent;


            // groupbox
            _groupBox = new CollapsibleGroupBox();
            _groupBox.Dock = DockStyle.Top;
            _groupBox.AutoSize = true; 
            _groupBox.Text = GroupTitle;
            _groupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));


            // main number - d number value
            _big_num_label = new Label();
            _big_num_label.Text = "";
            //_big_num_label.BorderStyle = BorderStyle.FixedSingle;
            _big_num_label.BackColor = Color.Transparent;
            _big_num_label.AutoSize = false;
            _big_num_label.Width = 80;
            _big_num_label.Height = 50;
            _big_num_label.Top = 8;
            _big_num_label.Left = _groupBox.Width - _big_num_label.Width - 2;
            _big_num_label.Font = new Font("Arial", 15, FontStyle.Bold);
            _big_num_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            _groupBox.Controls.Add(_big_num_label);
            Parent.Controls.Add(_groupBox);
            Parent.Resize += _groupBox_Resize;

        }


        private void _groupBox_Resize(object sender, EventArgs e)
        {
            _big_num_label.Top = 8;
            _big_num_label.Left = _groupBox.Width - _big_num_label.Width ;
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

        private void UpdateBigLabel(Label Lbl, Object obj)
        {
            if (Lbl == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateLabelDelegate ulb = new UpdateLabelDelegate(UpdateBigLabel);
                if (Lbl != null)
                {
                    Lbl?.Invoke(ulb, new object[] { Lbl, obj });
                }
            }
            else
            {
                Lbl.Text = obj.ToString();
            }
        }


        public void UpdateBigLabel(string Text)
        {
            UpdateBigLabel(_big_num_label, Text);
        }

        public void UpdateTitle(string Title)
        {
            UpdateTitle(_groupBox, Title);
        }

        public void AddItem(string Key, string Name)
        {
            _items.Add(new DynamicPropertyItem(_groupBox, Key, Name));
        }

        public void AddItem(string Key, string Name, Color color)
        {
            _items.Add(new DynamicPropertyItem(_groupBox, Key, Name, color));
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

        void ButtonPressedCallback(string key, int function) // 0 = mute, 1 snapshot, 2 = record
        {
            OnMediaButtonPressed?.Invoke(key, function);
        }

        public void AddMediaControls(string Key, string Name)
        {
            var item = new DynamicPropertyMediaControls(_groupBox, Key, Name, ButtonPressedCallback);
            _items.Add(item);
        }

        private void Item_OnSliderChanged(string key, int value)
        {
            OnSlidersChanged?.Invoke(key, value);
        }

        public void UpdateColor(string Key, Color Col)
        {
            // this can probably be done more efficient, but will do for now
            for (int c = 0; c < _items.Count; c++)
            {
                if (_items[c].Key == Key)
                {
                    _items[c].UpdateColor(Col);
                    break;
                }
            }

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

        public void UpdateMuteButtonColor(string Key, Color Col)
        {
            // this can probably be done more efficient, but will do for now
            for (int c = 0; c < _items.Count; c++)
            {
                if (_items[c].Key == Key)
                {
                    _items[c].UpdateMuteButtonColor(Col);
                    break;
                }
            }
        }

        public Dictionary<string, string> GetAll()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            foreach (var item in _items)
                data.Add(item.Key, item.LastValue);

            return data;
        }
    }
}
