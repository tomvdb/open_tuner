using System;
using System.Windows.Forms;
using System.Drawing;

namespace opentuner.Utilities
{
    public class DynamicPropertyMediaControls : DynamicPropertyInterface
    {
        private delegate void UpdateLabelDelegate(Label Lbl, Object obj);
        private delegate void UpdateLabelColorDelegate(Label Lbl, Color Col);

        public delegate void ButtonPressedCallback(string key, int function); // 0 = mute, 1 snapshot, 2 = record

        protected GroupBox _parent;
        protected string _key;
        protected string _title;
        protected string _value;

        ToolTip _toolTip = new ToolTip();

        private Button _MuteButton;
        private Button _SnapshotButton;
        private Button _UDPStreamButton;
        private Button _RecordButton;

        private readonly int buttonSize = 32;
        private readonly int left_margin = -10;

        public override string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override string LastValue => _value;

        ButtonPressedCallback _buttonPressedCallback;

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
        }

        public DynamicPropertyMediaControls(GroupBox Group, string Key, string Title, ButtonPressedCallback ButtonPressedCB)
        {
            _buttonPressedCallback = ButtonPressedCB;
            InitComponents(Group, Key, Title, Color.Transparent);
        }

        protected virtual void InitComponents(GroupBox Group, string Key, string Title, System.Drawing.Color Color)
        {
            _parent = Group;
            _key = Key;
            _title = Title;

            int top_margin = 10;

            _parent.Resize += _parent_Resize;

            _MuteButton = new Button();
            _MuteButton.Text = "M";
            _MuteButton.AutoSize = false;
            _MuteButton.Height = buttonSize;
            _MuteButton.Width = buttonSize;
            _MuteButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _MuteButton.Left = left_margin + _parent.Width - (4 * (buttonSize + 2));
            _MuteButton.Click += _MuteButton_Click;
            _toolTip.SetToolTip(_MuteButton, "Mute");

            _SnapshotButton = new Button();
            _SnapshotButton.Text = "S";
            _SnapshotButton.AutoSize = false;
            _SnapshotButton.Height = buttonSize;
            _SnapshotButton.Width = buttonSize;
            _SnapshotButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _SnapshotButton.Left = left_margin + _parent.Width - (3 * (buttonSize + 2));
            _SnapshotButton.Click += _SnapshotButton_Click;
            _toolTip.SetToolTip(_SnapshotButton, "Snapshot");

            _UDPStreamButton = new Button();
            _UDPStreamButton.Text = "U";
            _UDPStreamButton.AutoSize = false;
            _UDPStreamButton.Height = buttonSize;
            _UDPStreamButton.Width = buttonSize;
            _UDPStreamButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _UDPStreamButton.Left = left_margin + _parent.Width - (2 * (buttonSize + 2));
            _UDPStreamButton.Click += _UDPStreamButton_Click;
            _toolTip.SetToolTip(_UDPStreamButton, "UDP Stream");

            _RecordButton = new Button();
            _RecordButton.Text = "R";
            _RecordButton.AutoSize = false;
            _RecordButton.Height = buttonSize;
            _RecordButton.Width = buttonSize;
            _RecordButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _RecordButton.Left = left_margin + _parent.Width - (1 * (buttonSize + 2));
            _RecordButton.Click += _RecordButton_Click;
            _toolTip.SetToolTip(_RecordButton, "Record");

            _parent.Controls.Add(_MuteButton);
            _parent.Controls.Add(_SnapshotButton);
            _parent.Controls.Add(_RecordButton);
            _parent.Controls.Add(_UDPStreamButton);
        }

        private void _UDPStreamButton_Click(object sender, EventArgs e)
        {
            _buttonPressedCallback?.Invoke(_key, 3);
        }

        private void _RecordButton_Click(object sender, EventArgs e)
        {
            _buttonPressedCallback?.Invoke(_key, 2);
        }

        private void _SnapshotButton_Click(object sender, EventArgs e)
        {
            _buttonPressedCallback?.Invoke(_key, 1);
        }

        private void _MuteButton_Click(object sender, EventArgs e)
        {
            _buttonPressedCallback?.Invoke(_key, 0);
        }

        protected virtual void _parent_Resize(object sender, EventArgs e)
        {
            _MuteButton.Left = left_margin + _parent.Width - (4 * (buttonSize + 2));
            _SnapshotButton.Left = left_margin + _parent.Width - (3 * (buttonSize + 2));
            _UDPStreamButton.Left = left_margin + _parent.Width - (2 * (buttonSize + 2));
            _RecordButton.Left = left_margin + _parent.Width - (1 * (buttonSize + 2));
        }

        public override void UpdateColor(Color Col)
        {
            //UpdateColor(_valueLabel, Col);
        }

        public override void UpdateMuteButtonColor(Color Col)
        {
            _MuteButton.BackColor = Col;
        }

        public override void UpdateRecordButtonColor(Color Col)
        {
            _RecordButton.BackColor = Col;
        }

        public override void UpdateStreamButtonColor(Color Col)
        {
            _UDPStreamButton.BackColor = Col;
        }
    }
}
