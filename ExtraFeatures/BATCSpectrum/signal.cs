// From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.BATCSpectrum
{
    class signal
    {

        public int beacon_strength = -1;

        //struct for signal information
        public struct Sig
        {
            public int fft_start;
            public int fft_stop;
            public float fft_centre;
            public float text_pos;      // not wobbling position for showing the text message. (derived from fft_centre)
            public int fft_strength;
            public double frequency;
            public float sr;
            public string callsign;
            public bool overpower;
            public int max_strength;
            public float dbb;

            public Sig(int _fft_start, int _fft_stop, float _fft_centre, int _fft_strength, double _frequency, float _sr, bool _overpower, int _max_strength, float _dbb)
            {
                this.fft_start = _fft_start;
                this.fft_stop = _fft_stop;
                this.fft_centre = _fft_centre;
                this.text_pos = _fft_centre;
                this.fft_strength = _fft_strength;
                this.frequency = _frequency;
                this.sr = _sr;
                this.callsign = "";
                this.overpower = _overpower;
                this.max_strength = _max_strength;
                this.dbb = _dbb;
            }

            public Sig(Sig old, string _callsign)
            {
                this.fft_start = old.fft_start;
                this.fft_stop = old.fft_stop;
                this.fft_centre = old.fft_centre;
                this.text_pos = old.text_pos;
                this.fft_strength = old.fft_strength;
                this.frequency = old.frequency;
                this.sr = old.sr;
                this.callsign = _callsign;
                this.overpower = old.overpower;
                this.max_strength = old.max_strength;
                this.dbb = old.dbb;
            }

            public Sig(Sig old, float _text_pos, string _callsign, double _frequency, float _sr)
            {
                this.fft_start = old.fft_start;
                this.fft_stop = old.fft_stop;
                this.fft_centre = old.fft_centre;
                this.text_pos = _text_pos;
                this.fft_strength = old.fft_strength;
                this.frequency = _frequency;
                this.sr = _sr;
                this.callsign = _callsign;
                this.overpower = old.overpower;
                this.max_strength = old.max_strength;
                this.dbb = old.dbb;
            }

            public Sig(Sig old, string _callsign, double _frequency, float _sr)
            {
                this.fft_start = old.fft_start;
                this.fft_stop = old.fft_stop;
                this.fft_centre = old.fft_centre;
                this.text_pos = old.text_pos;
                this.fft_strength = old.fft_strength;
                this.frequency = _frequency;
                this.sr = _sr;
                this.callsign = _callsign;
                this.overpower = old.overpower;
                this.max_strength = old.max_strength;
                this.dbb = old.dbb;
            }

            public void updateCallsign(string _callsign)
            {
                this.callsign = _callsign;
            }

        }

        public Action<string> debug;

        Object list_lock;
        public List<Sig> newSignals = new List<Sig>();      // list of signals new arrived from websocket
        public List<Sig> signals = new List<Sig>();         // actual list of signals (processed)
        private const double start_freq = 10490.4754901;
        float minsr = 0.065f;
        int num_rx_scan = 1;
        int num_rx = 1;

        public signal(object _list_lock)
        {
            list_lock = _list_lock;
        }

        bool avoid_beacon = false;

        private Sig[] last_sig = new Sig[8];             //last tune signal - detail
        private Sig[] next_sig = new Sig[8];             //last tune signal - detail

        private DateTime[] last_tuned_time = new DateTime[8];   //time the last signal was tuned

        public void set_avoidbeacon(bool b)
        {
            avoid_beacon = b;
        }

        public void set_tuned(Sig s, int rx)
        {
            last_sig[rx] = s;
        }

        public void set_minsr(float _minsr)
        {
            minsr = _minsr;
        }

        public void set_num_rx(int _num_rx)
        {
            num_rx = _num_rx;
        }

        public void set_num_rx_scan(int _num_rx_scan)
        {
            num_rx_scan = _num_rx_scan;
        }

        public void clear(int rx)
        {
            last_sig[rx] = new Sig();
        }

        //function to find out whether to change tuning and which signal to tune to - auto tune mode
        public Tuple<Sig, int> tune(int mode, int time, int rx)
        {
            // int rx = 0;
            bool change = false;
            //mode
            //0=manual
            //1=auto wait
            //2=auto timed
            //Log.Information(rx);
            TimeSpan t = DateTime.Now - last_tuned_time[rx];

            lock (list_lock)
            {
                if (mode == 2)      //auto timed
                {
                    //Log.Information(t.Seconds);
                    if ((t.Minutes * 60) + t.Seconds > time)
                    {
                        //Log.Information("elapsed: " + rx.ToString());
                        next_sig[rx] = find_next(rx);

                        if (diff_signals(last_sig[rx], next_sig[rx]) && next_sig[rx].frequency > 0)       //check if next is not the same as current
                        {
                            change = true;
                        }
                    }
                    else
                    {
                        if (!find_signal(last_sig[rx], rx))      //if the selected signal goes off then find another one to tune to
                        {
                            next_sig[rx] = find_next(rx);

                            if (diff_signals(last_sig[rx], next_sig[rx]) && next_sig[rx].frequency > 0)       //check if next is not the same as current
                            {
                                change = true;
                            }
                        }
                    }
                }
                else
                {
                    if (!find_signal(last_sig[rx], rx))  //if the selected signal goes off then find another one to tune to
                    {
                        next_sig[rx] = find_next(rx);

                        if (diff_signals(last_sig[rx], next_sig[rx]) && next_sig[rx].frequency > 0)       //check if next is not the same as current
                        {
                            change = true;
                        }
                    }
                }

                // Log.Information("Count3:" + newSignals.Count().ToString());
                if (change)
                {
                    last_sig[rx] = next_sig[rx];
                    last_tuned_time[rx] = DateTime.Now.AddSeconds(rx);
                    return new Tuple<Sig, int>(last_sig[rx], rx);
                }
                else
                {
                    return new Tuple<Sig, int>(new Sig(), rx);
                }
            }
        }

        public bool find_signal(Sig lastsig, int rx)
        {
            float span;
            bool found = false;
            int n = 0;

            foreach (Sig s in newSignals)
            {
                if (s.sr < 0.070f)
                {
                    span = 0.01f;
                }
                else
                {
                    span = 0.05f;
                }
                if (s.frequency > (last_sig[rx].frequency - span) && s.frequency < (last_sig[rx].frequency + span) && s.sr == last_sig[rx].sr)      // +/- span, signal freq varies!
                {
                    found = true;
                }
                n++;
            }
            return found;
        }

        public bool diff_signals(Sig lastsig, Sig next)
        {
            // Log.Information(lastsig.frequency-next.frequency);
            float span;
            bool diff = true;
            if (lastsig.sr < 0.070f | next.sr < 0.070f)
            {
                span = 0.01f;
            }
            else
            {
                span = 0.075f;
            }
            if (next.frequency > (lastsig.frequency - span) && next.frequency < (lastsig.frequency + span) && next.sr == lastsig.sr)      // +/- span, signal freq varies!
            {
                diff = false;
            }
            return diff;        //returns false if they are the same
        }

        public bool diff_signals(Sig sig, double frequency, float sr)
        {
            //debug(sig.frequency.ToString() + "," + frequency.ToString());
            //Log.Information(lastsig.frequency-next.frequency);
            float span;
            bool diff = true;
            if (sig.sr < 0.070f | sr < 0.070f)
            {
                span = 0.01f;
            }
            else
            {
                span = 0.075f;
            }
            if (frequency > (sig.frequency - span) && frequency < (sig.frequency + span))      // +/- span, signal freq varies!
            {
                diff = false;
            }
            return diff;        //returns false if they are the same
        }

        public void updateSignalList()
        {
            // check current signal list
            //foreach (Sig s in signals.ToList())
            for (int x = 0; x < signals.Count; x++)
            {
                bool found = false;

                foreach (Sig sl in newSignals)
                {
                    if (diff_signals(signals[x], sl) == false) // same sig
                    {
                        signals[x] = new Sig(sl, signals[x].text_pos, signals[x].callsign, signals[x].frequency, signals[x].sr);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //debug("Removing a signal");
                    signals.Remove(signals[x]);
                }
            }

            // check new signal list
            foreach (Sig s in newSignals)
            {
                bool found = false;

                foreach (Sig sl in signals)
                {
                    if (diff_signals(s, sl) == false) // same sig
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //debug("Adding a signal");
                    signals.Add(s);
                }
            }
        }

        private Sig find_next(int rx)
        {
            Sig newsig = new Sig();
            int n = 0;
            double startfreq;
            if (avoid_beacon)
            {
                startfreq = 10492.0;
            }
            else
            {
                startfreq = 10490.5;
            }

            //Log.Information("Rx:" + rx.ToString() + " Current Tuned:" + last_sig[rx].frequency);
            foreach (Sig s in newSignals)
            {
                //Log.Information("Rx:" + rx.ToString() + " 1st Try:" + s.frequency.ToString());
                bool newfreq = true;
                for (int i = 0; i < num_rx; i++)
                {
                    if (!diff_signals(s, last_sig[i]))
                    {
                        newfreq = false;
                    }
                }
                //Log.Information("Available=" + newfreq.ToString());
                if (s.frequency > startfreq && s.frequency > (last_sig[rx].frequency + 0.05) && s.sr >= minsr && newfreq)      // +/- span, signal freq varies!
                {
                    newsig = s;
                    break;
                }
                n++;
                if (newsig.frequency > 0)
                    break;

            }
            if (newsig.frequency < 1)       //nothing available above last freq, return to bottom and start again until orig signal freq
            {
                foreach (Sig s in newSignals)
                {
                    //Log.Information("Rx:"+rx.ToString()+" 2nd Try:" + s.frequency.ToString());
                    bool newfreq = true;
                    for (int i = 0; i < num_rx; i++)
                    {
                        if (!diff_signals(s, last_sig[i]))
                        {
                            newfreq = false;
                        }
                    }
                    //Log.Information("Available="+newfreq.ToString());
                    if (s.frequency > startfreq && s.frequency < (last_sig[rx].frequency - 0.05) && s.sr >= minsr && newfreq)
                    {
                        newsig = s;
                        break;
                    }

                }
            }
            //Log.Information("new=:" + newsig.frequency.ToString());
            return newsig;
        }

        public void updateCurrentSignal(string callsign, double freq, float sr)
        {
            lock (list_lock)
            {
                for (int x = 0; x < signals.Count; x++)
                {
                    if (diff_signals(signals[x], freq, sr) == false)
                    {
                        //debug("updateCurrentSignal: found! x: " + x.ToString() +", Call: " + callsign + "QRG: " + freq.ToString() + ", SR: " + sr.ToString());
                        signals[x] = new Sig(signals[x], Convert.ToSingle(Math.Round((freq - start_freq) * 102.0f, 1)), callsign, freq, sr);
                        break;
                    }
                }
            }
        }

        public bool isOverPower(int beacon_strength, int signal_strength, float signal_bw)
        {
            if (beacon_strength != 0)
            {
                if (signal_bw < 0.4f)
                {
                    return false;
                }

                if (signal_strength > Math.Round(beacon_strength - (0.75 * 3276.8)))
                {
                    return true;
                }
            }
            else
            {
                Log.Information("Beacon Strength = 0");
            }
            return false;
        }

        // Entrypoint with new fft_data from drawspectrum
        public List<Sig> detect_signals(UInt16[] fft_data)
        {
            lock (list_lock)
            {
                newSignals.Clear();
                int i;
                int j;

                int noise_level = 11000;
                int signal_threshold = 16000;

                Boolean in_signal = false;
                int start_signal = 0;
                int end_signal;
                double mid_signal;
                int strength_signal;
                float signal_bw;
                double signal_freq;
                int acc;
                int acc_i;

                for (i = 2; i < fft_data.Length; i++)
                {
                    if (!in_signal)
                    {
                        if ((fft_data[i] + fft_data[i - 1] + fft_data[i - 2]) / 3.0f > signal_threshold)
                        {
                            in_signal = true;
                            start_signal = i;
                        }
                    }
                    else /* in_signal == true */
                    {
                        if ((fft_data[i] + fft_data[i - 1] + fft_data[i - 2]) / 3.0f < signal_threshold)
                        {
                            in_signal = false;

                            end_signal = i;

                            acc = 0;
                            acc_i = 0;
                            //Log.Information(Math.Round((start_signal + (0.3f * (end_signal - start_signal)))).ToString());
                            //Log.Information(Math.Round((start_signal + (0.8f * (end_signal - start_signal)))).ToString());

                            for (j = Convert.ToInt16(Math.Round(start_signal + (0.3f * (end_signal - start_signal)))) | 0; j < Math.Round(start_signal + (0.8f * (end_signal - start_signal))); j++)
                            {
                                acc = acc + fft_data[j];
                                acc_i = acc_i + 1;
                            }

                            strength_signal = acc / acc_i;

                            /* Find real start of top of signal */
                            for (j = start_signal; (fft_data[j] - noise_level) < 0.75f * (strength_signal - noise_level); j++)
                            {
                                start_signal = j;
                            }

                            /* Find real end of the top of signal */
                            for (j = end_signal; (fft_data[j] - noise_level) < 0.75f * (strength_signal - noise_level); j--)
                            {
                                end_signal = j;
                            }

                            mid_signal = start_signal + ((end_signal - start_signal) / 2.0);

                            signal_bw = align_symbolrate((end_signal - start_signal) * 8.9995f / fft_data.Length);
                            signal_freq = Math.Round((start_freq + mid_signal / fft_data.Length * 8.9995),3);

                            //Log.Information("Start   :" + start_signal.ToString());
                            //Log.Information("Middle  :" + mid_signal.ToString());
                            //Log.Information("End     :" + end_signal.ToString());
                            //Log.Information("Strength:" + strength_signal.ToString());
                            //Log.Information("QRG     :" + signal_freq.ToString());
                            //Log.Information("BW      :" + signal_bw.ToString() + "\n");

                            // Exclude signals in beacon band
                            if (signal_bw >= 0.033)
                            {
                                if (signal_freq < 10492000.0 && signal_bw > 1.0f)
                                {
                                    beacon_strength = strength_signal;
                                    newSignals.Add(new Sig(start_signal, end_signal, Convert.ToSingle(mid_signal), strength_signal, signal_freq, signal_bw, false, 0, 0));
                                }
                                else
                                {
                                    bool overpower = false;
                                    int max_strength = (int)(beacon_strength - (0.75 * 3276.8));
                                    //
                                    // The original dBb calculation code used at the QO-100 wideband spectrum web site is complicated:
                                    // 
                                    // -------------------------------
                                    //
                                    //  canvasHeight = 542;
                                    //  var db_per_pixel;
                                    //  var beacon_strength_pixel;
                                    //
                                    //  db_per_pixel = ((canvasHeight * 7/8) - (canvasHeight / 12)) / 15; // 15dB screen window
                                    //  beacon_strength_pixel = canvasHeight - ((beacon_strength / 65536) * canvasHeight);
                                    // 
                                    //  signal[x].top = (canvasHeight-((strength_signal/65536) * canvasHeight);
                                    //  dBb = ((beacon_strength_pixel - signal[x].top) / db_per_pixel).toFixed(1);
                                    //
                                    // -------------------------------
                                    //
                                    // I (DL1RF) have simplified the calculation by eliminating the 'canvasHeight' dependency.
                                    // With canvasHeight = 1 the db_per_pixel will calculate as ((1 * 7/8) - (1 / 12)) / 15 = 0.0527777...
                                    //
                                    // Life comparing the dBb values calculated here with the values at QO-100 wideband spectrum
                                    // show sometimes a diviation of -0.1dB.
                                    //
                                    // But this is the same while using the original calculation.
                                    //
                                    // Additional tests may be necessary to enshure that the simplified calcualtion is right.
                                    //
                                    float dBb = -1.0f * (((float)(beacon_strength - strength_signal) / 65536.0f) / 0.052778f);

                                    if (isOverPower(beacon_strength, strength_signal, signal_bw))
                                        overpower = true;

                                    newSignals.Add(new Sig(start_signal, end_signal, Convert.ToSingle(mid_signal), strength_signal, signal_freq, signal_bw, overpower, max_strength, dBb));
                                }
                            }


                        }
                    }
                }
                updateSignalList();
            }
            return newSignals;
        }

        public float align_symbolrate(float width)
        {
            if (width < 0.022f)
            {
                return 0;
            }
            else if (width < 0.060f)
            {
                return 0.035f;
            }
            else if (width < 0.086f)
            {
                return 0.066f;
            }
            else if (width < 0.185f)
            {
                return 0.125f;
            }
            else if (width < 0.277f)
            {
                return 0.250f;
            }
            else if (width < 0.388f)
            {
                return 0.333f;
            }
            else if (width < 0.700f)
            {
                return 0.500f;
            }
            else if (width < 1.2f)
            {
                return 1.000f;
            }
            else if (width < 1.6f)
            {
                return 1.500f;
            }
            else if (width < 2.2f)
            {
                return 2.000f;
            }
            else
            {
                return Convert.ToSingle(Math.Round(width * 5.0) / 5.0);
            }
        }
    }
}
