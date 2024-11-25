using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using Serilog;
using opentuner.Utilities;

namespace opentuner.ExtraFeatures.BATCSpectrum
{
    public struct RX
    {
        public float fft_centre;    // signal center position in spectrum
        public float fft_width;     // signal width in spectrum
        public bool locked;         // demode locked state
        public bool streaming;      // streaming flag
    }

    public class BATCSpectrum
    {
        private BATCSpectrumSettings spectrumSettings;
        private SettingsManager<BATCSpectrumSettings> batc_settingsManager;

        public delegate void SignalSelected(int Receiver, uint Freq, uint SymbolRate);

        public event SignalSelected OnSignalSelected;

        private static readonly Object list_lock = new Object();

        private const int height = 246;    //makes things easier
        private static readonly int bandplan_height = 30;
        private const double start_freq = 10490.4754901;

        Bitmap bmp;
        static Bitmap bmp2;
        Pen greyPen = new Pen(Color.FromArgb(200, 123, 123, 123));
        Pen greyPen2 = new Pen(Color.FromArgb(200, 123, 123, 123));
        Pen whitePen = new Pen(Color.FromArgb(200, 255, 255, 255));
        Pen overpowerPen = new Pen(Color.FromArgb(200, Color.Red));
        SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(200, Color.Gray));
        SolidBrush bandplanBrush = new SolidBrush(Color.FromArgb(180, 250, 250, 255));
        SolidBrush overpowerBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        SolidBrush tuner1Brush = new SolidBrush(Color.FromArgb(50, Color.Blue));
        SolidBrush tuner2Brush = new SolidBrush(Color.FromArgb(50, Color.Green));

        Graphics tmp;
        Graphics tmp2;

        RX[] rx_blocks;

        XElement bandplan;
        Rectangle[] channels;
        IList<XElement> indexedbandplan;
        string InfoText;
        string TX_Text;  //dh3cs

        List<string> blocks = new List<string>();

        socket web_socket;
        signal sigs;

        int num_rxs_to_scan = 1;

        private PictureBox _spectrum;
        private int _tuners;

        System.Windows.Forms.Timer SpectrumTuneTimer;
        System.Windows.Forms.Timer websocketTimer;

        private int _autoTuneMode = 0;

        int connect_retries = 5;
        int connect_retry_count = 0;

        private int mousePos_x = 0;
        private int mousePos_y = 0;

        private int fft_data_length = 918;
        private UInt16[] fft_data;

        public bool pluto_control_enabled = false;

        public void updateStreaming(int tuner, bool streaming)
        {
            rx_blocks[tuner].streaming = streaming;
        }

        public void updateSignalCallsign(int tuner, string callsign, double freq, float sr)
        {
            if (rx_blocks[tuner].streaming && rx_blocks[tuner].locked)
            {
                sigs.updateCurrentSignal(callsign, freq, sr);
            }
        }

        public BATCSpectrum(PictureBox Spectrum, int Tuners) 
        {
            spectrumSettings = new BATCSpectrumSettings();
            batc_settingsManager = new SettingsManager<BATCSpectrumSettings>("spectrumSettings");

            spectrumSettings = batc_settingsManager.LoadSettings(spectrumSettings);

            _spectrum = Spectrum;

            _tuners = Tuners;
            rx_blocks = new RX[Tuners];

            thread_wait_event_handles = new EventWaitHandle[]
                {
                    new AutoResetEvent(false),  // index 0: disconnected
                    new AutoResetEvent(false),  // index 1: draw bandplan
                    new AutoResetEvent(false),  // index 2: new fft_data
                    new AutoResetEvent(false)   // index 3: resize graphics
                };
            drawingThreadHandle = new Thread(drawingThread);
            drawingThreadHandle.Priority = ThreadPriority.BelowNormal;
            drawingThreadHandle.Start();

            _spectrum.Click += new System.EventHandler(this.spectrum_Click);
            _spectrum.MouseLeave += new System.EventHandler(this.spectrum_MouseLeave);
            _spectrum.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spectrum_MouseMove);
            _spectrum.SizeChanged += new System.EventHandler(this.spectrum_SizeChanged);

