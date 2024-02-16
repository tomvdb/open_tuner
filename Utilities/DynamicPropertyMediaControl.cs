using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using static opentuner.Utilities.DynamicPropertyMediaControls;

namespace opentuner.Utilities
{
    public enum PropertyIndicators
    {
        RecordingIndicator,
        StreamingIndicator
    };

    public class DynamicPropertyMediaControls : DynamicPropertyInterface
    {
        private delegate void UpdateLabelDelegate(Label Lbl, Object obj);
        private delegate void UpdateLabelColorDelegate(Label Lbl, Color Col);

        public delegate void ButtonPressedCallback(string key, int function); // 0 = mute, 1 snapshot, 2 = record

        protected GroupBox _parent;
        protected string _key;
        protected string _title;

        ToolTip _toolTip = new ToolTip();

        private Button _MuteButton;
        private Button _SnapshotButton;
        private Button _UDPStreamButton;
        private Button _RecordButton;

        private PictureBox _recordIndicator;
        private PictureBox _streamIndicator;

        Bitmap _recordBitmap;
        Bitmap _streamBitmap;
        Bitmap _offBitmap;

        private int buttonSize = 32;
        public override string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        ButtonPressedCallback _buttonPressedCallback;

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

        public bool IsIndicatorSet(int indicatorInput, PropertyIndicators indicator)
        {
            
            return (indicatorInput & (1 << (int)indicator)) != 0;
        }

        public override void UpdateValue(string Value)
        {
            Console.WriteLine("Update Indicator Value: " + Value);

            int indicatorInput = 0;

            if (int.TryParse(Value, out indicatorInput))
            {
                if (IsIndicatorSet(indicatorInput, PropertyIndicators.RecordingIndicator))
                {
                    _recordIndicator.Image = _recordBitmap;
                }
                else
                {
                    _recordIndicator.Image = _offBitmap;
                }

                if (IsIndicatorSet(indicatorInput, PropertyIndicators.StreamingIndicator))
                {
                    _streamIndicator.Image = _streamBitmap;
                }
                else
                {
                    _streamIndicator.Image = _offBitmap;
                }

            }
        }

        private void DrawIndicator(ref Bitmap bmp, Brush brush)
        {
            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                // Draw a red filled circle
                int circleDiameter = 30; // Diameter of the circle
                int x = (32 - circleDiameter) / 2; // X-coordinate of the circle
                int y = (32 - circleDiameter) / 2; // Y-coordinate of the circle
                g.FillEllipse(brush, x, y, circleDiameter, circleDiameter);
                g.DrawEllipse(Pens.Gray, x, y, circleDiameter, circleDiameter);
            }
        }

        public DynamicPropertyMediaControls(GroupBox Group, string Key, string Title, ButtonPressedCallback ButtonPressedCB)
        {
            _buttonPressedCallback = ButtonPressedCB;

            _recordBitmap = new Bitmap(32,32);
            DrawIndicator(ref _recordBitmap, Brushes.PaleVioletRed);

            _streamBitmap = new Bitmap(32,32);
            DrawIndicator(ref _streamBitmap, Brushes.PaleTurquoise);

            _offBitmap = new Bitmap(32,32);
            DrawIndicator(ref _offBitmap, Brushes.LightGray);

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
            _MuteButton.Left = _parent.Width - (4 * (buttonSize + 2)) - (_parent.Width / 4) / 2;
            _MuteButton.Click += _MuteButton_Click;
            _toolTip.SetToolTip(_MuteButton, "Mute");

            _SnapshotButton = new Button();
            _SnapshotButton.Text = "S";
            _SnapshotButton.AutoSize = false;
            _SnapshotButton.Height = buttonSize;
            _SnapshotButton.Width = buttonSize;
            _SnapshotButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _SnapshotButton.Left = _parent.Width - (3 * (buttonSize + 2)) - (_parent.Width / 4) /2;
            _SnapshotButton.Click += _SnapshotButton_Click;
            _toolTip.SetToolTip(_SnapshotButton, "Snapshot");

            _UDPStreamButton = new Button();
            _UDPStreamButton.Text = "U";
            _UDPStreamButton.AutoSize = false;
            _UDPStreamButton.Height = buttonSize;
            _UDPStreamButton.Width = buttonSize;
            _UDPStreamButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _UDPStreamButton.Left = _parent.Width - (2 * (buttonSize + 2)) - (_parent.Width / 4) / 2;
            _UDPStreamButton.Click += _UDPStreamButton_Click;
            _toolTip.SetToolTip(_UDPStreamButton, "UDP Stream");

            _RecordButton = new Button();
            _RecordButton.Text = "R";
            _RecordButton.AutoSize = false;
            _RecordButton.Height = buttonSize;
            _RecordButton.Width = buttonSize;
            _RecordButton.Top = _parent.Controls[_parent.Controls.Count - 1].Top + _parent.Controls[_parent.Controls.Count - 1].Height + top_margin;
            _RecordButton.Left = _parent.Width - (1 * (buttonSize + 2)) - (_parent.Width / 4) / 2;
            _RecordButton.Click += _RecordButton_Click;
            _toolTip.SetToolTip(_RecordButton, "Record");

            _streamIndicator = new PictureBox();
            _streamIndicator.Size = new Size(buttonSize, buttonSize);
            _streamIndicator.Image = _offBitmap;
            _streamIndicator.Top = _MuteButton.Top;
            _streamIndicator.Left = 10;
            _toolTip.SetToolTip(_streamIndicator, "UDP Stream Indicator");

            _recordIndicator = new PictureBox();
            _recordIndicator.Size = new Size(buttonSize, buttonSize);
            _recordIndicator.Image = _offBitmap;
            _recordIndicator.Top = _MuteButton.Top;
            _recordIndicator.Left = 10 + 42;
            _toolTip.SetToolTip(_recordIndicator, "Record Indicator");

            _parent.Controls.Add(_recordIndicator);
            _parent.Controls.Add(_streamIndicator);
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
            _MuteButton.Left = _parent.Width - (4 * (buttonSize + 2)) - (_parent.Width/4) / 2;
            _SnapshotButton.Left = _parent.Width - (3 * (buttonSize + 2)) - (_parent.Width / 4) / 2;
            _UDPStreamButton.Left = _parent.Width - (2 * (buttonSize + 2)) - (_parent.Width / 4) / 2;
            _RecordButton.Left = _parent.Width - (1 * (buttonSize + 2)) - (_parent.Width / 4) / 2;

        }

        public override void UpdateColor(Color Col)
        {
            //UpdateColor(_valueLabel, Col);
        }
    }
 }
