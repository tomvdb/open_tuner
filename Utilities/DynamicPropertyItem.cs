using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.Utilities
{
    public class DynamicPropertyItem
    {

        private delegate void UpdateLabelDelegate(Label Lbl, Object obj);

        private GroupBox _parent;
        private string _key;
        private string _title;
        private Label _titleLabel;
        private Label _valueLabel;

        public ContextMenuStrip ContextMenu
        {
            get { return _valueLabel.ContextMenuStrip;  }
            set { _valueLabel.ContextMenuStrip = value; }
        }

        public string Key
        {
            get { return _key; }
        }

        // height of the property row
        private const int ItemHeight = 20;

        private void UpdateLabel(Label Lbl, Object obj)
        {

            if (Lbl == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateLabelDelegate ulb = new UpdateLabelDelegate(UpdateLabel);
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

        public void UpdateValue(string Value)
        {
            UpdateLabel(_valueLabel, Value);
        }

        public DynamicPropertyItem(GroupBox Group, string Key, string Title)
        {
            InitComponents(Group, Key, Title, Color.White);
        }

        private void InitComponents(GroupBox Group, string Key, string Title, System.Drawing.Color Color)
        {
            _parent = Group;
            _key = Key;
            _title = Title;

            _parent.Resize += _parent_Resize;

            // create controls
            _titleLabel = new Label();
            _titleLabel.Text = _title + " :";
            //_titleLabel.BorderStyle = BorderStyle.FixedSingle;
            _titleLabel.AutoSize = false;
            _titleLabel.Width = _parent.Width / 2 - 5;
            _titleLabel.Height = ItemHeight;
            _titleLabel.Left = 5;
            _titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            if (_parent.Controls.Count == 0)
                _titleLabel.Top = _titleLabel.Height + 5;
            else
                _titleLabel.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _titleLabel.Height + 5;

            _valueLabel = new Label();
            _valueLabel.Text = "Value";
            _valueLabel.BorderStyle = BorderStyle.FixedSingle;
            _valueLabel.Top = _titleLabel.Top;
            _valueLabel.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _valueLabel.Width = _parent.Width - _valueLabel.Left - 5;
            _valueLabel.Height = _titleLabel.Height;
            _valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valueLabel.BackColor = Color;
            _valueLabel.Name = _key;

            _parent.Controls.Add(_titleLabel);
            _parent.Controls.Add(_valueLabel);
        }

        public DynamicPropertyItem(GroupBox Group, string Key, string Title, System.Drawing.Color color)
        {
            InitComponents(Group, Key, Title, color);
        }

        private void _parent_Resize(object sender, EventArgs e)
        {
            _titleLabel.Width = _parent.Width / 2 - 5;
            _valueLabel.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _valueLabel.Width = _parent.Width - _valueLabel.Left - 5;
        }
    }
}