            bmp2 = new Bitmap(_spectrum.Width, bandplan_height);     //bandplan
            bmp = new Bitmap(_spectrum.Width, height + 20);
            tmp = Graphics.FromImage(bmp);
            tmp2 = Graphics.FromImage(bmp2);

            greyPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            greyPen.DashPattern = new float[] { 4.0F, 4.0F };

            try
            {
                bandplan = XElement.Load(Path.GetDirectoryName(Application.ExecutablePath) + @"\extra\bandplan.xml");
                thread_wait_event_handles[1].Set(); // fire worker thread to draw bandplan
                indexedbandplan = bandplan.Elements().ToList();
                foreach (var channel in bandplan.Elements("channel"))
                {
                    if (!blocks.Contains(channel.Element("block").Value))
                    {
                        blocks.Add(channel.Element("block").Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            web_socket = new socket();

            sigs = new signal(list_lock);
            web_socket.callback += new_fft_data;
            web_socket.ConnectionStatusChanged += Web_socket_ConnectionStatusChanged;
            sigs.debug += debug;

            // try to connect
            web_socket.start();

            sigs.set_num_rx_scan(num_rxs_to_scan);
            sigs.set_num_rx(Tuners);

            sigs.set_avoidbeacon(true);

            SpectrumTuneTimer = new System.Windows.Forms.Timer();
            SpectrumTuneTimer.Enabled = false;
            SpectrumTuneTimer.Interval = 1500;
            SpectrumTuneTimer.Tick += new System.EventHandler(this.SpectrumTuneTimer_Tick);

            websocketTimer = new System.Windows.Forms.Timer();
            websocketTimer.Interval = 2000;
            websocketTimer.Tick += new System.EventHandler(this.websocketTimer_Tick);
            websocketTimer.Enabled = true;

            thread_wait_event_handles[0].Set(); // fire worker thread to draw disconnected
        }

        private void draw_disconnect()
        {
            tmp.Clear(Color.Black);
            tmp.DrawString("FFT Service Disconnected", new Font("Tahoma", 10), Brushes.Red, new PointF(10, 10));

            if (connect_retry_count < connect_retries)
            {
                tmp.DrawString("Retrying ...." + (connect_retry_count + 1).ToString() + "/" + connect_retries.ToString(), new Font("Tahoma", 10), Brushes.Red, new PointF(10, 30));
            }
            else
            {
                tmp.DrawString("Click with mouse to reconnect", new Font("Tahoma", 10), Brushes.Red, new PointF(10, 30));
            }
            UpdateDrawing();
        }

        private void Web_socket_ConnectionStatusChanged(object sender, bool connection_status)
        {
            if (connection_status)  // we are connected
            {
                // reset retry count
                connect_retry_count = 0;
            }
            else
            {
                // if we lost connection then disable autotune
                _autoTuneMode = 0;
                SpectrumTuneTimer.Enabled = false;
            }
        }

        public void Close()
        {
            drawingThreadHandle?.Abort();

            websocketTimer?.Stop();
            websocketTimer?.Dispose();

            SpectrumTuneTimer?.Stop();
            SpectrumTuneTimer?.Dispose();
            
            // stop socket
            web_socket?.stop();
        }

        private void websocketTimer_Tick(object sender, EventArgs e)
        {
            if (web_socket != null)
            {

                TimeSpan t = DateTime.Now - web_socket.lastdata;

                if (t.Seconds > 2)
                {
                    web_socket.stop();
                }

                if (!web_socket.connected)
                {
                    thread_wait_event_handles[0].Set(); // fire worker thread to draw disconnected

                    if (connect_retry_count < connect_retries)
                    {
                        connect_retry_count += 1;
                        debug("Websocket Not Connected: Retrying " + connect_retry_count.ToString() + "/" + connect_retries.ToString());
                        web_socket.start();
                    }
                }

            }
        }

        public void changeTuneMode(int mode)
        {
            _autoTuneMode = mode;

            if (mode == 0)
            {
                SpectrumTuneTimer?.Stop();
            }
            else
            {
                SpectrumTuneTimer?.Start();
            }

        }

        private void SpectrumTuneTimer_Tick(object sender, EventArgs e)
        {
            int mode = _autoTuneMode;
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;

            ushort autotuneWait = 30;

            Tuple<signal.Sig, int> ret = sigs.tune(mode, Convert.ToInt16(autotuneWait), 0);
            if (ret.Item1.frequency > 0)      //above 0 is a change in signal
            {
                System.Threading.Thread.Sleep(100);
                selectSignal(Convert.ToInt32(ret.Item1.text_pos * spectrum_wScale), 0);
                sigs.set_tuned(ret.Item1, 0);
                rx_blocks[0].fft_centre = ret.Item1.text_pos;
                rx_blocks[0].fft_width = ret.Item1.sr * 100.0f / fft_data_length / 9.0f;
            }
        }

        private void debug(string msg)
        {
            //Log.Information(msg);
        }

        private void spectrum_MouseLeave(object sender, EventArgs e)
        {
            get_bandplan_TX_freq(0, 0);  // dh3cs
            mousePos_x = 0;
            mousePos_y = 0;
            spectrumTunerHighlight = -1;
        }

        private void spectrum_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                thread_wait_event_handles[3].Set(); // fire worker thread to resize graphics

                bmp2 = new Bitmap(_spectrum.Width, bandplan_height);    // bandplan
                bmp = new Bitmap(_spectrum.Width, height + 20);
                tmp = Graphics.FromImage(bmp);
                tmp2 = Graphics.FromImage(bmp2);

                if (bandplan != null)
                {
                    thread_wait_event_handles[1].Set(); // fire worker thread to draw bandplan
                }
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
            }
        }


        // quicktune functions
        private void drawspectrum_bandplan()
        {
            int span = 9;
            int count = 0;

            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;

            List<string> blocks = new List<string>();

            //count blocks ('layers' of bandplan)
            foreach (var channel in bandplan.Elements("channel"))
            {
                count++;
                if (!blocks.Contains(channel.Element("block").Value))
                {
                    blocks.Add(channel.Element("block").Value);
                }
            }

            channels = new Rectangle[count];

            int n = 0;

            //create rectangle blocks to display bandplan
            foreach (var channel in bandplan.Elements("channel"))
            {
                int w = 0;
                int offset = 0;
                float rolloff = 1.35f;
                string xval = channel.Element("x-freq").Value;

                float freq;
                int sr;

                freq = Convert.ToSingle(xval, CultureInfo.InvariantCulture);
                sr = Convert.ToInt32(channel.Element("sr").Value, CultureInfo.InvariantCulture);

                int pos = Convert.ToInt16((fft_data_length / span) * (freq - start_freq));
                w = Convert.ToInt32(sr / (span * 1000.0) * fft_data_length * rolloff);
                w = Convert.ToInt32(w * spectrum_wScale);

                int split = bandplan_height / blocks.Count();
                int b = blocks.Count();
                foreach (string blk in blocks)
                {
                    if (channel.Element("block").Value == blk)
                    {
                        offset = b * split;
                    }
                    b--;
                }
                channels[n] = new Rectangle(Convert.ToInt32(pos * spectrum_wScale) - (w / 2), offset - (split / 2) - 3, w, split - 2);
                n++;
            }

            //draw blocks
            for (int i = 0; i < count; i++)
            {
                tmp2.FillRectangles(bandplanBrush, new RectangleF[] { channels[i] });      //x,y,w,h
            }
        }

        private void drawspectrum_signals(List<signal.Sig> signals)
        {
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;

            lock (list_lock)        //hopefully lock signals list while drawing
            {
                //draw the text for each signal found
                foreach (signal.Sig s in signals)
                {
                    if (check_mouse_over_signal(s))
                    {
                        tmp.DrawLine(whitePen, Convert.ToInt16(s.fft_start * spectrum_wScale), height - Convert.ToInt16(s.fft_strength / height), Convert.ToInt16(s.fft_stop * spectrum_wScale), height - Convert.ToInt16(s.fft_strength / height));
                        tmp.DrawLine(whitePen, Convert.ToInt16(s.fft_start * spectrum_wScale), height - Convert.ToInt16(s.fft_strength / height), Convert.ToInt16(s.fft_start * spectrum_wScale), height);
                        tmp.DrawLine(whitePen, Convert.ToInt16(s.fft_stop * spectrum_wScale), height - Convert.ToInt16(s.fft_strength / height), Convert.ToInt16(s.fft_stop * spectrum_wScale), height);
                        tmp.DrawString(s.callsign + "\n" + s.frequency.ToString("#0.000") + "\n " + (s.sr * 1000).ToString("#Ks") + "\n " + s.dbb.ToString("#0.0dBb"), new Font("Tahoma", 10), Brushes.White, new PointF(s.text_pos * spectrum_wScale - 30.0f, height - Convert.ToSingle(s.fft_strength / height + 50)));
                    }
                    else
                    {
                        tmp.DrawString(s.callsign + "\n" + s.frequency.ToString("#0.000") + "\n " + (s.sr * 1000).ToString("#Ks"), new Font("Tahoma", 10), Brushes.White, new PointF(s.text_pos * spectrum_wScale - 30.0f, height - Convert.ToSingle(s.fft_strength / height + 50)));
                    }
                }
            }
            UpdateDrawing();
        }

        private void UpdateDrawing()
        {
            try
            {
                _spectrum.Parent?.Invoke(new MethodInvoker(delegate () { _spectrum.Image = bmp; _spectrum.Update(); }));
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
            }

        }

        // Entrypoint with new fft_data from websocket
        private void new_fft_data(UInt16[] _fft_data)
        {
            fft_data = _fft_data;
            fft_data_length = fft_data.Length;

            sigs.detect_signals(fft_data);

            int spectrum_h = _spectrum.Height - bandplan_height;
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;
            int y;

            // auto tune function
            for (int i = 0; i < _tuners; i++)
            {
                y = i * (spectrum_h / _tuners);
                if (spectrumSettings.tuneMode[i] == 1 && rx_blocks[i].locked == false)
                {
                    Tuple<signal.Sig, int> ret = sigs.tune(spectrumSettings.tuneMode[i], 30, i);
                    if (ret.Item1.frequency > 0)      //above 0 is a change in signal
                    {
                        selectSignal(Convert.ToInt32(ret.Item1.text_pos * spectrum_wScale), y);
                        sigs.set_tuned(ret.Item1, i);
                        rx_blocks[i].fft_centre = ret.Item1.text_pos;
                        rx_blocks[i].fft_width = ret.Item1.sr * 100.0f / fft_data_length / 9.0f;
                    }
                }
            }
            thread_wait_event_handles[2].Set(); // fire worker thread to draw new fft_data
        }

        private void drawspectrum()
        {
            tmp.Clear(Color.Black);     //clear canvas

            int spectrum_h = _spectrum.Height - bandplan_height;
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;

            int i = 1;
            int y = 0;

            PointF[] points = new PointF[fft_data.Length];

            for (i = 1; i < fft_data.Length; i++)     //ignore padding?
            {
                PointF point = new PointF(i * spectrum_wScale, height - fft_data[i] / height);
                points[i] = point;
            }

            points[0] = new PointF(0, height);
            points[points.Length - 1] = new PointF(spectrum_w, height);

            if (spectrumTunerHighlight > -1)
            {
                y = spectrumTunerHighlight * (spectrum_h / _tuners);
                tmp.FillRectangle(tuner1Brush, new RectangleF(0, y, spectrum_w, (spectrum_h / _tuners)));
            }

            //tmp.FillRectangle((spectrumTunerHighlight == 1 ? tuner1Brush : tuner2Brush), new RectangleF(0, (spectrumTunerHighlight == 1 ? 0 : spectrum_h / 2), spectrum_w, _tuners == 1 ? spectrum_h : spectrum_h / 2));

            //tmp.DrawPolygon(greenpen, points);
            SolidBrush spectrumBrush = new SolidBrush(Color.Blue);

            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(0, 0),
            new Point(0, height),
            Color.FromArgb(255, 255, 99, 132),   // Opaque red
            Color.FromArgb(255, 54, 162, 235));  // Opaque blue

            tmp.FillPolygon(linGrBrush, points);

            tmp.DrawImage(bmp2, 0, height - bandplan_height); //bandplan

            y = 0;

            for (int tuner = 0; tuner < _tuners; tuner++)
            {
                y = tuner * (spectrum_h / _tuners);

                //draw block showing signal selected
                if (rx_blocks[tuner].fft_centre > 0.0f)
                {
                    tmp.FillRectangle(shadowBrush, new RectangleF(rx_blocks[tuner].fft_centre * spectrum_wScale - ((rx_blocks[tuner].fft_width * spectrum_wScale) / 2), y, rx_blocks[tuner].fft_width * spectrum_wScale, (spectrum_h / _tuners)));
                }
            }

            tmp.DrawString(InfoText, new Font("Tahoma", 15), Brushes.White, new PointF(10, 10));
            tmp.DrawString(TX_Text, new Font("Tahoma", 15), Brushes.Red, new PointF(70, _spectrum.Height - 50));  //dh3cs

            //drawspectrum_signals(sigs.detect_signals(fft_data));
            //sigs.detect_signals(fft_data);

            // draw over power
            lock (list_lock)
            {
                foreach (var sig in sigs.signals)
                {
                    if (sig.overpower)
                    {
                        switch (spectrumSettings.overPowerIndicatorLayout)
                        {
                            case 0:     // classic
                                tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.RectangleF(sig.text_pos * spectrum_wScale - ((sig.fft_stop - sig.fft_start) * spectrum_wScale / 2), 1, (sig.fft_stop - sig.fft_start) * spectrum_wScale, height - 4) });
                                break;
                            case 1:     // classic + line only
                                tmp.DrawLine(overpowerPen, Convert.ToInt16(sig.fft_start * spectrum_wScale - 15), height - Convert.ToInt16(sig.max_strength / height), Convert.ToInt16(sig.fft_stop * spectrum_wScale + 15), height - Convert.ToInt16(sig.max_strength / height));
                                tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.RectangleF(sig.text_pos * spectrum_wScale - ((sig.fft_stop - sig.fft_start) * spectrum_wScale / 2), 1, (sig.fft_stop - sig.fft_start) * spectrum_wScale, height - 4) });
                                break;
                            case 2:     // box from top to line
                                tmp.DrawLine(overpowerPen, Convert.ToInt16(sig.fft_start * spectrum_wScale - 15), height - Convert.ToInt16(sig.max_strength / height), Convert.ToInt16(sig.fft_stop * spectrum_wScale + 15), height - Convert.ToInt16(sig.max_strength / height));
                                tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.RectangleF(sig.text_pos * spectrum_wScale - ((sig.fft_stop - sig.fft_start) * spectrum_wScale / 2), 1, (sig.fft_stop - sig.fft_start) * spectrum_wScale, height - Convert.ToInt16(sig.max_strength / height)) });
                                break;
                            case 3:     // box from line to bottom
                                tmp.DrawLine(overpowerPen, Convert.ToInt16(sig.fft_start * spectrum_wScale - 15), height - Convert.ToInt16(sig.max_strength / height), Convert.ToInt16(sig.fft_stop * spectrum_wScale + 15), height - Convert.ToInt16(sig.max_strength / height));
                                tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.RectangleF(sig.text_pos * spectrum_wScale - ((sig.fft_stop - sig.fft_start) * spectrum_wScale / 2), height - sig.max_strength / height, (sig.fft_stop - sig.fft_start) * spectrum_wScale, height - 4) });
                                break;
                            case 4:     // line only
                                tmp.DrawLine(overpowerPen, Convert.ToInt16(sig.fft_start * spectrum_wScale - 15), height - Convert.ToInt16(sig.max_strength / height), Convert.ToInt16(sig.fft_stop * spectrum_wScale + 15), height - Convert.ToInt16(sig.max_strength / height));
                                break;
                            default:    // no indication
                                break;
                        }
                    }
                }
            }

            for (i = 0; i < _tuners; i++)
            {
                y = i * (spectrum_h / _tuners);
                tmp.DrawLine(greyPen, 10, y, spectrum_w, y);
                tmp.DrawString("RX " + (i + 1).ToString(), new Font("Tahoma", 10), Brushes.White, new PointF(5, y));
                switch (spectrumSettings.tuneMode[i])
                {
                    case 0:
                        tmp.DrawString("Manual", new Font("Tahoma", 10), Brushes.White, new PointF(5, y + 14));
                        break;
                    case 1:
                        tmp.DrawString("Auto", new Font("Tahoma", 10), Brushes.White, new PointF(5, y + 14));
                        break;
                    default:
                        break;
                }
            }
            drawspectrum_signals(sigs.signals);
        }

        private void spectrum_Click(object sender, EventArgs e)
        {
            if (web_socket.connected)
            {
                int spectrum_h = _spectrum.Height - bandplan_height;
                float spectrum_w = _spectrum.Width;
                float spectrum_wScale = spectrum_w / fft_data_length;

                MouseEventArgs me = (MouseEventArgs)e;
                var pos = me.Location;

                int X = pos.X;
                int Y = pos.Y;

                if (Y > spectrum_h)
                {
                    if (pluto_control_enabled)
                    {
                        switch (me.Button)
                        {
                            case MouseButtons.Left:
                                string tx_freq = get_bandplan_TX_freq(X, Y);
                                debug("TX-Freq: " + tx_freq + " MHz");
                                // dh3cs
                                if (!string.IsNullOrEmpty(tx_freq))
                                {
                                    //Clipboard.SetText((Convert.ToDecimal(tx_freq) * 1000).ToString());    //DATV Express in Hz
                                    Clipboard.SetText(tx_freq);                                             //DATV-Easy in MHz
                                    TX_Text = " TX: " + tx_freq;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    switch (me.Button)
                    {
                        case MouseButtons.Left:
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                uint freq = Convert.ToUInt32((10490.5 + (X / spectrum_wScale / 922.0) * 9.0) * 1000.0);

                                using (opentuner.SRForm srForm = new opentuner.SRForm(freq))      //open up the manual sr select form
                                {
                                    Point spectrum_screen_location = _spectrum.PointToScreen(_spectrum.Location);
                                    Point new_srForm_location = spectrum_screen_location;
                                    int spectrum_width = _spectrum.Size.Width;
                                    int srForm_width = srForm.Size.Width;

                                    if (X > (srForm_width / 2))
                                        new_srForm_location.X = spectrum_screen_location.X + X - srForm.Size.Width / 2;
                                    if (X > (spectrum_width - srForm.Size.Width / 2))
                                        new_srForm_location.X = spectrum_screen_location.X + (spectrum_width - srForm.Size.Width);

                                    srForm.StartPosition = FormStartPosition.Manual;
                                    srForm.Location = new_srForm_location;
                                    DialogResult result = srForm.ShowDialog();
                                    if (result == DialogResult.OK)
                                    {
                                        OnSignalSelected?.Invoke(determine_rx(Y), freq, srForm.getsr());
                                    }
                                }
                            }
                            else
                            {
                                selectSignal(X, Y);
                            }
                            break;
                        case MouseButtons.Right:
                            int tuner = determine_rx(Y);
                            using (oneTunerTuneModeForm oTTMForm = new oneTunerTuneModeForm(
                                tuner + 1,
                                spectrumSettings.tuneMode[tuner],
                                spectrumSettings.avoidBeacon[tuner]))      //open up the single tune mode select form
                            {
                                Point spectrum_screen_location = _spectrum.PointToScreen(_spectrum.Location);
                                Point new_oTTMForm_location = spectrum_screen_location;
                                int spectrum_width = _spectrum.Size.Width;
                                int oTTMForm_width = oTTMForm.Size.Width;

                                if (X > (oTTMForm_width / 2))
                                    new_oTTMForm_location.X = spectrum_screen_location.X + X - oTTMForm.Size.Width / 2;
                                if (X > (spectrum_width - oTTMForm.Size.Width / 2))
                                    new_oTTMForm_location.X = spectrum_screen_location.X + (spectrum_width - oTTMForm.Size.Width);

                                oTTMForm.StartPosition = FormStartPosition.Manual;
                                oTTMForm.Location = new_oTTMForm_location;
                                DialogResult result = oTTMForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    spectrumSettings.tuneMode[tuner] = oTTMForm.getTuneMode();
                                    spectrumSettings.avoidBeacon[tuner] = oTTMForm.getAvoidBeacon();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (connect_retry_count >= connect_retries)
                {
                    connect_retry_count = 0;
                }
            }
        }

        // quick tune functions - From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank

        public void updateTuner(int tuner, double freq, float sr, bool demod_locked)
        {
            if (demod_locked)
            {
                if (!rx_blocks[tuner].locked)
                {
                    float fft_centre = Convert.ToSingle((freq - start_freq) * fft_data_length / 9.0f);
                    float fft_width = sr * fft_data_length / 9.0f;
                    if (rx_blocks[tuner].streaming)
                    {
                        rx_blocks[tuner].fft_centre = fft_centre;
                        rx_blocks[tuner].fft_width = fft_width;
                        rx_blocks[tuner].locked = true;     // locked
                    }
                }
            }
            else    // !demod_locked
            {
                if (rx_blocks[tuner].locked)
                {
                    rx_blocks[tuner].locked = false;    // not locked
                }
            }
        }

        public void switchTuner(int tuner, double freq, float sr)
        {
            rx_blocks[tuner].fft_centre = Convert.ToSingle((freq - start_freq) * fft_data_length / 9.0f);
            rx_blocks[tuner].fft_width = sr * fft_data_length / 9.0f;
        }

        private int determine_rx(int pos)
        {
            int rx = 0;
            int div = (_spectrum.Height - bandplan_height) / _tuners;
            rx = pos / div;

            return rx;
        }

        private void selectSignal(int X, int Y)
        {

            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;
            int spectrum_h = _spectrum.Height - bandplan_height;

            int rx = determine_rx(Y);

            debug("Select Signal - RX: " + rx.ToString());

            try
            {
                lock (list_lock)
                {
                    foreach (signal.Sig s in sigs.newSignals)
                    {
                        if ((X / spectrum_wScale) > s.fft_start & (X / spectrum_wScale) < s.fft_stop)
                        {
                            sigs.set_tuned(s, rx);
                            rx_blocks[rx].fft_centre = s.text_pos;
                            rx_blocks[rx].fft_width = s.sr * fft_data_length / 9.0f;

                            UInt32 freq = Convert.ToUInt32((s.frequency) * 1000);
                            UInt32 sr = Convert.ToUInt32((s.sr * 1000.0));

                            debug("Freq: " + freq.ToString());
                            debug("SR: " + sr.ToString());

                            OnSignalSelected?.Invoke(rx, freq, sr);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Log.Error(Ex.Message);
            }
        }

        int spectrumTunerHighlight = -1;

        private bool check_mouse_over_signal(signal.Sig s)
        {
            int spectrum_h = _spectrum.Height - bandplan_height;
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / fft_data_length;

            if (mousePos_y < spectrum_h)
            {
                if ((mousePos_x > (s.fft_start * spectrum_wScale)) && (mousePos_x < (s.fft_stop * spectrum_wScale)) && (mousePos_y > (_spectrum.Height - s.fft_strength / height)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void spectrum_MouseMove(object sender, MouseEventArgs e)
        {
            get_bandplan_TX_freq(e.X, e.Y);  // dh3cs
            // save mouse position for rendering signal box
            mousePos_x = e.X;
            mousePos_y = e.Y;

            spectrumTunerHighlight = determine_rx(e.Y);

        }

        // moved to separate function, to use by right click in spectrum,  dh3cs 
        private string get_bandplan_TX_freq(int x, int y)  // returns TX-Freq in MHz from the rectangle in Bandplan
        {
            int n = 0;
            string tx_freq_MHz = "";
            if (y > (_spectrum.Height - bandplan_height))
            {
                if (channels != null)
                {
                    foreach (Rectangle ch in channels)
                    {
                        if (x >= ch.Location.X & x <= ch.Location.X + ch.Width)
                        {
                            if (y - (_spectrum.Height - bandplan_height) >= ch.Location.Y - (ch.Height / 2) + 3 & y - (_spectrum.Height - bandplan_height) <= ch.Location.Y + (ch.Height / 2) + 3)
                            {
                                if (indexedbandplan[n].Element("name").Value != "BEACON")
                                {
                                    tx_freq_MHz = indexedbandplan[n].Element("s-freq").Value;
                                }
                                InfoText = " Dn: " + indexedbandplan[n].Element("x-freq").Value + "  SR: " + indexedbandplan[n].Element("name").Value + Environment.NewLine
                                    + " Up: " + tx_freq_MHz;
                                break;
                            }
                            else
                            {
                                if (InfoText != "")
                                {
                                    InfoText = "";
                                }
                            }
                        }
                        n++;
                    }
                }
            }
            else
            {
                if (InfoText != "")
                {
                    InfoText = "";
                }
            }
            return tx_freq_MHz;
        }

        private EventWaitHandle[] thread_wait_event_handles;
        private Thread drawingThreadHandle;

        private void drawingThread()
        {
            Log.Information("drawingThread started");
            int eventIndex;
            try
            {
                while (true)
                {
                    eventIndex = EventWaitHandle.WaitAny(thread_wait_event_handles);
                    switch (eventIndex)
                    {
                        case 0:
                            //Log.Information("drawingThread event diconnected");
                            draw_disconnect();
                            break;
                        case 1:
                            //Log.Information("drawingThread event draw_bandplan");
                            if (bandplan != null)
                            {
                                drawspectrum_bandplan();
                            }
                            break;
                        case 2:
                            //Log.Information("drawingThread event new fft_data");
                            drawspectrum();
                            break;
                        case 3:
                            bmp2 = new Bitmap(_spectrum.Width, bandplan_height);    // bandplan
                            tmp2 = Graphics.FromImage(bmp2);
                            bmp = new Bitmap(_spectrum.Width, height + 20); // ?
                            tmp = Graphics.FromImage(bmp);
                            break;
                        default:
                            Log.Information("drawingThread event: " + eventIndex.ToString());
                            break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Log.Information("drawingThread terminated");
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                Log.Error("drawingThread Exception: " + ex.Message);
            }
        }
    }
}
