using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace opentuner.Utilities
{
    public class DynamicPropertySlider : DynamicPropertyInterface
    {
        public delegate void SliderChanged(string key, int value);

        private const int ItemHeight = 20;
        private TrackBar _trackBar;

        private int _min;
        private int _max;

        public event SliderChanged OnSliderChanged;

        protected GroupBox _parent;
        protected string _key;
        protected string _title;
        protected string _value;
        protected Label _titleLabel;
        
        public override string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override string LastValue => _value;

        public DynamicPropertySlider(GroupBox Group, string Key, string Title, int Min, int Max)
        { 
            _min = Min;
            _max = Max;

            InitComponents(Group, Key, Title);
        }

        protected void InitComponents(GroupBox Group, string Key, string Title)
        {
            _parent = Group;
            _key = Key;
            _title = Title;

            _parent.Resize += _parent_Resize;

            // create controls
            _titleLabel = new Label();
            //_titleLabel.BorderStyle = BorderStyle.FixedSingle;
            _titleLabel.AutoSize = false;
            _titleLabel.Width = _parent.Width / 2 - 5;
            _titleLabel.Height = ItemHeight;
            _titleLabel.Left = 5;
            _titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            if (_parent.Controls.Count == 0)
                _titleLabel.Top = _titleLabel.Height + 5;
            else
                _titleLabel.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + 5;

            _trackBar = new TrackBar();
            _trackBar.AutoSize = false;
            _trackBar.Top = _titleLabel.Top;
            _trackBar.Height = ItemHeight;
            _trackBar.Width = _parent.Width - _trackBar.Left - 5;
            _trackBar.Minimum = _min;
            _trackBar.Maximum = _max;

            _trackBar.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _trackBar.Width = _parent.Width - _trackBar.Left - 5;
            _trackBar.Name = _key;

            // set initial value
            _titleLabel.Text = _title + " " + _trackBar.Value.ToString() + " %";
            _trackBar.ValueChanged += _trackBar_ValueChanged;


            _parent.Controls.Add(_titleLabel);
            _parent.Controls.Add(_trackBar);
        }

        private void _trackBar_ValueChanged(object sender, EventArgs e)
        {
            OnSliderChanged?.Invoke(_key, _trackBar.Value);
            _titleLabel.Text = _title + " " + _trackBar.Value.ToString() + " %";
        }

        protected void _parent_Resize(object sender, EventArgs e)
        {
            _titleLabel.Width = _parent.Width / 2 - 5;
            _trackBar.Left = _titleLabel.Left + _titleLabel.Width + 5;
            _trackBar.Width = _parent.Width - _trackBar.Left - 5;
        }

        public override void UpdateValue(string Value)
        {
            int new_value = 0;

            _value = Value;

            if (Int32.TryParse(Value, out new_value))
            {
                //Log.Information("Update slider to " + new_value.ToString());
                _trackBar.Value = new_value;
            }
        }

        public override void UpdateColor(Color Col)
        {
            throw new NotImplementedException();
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
