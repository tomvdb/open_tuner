// From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    class signal
    {

        public int beacon_strength = -1;

        //struct for signal information
        public struct Sig
        {
            public int fft_start;
            public int fft_stop;
            public int fft_centre;
            public int fft_strength;
            public double frequency;
            public float sr;
            public string callsign;
            public bool overpower;
            public float dbb;
            public Sig(int _fft_start, int _fft_stop, int _fft_centre, int _fft_strength, double _frequency, float _sr, bool overpower, float _dbb)
            {
                this.fft_start = _fft_start;
                this.fft_stop = _fft_stop;
                this.fft_centre = _fft_centre;
                this.fft_strength = _fft_strength;
                this.frequency = _frequency;
                this.sr = _sr;
                this.callsign = "";
                this.overpower = overpower;
                this.dbb = _dbb;
            }

            public Sig(Sig old, string callsign)
            {
                this.fft_start = old.fft_start;
                this.fft_stop = old.fft_stop;
                this.fft_centre = old.fft_centre;
                this.fft_strength = old.fft_strength;
                this.frequency = old.frequency;
                this.sr = old.sr;
                this.callsign = callsign;
                this.overpower = old.overpower;
                this.dbb = old.dbb;
            }

            public void updateCallsign(string callsign)
            {
                this.callsign = callsign;
            }

        }

        public Action<string> debug;


        Object list_lock;
        public List<Sig> signals = new List<Sig>();  //list of signals found: 
        public List<Sig> signalsData = new List<Sig>();
        double start_freq = 10490.5f;
        float minsr = 0.065f;
        int num_rx_scan = 1;
        int num_rx = 1;

        Random rnd = new Random();

        public signal(object _list_lock)
        {
            list_lock = _list_lock;
            //init nothing!


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
            //Console.WriteLine(rx);
            TimeSpan t = DateTime.Now - last_tuned_time[rx];


            lock (list_lock)
            {

                if (mode == 2)      //auto timed
                {
                    //Console.WriteLine(t.Seconds);
                    if ((t.Minutes * 60) + t.Seconds > time)
                    {
                        //          Console.WriteLine("elapsed: "+rx.ToString());
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


                // Console.WriteLine("Count3:" + signals.Count().ToString());
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

            foreach (Sig s in signals)
            {
                if (s.sr < 0.070)
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
            // Console.WriteLine(lastsig.frequency-next.frequency);
            float span;
            bool diff = true;
            if (lastsig.sr < 0.070 | next.sr < 0.070)
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
            // Console.WriteLine(lastsig.frequency-next.frequency);
            float span;
            bool diff = true;
            if (sig.sr < 0.070 | sr < 0.070)
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
            //foreach (Sig s in signalsData.ToList())
            for (int x = 0; x < signalsData.Count; x++)
            {
                bool found = false;

                foreach (Sig sl in signals)
                {
                    if (diff_signals(signalsData[x], sl) == false) // same sig
                    {
                        signalsData[x] = new Sig(sl, signalsData[x].callsign);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //debug("Removing a signal");
                    signalsData.Remove(signalsData[x]);
                }
            }

            // check new signal list
            foreach (Sig s in signals)
            {
                bool found = false;

                foreach (Sig sl in signalsData)
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
                    signalsData.Add(s);
                }
            }




        }

        private Sig find_next(int rx)
        {

            Sig newsig = new Sig();
            int n = 0;
            float startfreq;
            if (avoid_beacon)
            {
                startfreq = 10492;
            }
            else
            {
                startfreq = startfreq = 10490; ;
            }

            //      Console.Write("Rx:" + rx.ToString() + " Current Tuned:" + last_sig[rx].frequency+"\n");
            foreach (Sig s in signals)
            {
                //         Console.Write("Rx:" + rx.ToString() + " 1st Try:" + s.frequency.ToString()+" ");
                bool newfreq = true;
                for (int i = 0; i < num_rx; i++)
                {
                    if (!diff_signals(s, last_sig[i]))
                    {
                        newfreq = false;
                    }
                }
                //          Console.Write("Available=" + newfreq.ToString() + " \n");
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
                foreach (Sig s in signals)
                {
                    //             Console.Write("Rx:"+rx.ToString()+" 2nd Try:" + s.frequency.ToString()+" ");
                    bool newfreq = true;
                    for (int i = 0; i < num_rx; i++)
                    {
                        if (!diff_signals(s, last_sig[i]))
                        {
                            newfreq = false;
                        }
                    }
                    //              Console.Write("Available="+newfreq.ToString()+" \n");
                    if (s.frequency > startfreq && s.frequency < (last_sig[rx].frequency - 0.05) && s.sr >= minsr && newfreq)
                    {
                        newsig = s;

                        break;
                    }

                }
            }
            //       Console.WriteLine("new=:" + newsig.frequency.ToString()+"\n");
            return newsig;
        }

        public void updateCurrentSignal(string callsign, double freq, float sr)
        {
            lock (list_lock)
            {
                for (int x = 0; x < signalsData.Count; x++)
                {
                    if (diff_signals(signalsData[x], freq, sr) == false)
                    {
                        debug("found - updating callsign");
                        signalsData[x] = new Sig(signalsData[x], callsign);
                        break;
                    }
                }
            }
        }

        public bool isOverPower(int beacon_strength, int signal_strength, float signal_bw)
        {
            if (beacon_strength != 0)
            {
                if (signal_bw < 0.4)
                {
                    return false;
                }

                if (signal_strength > (beacon_strength - (0.75 * 3276.8)))
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Beacon Strength = 0");
            }


            return false;
        }

        public List<Sig> detect_signals(UInt16[] fft_data)
        {
            lock (list_lock)
            {
                signals.Clear();
                int i;
                int j;

                int noise_level = 11000;
                int signal_threshold = 16000;

                Boolean in_signal = false;
                int start_signal = 0;
                int end_signal;
                float mid_signal;
                int strength_signal;
                float signal_bw;
                double signal_freq;
                int acc;
                int acc_i;

                for (i = 2; i < fft_data.Length; i++)
                {
                    if (!in_signal)
                    {
                        if ((fft_data[i] + fft_data[i - 1] + fft_data[i - 2]) / 3.0 > signal_threshold)
                        {
                            in_signal = true;
                            start_signal = i;

                        }
                    }
                    else /* in_signal == true */
                    {
                        if ((fft_data[i] + fft_data[i - 1] + fft_data[i - 2]) / 3.0 < signal_threshold)
                        {
                            in_signal = false;

                            end_signal = i;

                            acc = 0;
                            acc_i = 0;
                            // Console.WriteLine(Convert.ToInt16(start_signal + (0.3 * (end_signal - start_signal))));
                            //  Console.WriteLine( start_signal + (0.7 * (end_signal - start_signal)));

                            for (j = Convert.ToInt16(start_signal + (0.3 * (end_signal - start_signal))) | 0; j < start_signal + (0.8 * (end_signal - start_signal)); j++)
                            {
                                acc = acc + fft_data[j];
                                acc_i = acc_i + 1;
                            }


                            strength_signal = acc / acc_i;

                            /* Find real start of top of signal */
                            for (j = start_signal; (fft_data[j] - noise_level) < 0.75 * (strength_signal - noise_level); j++)
                            {
                                start_signal = j;
                            }


                            /* Find real end of the top of signal */
                            for (j = end_signal; (fft_data[j] - noise_level) < 0.75 * (strength_signal - noise_level); j--)
                            {
                                end_signal = j;
                            }
                            //  Console.WriteLine("Start:" + start_signal.ToString());
                            //  Console.WriteLine("End:" + end_signal.ToString());
                            // Console.WriteLine("Strength:" + strength_signal.ToString());
                            mid_signal = Convert.ToSingle(start_signal + ((end_signal - start_signal) / 2.0));

                            signal_bw = align_symbolrate(Convert.ToSingle((end_signal - start_signal) * (9.0 / (fft_data.Length))));
                            signal_freq = Convert.ToDouble(start_freq + (((mid_signal + 1) / (fft_data.Length)) * 9.0));


                            // Exclude signals in beacon band
                            if (signal_bw >= 0.033)
                            {
                                if (signal_freq < 10492000 && signal_bw >= 1.0)
                                {
                                    beacon_strength = strength_signal;
                                    signals.Add(new Sig(start_signal, end_signal, Convert.ToInt32(mid_signal), strength_signal / 255, signal_freq, signal_bw, false, 0));
                                }
                                else
                                {
                                    bool overpower = false;

                                    if (isOverPower(beacon_strength, strength_signal, signal_bw))
                                        overpower = true;

                                    signals.Add(new Sig(start_signal, end_signal, Convert.ToInt32(mid_signal), strength_signal / 255, signal_freq, signal_bw, overpower, 0));
                                }
                            }


                        }
                    }
                }
                updateSignalList();

            }
            return signals;
        }

        public float align_symbolrate(float width)
        {
            if (width < 0.022)
            {
                return 0;
            }
            else if (width < 0.065)
            {
                return 0.035f;
            }
            else if (width < 0.086)
            {
                return 0.066f;
            }
            else if (width < 0.195)
            {
                return 0.125f;
            }
            else if (width < 0.277)
            {
                return 0.250f;
            }
            else if (width < 0.388)
            {
                return 0.333f;
            }
            else if (width < 0.700)
            {
                return 0.500f;
            }
            else if (width < 1.2)
            {
                return 1.000f;
            }
            else if (width < 1.6)
            {
                return 1.500f;
            }
            else if (width < 2.2)
            {
                return 2.000f;
            }
            else
            {
                return Convert.ToSingle(Math.Round(width * 5) / 5.0);
            }
        }
    }
}
