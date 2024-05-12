using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.Utilities
{
    public class DynamicPropertyItem : DynamicPropertyInterface
    {
        private delegate void UpdateLabelDelegate(Label Lbl, Object obj);
        private delegate void UpdateLabelColorDelegate(Label Lbl, Color Col);

        protected GroupBox _parent;
        protected string _key;
        protected string _title;
        protected string _value;
        protected Label _titleLabel;
        private Label _valueLabel;

        ToolTip _toolTip = new ToolTip();

        public ContextMenuStrip ContextMenu
        {
            get { return _valueLabel.ContextMenuStrip;  }
            set 
            { 
                
                _valueLabel.ContextMenuStrip = value; 
                if (value != null )
                {
                    _toolTip.ShowAlways = true;
                    _toolTip.SetToolTip(_valueLabel, "Right Click for Options");
                }
            }
        }

        public override string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override string LastValue { get => _value; }

        // height of the property row
        private const int ItemHeight = 20;

        private void UpdateLabel(Label Lbl, Object obj)
        {

            if (Lbl == null)
                return;

            if (obj == null)
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
                _value = obj.ToString();
            }

        }

        private void UpdateColor(Label Lbl, Color Col)
        {

            if (Lbl == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateLabelColorDelegate ulb = new UpdateLabelColorDelegate(UpdateColor);
                if (Lbl != null)
                {
                    Lbl?.Invoke(ulb, new object[] { Lbl, Col });
                }
            }
            else
            {
                Lbl.BackColor = Col;
            }

        }


        public override void UpdateValue(string Value)
        {
            UpdateLabel(_valueLabel, Value);
        }

        public DynamicPropertyItem(GroupBox Group, string Key, string Title)
        {
            InitComponents(Group, Key, Title, Color.Transparent);
        }

        public DynamicPropertyItem(GroupBox Group, string Key, string Title, System.Drawing.Color color)
        {
            InitComponents(Group, Key, Title, color);
        }

        protected virtual void  InitComponents(GroupBox Group, string Key, string Title, System.Drawing.Color Color)
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


            if (_parent.Controls.Count < 2)
                _titleLabel.Top = _titleLabel.Height + 10;
            else
                _titleLabel.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + 5;

            _valueLabel = new Label();
            _valueLabel.Text = "";
            //_valueLabel.BorderStyle = BorderStyle.FixedSingle;
            _valueLabel.Top = _titleLabel.Top;
            _valueLabel.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _valueLabel.Width = _parent.Width - _valueLabel.Left - 5;
            _valueLabel.Height = _titleLabel.Height;
            _valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valueLabel.BackColor = Color;
            _valueLabel.Name = _key;
            _valueLabel.Tag = _parent.Tag;

            _parent.Controls.Add(_titleLabel);
            _parent.Controls.Add(_valueLabel);
        }


        protected virtual void _parent_Resize(object sender, EventArgs e)
        {
            _titleLabel.Width = _parent.Width / 2 - 5;
            _valueLabel.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _valueLabel.Width = _parent.Width - _valueLabel.Left - 5;
        }

        public override void UpdateColor(Color Col)
        {
            UpdateColor(_valueLabel, Col);
        }

        public override void UpdateMuteButtonColor(Color Col)
        {
            throw new NotImplementedException();
        }

        public override void UpdateRecordButtonColor(Color Col)
        {
            throw new NotImplementedException();
        }

        public override void UpdateStreamButtonColor(Color Col)
        {
            throw new NotImplementedException();
        }
    }
}
