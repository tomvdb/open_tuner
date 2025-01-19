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
            public float fft_centre;    // signal center position in spectrum
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
        }

        public Action<string> debug;

        Object list_lock;
        public List<Sig> newSignals = new List<Sig>();      // list of signals new arrived from websocket
        public List<Sig> signals = new List<Sig>();         // actual list of signals (processed)

        private const double start_freq = 10490.4754901;
        float minsr = 0.033f;
        int num_rx = 1;

        public signal(object _list_lock)
        {
            list_lock = _list_lock;
        }

        public int compareFrequency(double frequency1, double frequency2, float sr)
        {
            float span;

            if (sr < 0.070f)
                span = 0.01f;
            else
                span = 0.075f;

            if ((frequency1 + span) < (frequency2 - span))      // +/- span, signal freq varies!
            {
                //Log.Information("compareFrequency: -1, " + frequency1.ToString() + ", " + frequency2.ToString());
                return -1;
            }
            else if ((frequency1 - span) > (frequency2 + span)) // +/- span, signal freq varies!
            {
                //Log.Information("compareFrequency: 1, " + frequency1.ToString() + ", " + frequency2.ToString());
                return 1;
            }
            //Log.Information("compareFrequency: 0, " + frequency1.ToString() + ", " + frequency2.ToString());
            return 0;
        }

        private int signalCompare(Sig s1, Sig s2)
        {
            if (0 == compareFrequency(s1.frequency, s2.frequency, Math.Max(s1.sr, s2.sr)))
            {
                return 0;
            }
            else if (0 < compareFrequency(s1.frequency, s2.frequency, Math.Max(s1.sr, s2.sr)))
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private void print_Signal(Sig _sig, int index, string name)
        {
            Log.Information(name.ToString() + "[" + index.ToString() + "]");
            Log.Information(" fft_start    : " + _sig.fft_start.ToString());
            Log.Information(" fft_centre   : " + _sig.fft_centre.ToString());
            Log.Information(" fft_stop     : " + _sig.fft_stop.ToString());
            Log.Information(" fft_strength : " + _sig.fft_strength.ToString());
            Log.Information(" frequency    : " + _sig.frequency.ToString());
            Log.Information(" sr           : " + _sig.sr.ToString());
            Log.Information(" callsign     : " + _sig.callsign.ToString());
            Log.Information(" text_pos     : " + _sig.text_pos.ToString());
            Log.Information(" max_strength : " + _sig.max_strength.ToString());
            Log.Information(" overpower    : " + _sig.overpower.ToString());
            Log.Information(" dbb          : " + _sig.dbb.ToString());
            Log.Information("");
        }

        public void set_minsr(float _minsr)
        {
            minsr = _minsr;
        }

        public void set_num_rx(int _num_rx)
        {
            num_rx = _num_rx;
        }

        public bool signalLost(double frequency, float sr, bool avoidBeacon)
        {
            bool found = false;

            if (avoidBeacon && frequency < 10492.0)
                return !found;

            lock (list_lock)
            {
                foreach (Sig s in signals)
                {
                    if (0 == compareFrequency(s.frequency, frequency, sr))
                    {
                        if (s.sr == sr)
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
            return !found;
        }

        private Sig compareSignalFrequencies(Sig old_signal, Sig new_signal, double own_freq)
        {
            double absDiffOld2Own = Math.Abs(own_freq - old_signal.frequency);
            double absDiffNew2Own = Math.Abs(own_freq - new_signal.frequency);
            if (absDiffNew2Own < absDiffOld2Own)
                return new_signal;
            else
                return old_signal;
        }

        public Sig findSameSignal(double frequency, float sr, bool avoidBeacon, float treshHold)
        {
            Sig newSig = new Sig();

            lock (list_lock)
            {
                foreach (Sig s in signals)
                {
                    if (!avoidBeacon || s.frequency >= 10492.0)
                    {
                        if (s.dbb >= treshHold)
                        {
                            if (0 == compareFrequency(s.frequency, frequency, Math.Max(s.sr, sr)))
                            {
                                newSig = s;
                                break;
                            }
                        }
                    }
                }
            }
            return newSig;
        }

        public Sig findNextNearestSignal(List<RX_Sig> inuse, double frequency, float sr, bool avoidBeacon, float treshHold)
        {
            Sig newSig = new Sig();
            List<Sig> notTunedSignals = new List<Sig>();

            lock (list_lock)
            {
                foreach (Sig s in signals)
                {
                    bool skip = false;
                    // create signal list of not tuned signals
                    foreach (RX_Sig skipSignal in inuse)
                    {
                        if (0 == compareFrequency(s.frequency, skipSignal.frequency, skipSignal.sr))
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        notTunedSignals.Add(s);
                    }
                }
            }
            if (notTunedSignals.Count > 0)
            {
                // first search for signal with same frequency
                foreach (Sig notTunedSignal in notTunedSignals)
                {
                    //Log.Information("not tuned Signal: " + notTunedSignal.frequency.ToString() + ", " + notTunedSignal.sr.ToString());
                    if (!avoidBeacon || notTunedSignal.frequency >= 10492.0)
                    {
                        if (notTunedSignal.dbb >= treshHold)
                        {
                            if (0 == compareFrequency(notTunedSignal.frequency, frequency, sr))
                            {
                                return notTunedSignal;
                            }
                        }
                    }
                }
                // second search for next nearest signal
                foreach (Sig notTunedSignal in notTunedSignals)
                {
                    //Log.Information("not tuned Signal: " + notTunedSignal.frequency.ToString() + ", " + notTunedSignal.sr.ToString());
                    if (!avoidBeacon || notTunedSignal.frequency >= 10492.0)
                    {
                        if (notTunedSignal.dbb >= treshHold)
                        {
                            newSig = compareSignalFrequencies(newSig, notTunedSignal, frequency);
                        }
                    }
                }
            }
            return newSig;
        }

        public Sig findNextSignalTimed(double frequency, float sr, bool avoidBeacon, float treshHold)
        {
            bool found = false;
            Sig newSig = new Sig();

            lock (list_lock)
            {
                foreach (Sig s in signals)
                {
                    if (0 < compareFrequency(s.frequency, frequency, sr))
                    {
                        if (!avoidBeacon || s.frequency >= 10492.0)
                        {
                            if (s.dbb >= treshHold)
                            {
                                found = true;
                                newSig = s;
                                break;
                            }
                        }
                    }
                }
            }
            if (!found)
            {
                newSig = signals[0];
                if (avoidBeacon)
                {
                    lock (list_lock)
                    {
                        foreach (Sig s in signals)
                        {
                            if (0 < compareFrequency(s.frequency, newSig.frequency, newSig.sr))
                            {
                                if (!avoidBeacon || s.frequency >= 10492.0)
                                {
                                    if (s.dbb >= treshHold)
                                    {
                                        found = true;
                                        newSig = s;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return newSig;
        }

        public bool diff_signals(Sig lastsig, Sig next)
        {
            //debug("diff_signals(): last QRG: " + lastsig.frequency.ToString() + ", next QRG: " + next.frequency.ToString());
            //debug("diff_signals(): last SR: " + lastsig.sr.ToString() + ", next SR: " + next.sr.ToString());

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
            if (next.frequency > (lastsig.frequency - span) && next.frequency < (lastsig.frequency + span) && align_symbolrate(next.sr) == lastsig.sr)      // +/- span, signal freq varies!  align_symbol new SR as it may vary!
            {
                diff = false;
            }
            return diff;        //returns false if they are the same
        }

        public bool diff_signals(Sig sig, double frequency, float sr)
        {
            //debug(sig.frequency.ToString() + "," + frequency.ToString());
            //Log.Information((sig.frequency - frequency).ToString());
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
            int x;
            // check current signal list
            //foreach (Sig s in signals.ToList())
            for (x = 0; x < signals.Count; x++)
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
                    print_Signal(signals[x], x, "Removing signal");
                    signals.Remove(signals[x]);
                }
            }

            x = 0;
            // check new signal list
            foreach (Sig s in newSignals)
            {
                bool found = false;

                foreach (Sig sl in signals)
                {
                    if (diff_signals(s, sl) == false) // same sig
                    {
                        //debug("same signal");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    print_Signal(s, x, "Adding signal");
                    signals.Add(s);
                }
                x++;
            }
            signals.Sort(signalCompare);
        }

        public void updateCurrentSignal(string callsign, double freq, float _sr)
        {
            float sr = align_symbolrate(_sr);
            lock (list_lock)
            {
                for (int x = 0; x < signals.Count; x++)
                {
                    if (diff_signals(signals[x], freq, sr) == false)
                    {
                        //debug("updateCurrentSignal: found! Index: " + x.ToString() + ", Call: " + callsign + ", QRG: " + freq.ToString() + ", SR: " + sr.ToString());
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

                            signal_bw = align_symbolrate((end_signal - start_signal) * 9.0f / fft_data.Length);
                            signal_freq = Math.Round((start_freq + mid_signal / fft_data.Length * 9.0f),3);

                            // Exclude signals lower minsr
                            if (signal_bw >= minsr)
                            {
                                // Exclude signals in beacon band
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
                return 0.033f;
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
            else if (width < 0.7f)
            {
                return 0.500f;
            }
            else if (width < 0.85f)
            {
                return 0.750f;
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
