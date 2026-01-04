using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using static opentuner.signal;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using Serilog;

namespace opentuner.ExtraFeatures.BATCSpectrum
{
    public class BATCSpectrum
    {
        public delegate void SignalSelected(int Receiver, uint Freq, uint SymbolRate);

        public event SignalSelected OnSignalSelected;

        private static readonly Object list_lock = new Object();

        static int height = 255;    //makes things easier
        static int bandplan_height = 30;

        Bitmap bmp;
        static Bitmap bmp2;
        Pen greyPen = new Pen(Color.FromArgb(200, 123, 123, 123));
        Pen greyPen2 = new Pen(Color.FromArgb(200, 123, 123, 123));
        SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
        SolidBrush bandplanBrush = new SolidBrush(Color.FromArgb(180, 250, 250, 255));
        SolidBrush overpowerBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        SolidBrush tuner1Brush = new SolidBrush(Color.FromArgb(50, Color.Blue));
        SolidBrush tuner2Brush = new SolidBrush(Color.FromArgb(50, Color.Green));

        Graphics tmp;
        Graphics tmp2;

        int[,] rx_blocks = new int[4, 3];

        double start_freq = 10490.5f;

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

        Timer SpectrumTuneTimer;
        Timer websocketTimer;

        private int _autoTuneMode = 0;

        int connect_retries = 5;
        int connect_retry_count = 0;

        public void updateSignalCallsign(string callsign, double freq, float sr)
        {
            sigs.updateCurrentSignal(callsign, freq, sr);
        }

        public BATCSpectrum(PictureBox Spectrum, int Tuners) 
        {
            _spectrum = Spectrum;

            _tuners = Tuners;

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
                drawspectrum_bandplan();
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
                MessageBox.Show(ex.Message);
            }

            web_socket = new socket();

            sigs = new signal(list_lock);
            web_socket.callback += drawspectrum;
            web_socket.ConnectionStatusChanged += Web_socket_ConnectionStatusChanged;
            sigs.debug += debug;

            // try to connect
            web_socket.start();

            sigs.set_num_rx_scan(num_rxs_to_scan);
            sigs.set_num_rx(1);

            sigs.set_avoidbeacon(true);

            SpectrumTuneTimer = new Timer();
            SpectrumTuneTimer.Enabled = false;
            SpectrumTuneTimer.Interval = 1500;
            SpectrumTuneTimer.Tick += new System.EventHandler(this.SpectrumTuneTimer_Tick);

            websocketTimer = new Timer();
            websocketTimer.Interval = 2000;
            websocketTimer.Tick += new System.EventHandler(this.websocketTimer_Tick);
            websocketTimer.Enabled = true;

            draw_disconnect();
        }

        private void draw_disconnect()
        {
            tmp.Clear(Color.Black);
            tmp.DrawString("FFT Service Disconnected", new Font("Tahoma", 10), Brushes.Red, new PointF(10, 10));

            if (connect_retry_count < connect_retries)
            {
                tmp.DrawString("Retrying ...." + connect_retry_count.ToString() + "/" + connect_retries.ToString(), new Font("Tahoma", 10), Brushes.Red, new PointF(10, 30));
            }
            else
            {
                tmp.DrawString("", new Font("Tahoma", 10), Brushes.Red, new PointF(10, 30));
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
                    draw_disconnect();
                    connect_retry_count += 1;

                    if (connect_retry_count < connect_retries)
                    {
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
            float spectrum_wScale = spectrum_w / 922;

            ushort autotuneWait = 30;

            Tuple<signal.Sig, int> ret = sigs.tune(mode, Convert.ToInt16(autotuneWait), 0);
            if (ret.Item1.frequency > 0)      //above 0 is a change in signal
            {
                System.Threading.Thread.Sleep(100);
                selectSignal(Convert.ToInt32(ret.Item1.fft_centre * spectrum_wScale), 0);
                sigs.set_tuned(ret.Item1, 0);
                rx_blocks[0, 0] = Convert.ToInt16(ret.Item1.fft_centre);
                rx_blocks[0, 1] = Convert.ToInt16(ret.Item1.fft_stop - ret.Item1.fft_start);
            }

        }

        private void debug(string msg)
        {
            Log.Information(msg);
        }

        private void spectrum_MouseLeave(object sender, EventArgs e)
        {
            spectrumTunerHighlight = -1;
        }

        private void spectrum_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                bmp2 = new Bitmap(_spectrum.Width, bandplan_height);     //bandplan
                bmp = new Bitmap(_spectrum.Width, height + 20);
                tmp = Graphics.FromImage(bmp);
                tmp2 = Graphics.FromImage(bmp2);

                if (bandplan != null)
                {
                    drawspectrum_bandplan();
                }
            }
            catch (Exception ex)
            {

            }
        }


        // quicktune functions
        private void drawspectrum_bandplan()
        {
            int span = 9;
            int count = 0;

            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

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

                int pos = Convert.ToInt16((922.0 / span) * (freq - start_freq));
                w = Convert.ToInt32(sr / (span * 1000.0) * 922 * rolloff);
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
            float spectrum_wScale = spectrum_w / 922;

            lock (list_lock)        //hopefully lock signals list while drawing
            {
                //draw the text for each signal found
                foreach (signal.Sig s in signals)
                {
                    tmp.DrawString(s.callsign + "\n" + s.frequency.ToString("#.00") + "\n " + (s.sr * 1000).ToString("#Ks"), new Font("Tahoma", 10), Brushes.White, new PointF(Convert.ToSingle((s.fft_centre * spectrum_wScale) - (25)), (255 - Convert.ToSingle(s.fft_strength + 50))));
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
            catch (Exception ex)
            {

            }

        }

        private void drawspectrum(UInt16[] fft_data)
        {
            tmp.Clear(Color.Black);     //clear canvas


            int spectrum_h = _spectrum.Height - bandplan_height;
            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            int i = 1;
            int y = 0;


            PointF[] points = new PointF[fft_data.Length - 2];

            for (i = 1; i < fft_data.Length - 3; i++)     //ignore padding?
            {
                PointF point = new PointF(i * spectrum_wScale, 255 - fft_data[i] / 255);
                points[i] = point;
            }

            points[0] = new PointF(0, 255);
            points[points.Length - 1] = new PointF(spectrum_w, 255);

            if (spectrumTunerHighlight > -1)
            {
                y = spectrumTunerHighlight * (spectrum_h / _tuners);
                tmp.FillRectangle(tuner1Brush, new RectangleF(0, y, spectrum_w, (spectrum_h/_tuners)));
            }

            //tmp.FillRectangle((spectrumTunerHighlight == 1 ? tuner1Brush : tuner2Brush), new RectangleF(0, (spectrumTunerHighlight == 1 ? 0 : spectrum_h / 2), spectrum_w, _tuners == 1 ? spectrum_h : spectrum_h / 2));

            //tmp.DrawPolygon(greenpen, points);
            SolidBrush spectrumBrush = new SolidBrush(Color.Blue);

            LinearGradientBrush linGrBrush = new LinearGradientBrush(
               new Point(0, 0),
               new Point(0, 255),
               Color.FromArgb(255, 255, 99, 132),   // Opaque red
               Color.FromArgb(255, 54, 162, 235));  // Opaque blue

            tmp.FillPolygon(linGrBrush, points);

            tmp.DrawImage(bmp2, 0, 255 - bandplan_height); //bandplan

            y = 0;

            for (int tuner = 0; tuner < _tuners; tuner++)
            {
                y = tuner * (spectrum_h / _tuners);

                //draw block showing signal selected
                if (rx_blocks[tuner, 0] > 0)
                {
                    tmp.FillRectangle(shadowBrush, new RectangleF(rx_blocks[tuner, 0] * spectrum_wScale - ((rx_blocks[tuner, 1] * spectrum_wScale)/2) , y, rx_blocks[tuner, 1] * spectrum_wScale, (spectrum_h / _tuners)));
                }
            }

            tmp.DrawString(InfoText, new Font("Tahoma", 15), Brushes.White, new PointF(10, 10));
            tmp.DrawString(TX_Text, new Font("Tahoma", 15), Brushes.Red, new PointF(70, _spectrum.Height - 50));  //dh3cs

            //drawspectrum_signals(sigs.detect_signals(fft_data));
            sigs.detect_signals(fft_data);

            // draw over power
            foreach (var sig in sigs.signalsData)
            {
                if (sig.overpower)
                {
                    tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.Rectangle(Convert.ToInt16(sig.fft_centre * spectrum_wScale) - (Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale) / 2), 1, Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale), (255) - 4) });
                }
            }

            for (i = 0; i < _tuners; i++)
            {
                y = i * (spectrum_h / _tuners);
                tmp.DrawLine(greyPen, 10, y, spectrum_w, y);
                tmp.DrawString("RX " + (i+1).ToString(), new Font("Tahoma", 10), Brushes.White, new PointF(5, y));
            }

            drawspectrum_signals(sigs.signalsData);
        }

        private void spectrum_Click(object sender, EventArgs e)
        {

            float spectrum_w = _spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            System.Windows.Forms.MouseEventArgs me = (System.Windows.Forms.MouseEventArgs)e;
            var pos = me.Location;


            int X = pos.X;
            int Y = pos.Y;

            if (me.Button == MouseButtons.Right)
            {
                int freq = Convert.ToInt32((10490.5 + ((X / spectrum_wScale) / 922.0) * 9.0) * 1000.0);
                //UpdateTextBox(txtFreq, freq.ToString());

                string tx_freq = get_bandplan_TX_freq(X, Y);
                debug("TX-Freq: " + tx_freq + " MHz");
                // dh3cs
                if (!string.IsNullOrEmpty(tx_freq))
                {
                    //Clipboard.SetText((Convert.ToDecimal(tx_freq) * 1000).ToString());    //DATV Express in Hz
                    Clipboard.SetText(tx_freq);                                             //DATV-Easy in MHz
                    TX_Text = " TX: " + tx_freq;
                }

            }
            else
            {
                selectSignal(X, Y);
            }

        }


        // quick tune functions - From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank

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
            float spectrum_wScale = spectrum_w / 922;
            int spectrum_h = _spectrum.Height - bandplan_height;

            int rx = determine_rx(Y);

            debug("Select Signal - RX: " + rx.ToString());

            try
            {
                foreach (signal.Sig s in sigs.signals)
                {
                    if ((X / spectrum_wScale) > s.fft_start & (X / spectrum_wScale) < s.fft_stop)
                    {

                        sigs.set_tuned(s, rx);
                        rx_blocks[rx, 0] = Convert.ToInt16(s.fft_centre);
                        rx_blocks[rx, 1] = Convert.ToInt16((s.fft_stop) - (s.fft_start));
                        UInt32 freq = Convert.ToUInt32((s.frequency) * 1000);
                        UInt32 sr = Convert.ToUInt32((s.sr * 1000.0));

                        debug("Freq: " + freq.ToString());
                        debug("SR: " + sr.ToString());

                        OnSignalSelected?.Invoke(rx, freq, sr);

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        int spectrumTunerHighlight = -1;

        public void spectrum_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            get_bandplan_TX_freq(e.X, e.Y);  // dh3cs

            spectrumTunerHighlight = determine_rx(e.Y);

        }

        // moved to separate function, to use by right click in spectrum,  dh3cs 
        private string get_bandplan_TX_freq(int x, int y)  // returns TX-Freq in MHz from the rectangle in Bandplan
        {
            int n = 0;
            string tx_freq_MHz = "";
            if (x > (_spectrum.Height - bandplan_height))
            {
                if (channels != null)
                {
                    foreach (Rectangle ch in channels)
                    {
                        if (x >= ch.Location.X & x <= ch.Location.X + ch.Width)
                        {
                            if (y - (_spectrum.Height - bandplan_height) >= ch.Location.Y - (ch.Height / 2) + 3 & y - (_spectrum.Height - bandplan_height) <= ch.Location.Y + (ch.Height / 2) + 3)
                            {
                                tx_freq_MHz = indexedbandplan[n].Element("s-freq").Value;
                                InfoText = " Dn: " + indexedbandplan[n].Element("x-freq").Value + "  SR: " + indexedbandplan[n].Element("name").Value + Environment.NewLine
                                    + " Up: " + tx_freq_MHz;
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
    }
}
