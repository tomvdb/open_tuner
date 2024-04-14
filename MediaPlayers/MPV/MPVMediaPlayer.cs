using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static opentuner.MediaPlayers.MPV.LibMpv;

namespace opentuner.MediaPlayers.MPV
{
    public class MPVMediaPlayer : OTMediaPlayer
    {

        public delegate Int64 MyStreamCbReadFn(IntPtr cookie, IntPtr buf, Int64 numbytes);
        public delegate Int64 MyStreamCbSeekFn(IntPtr cookie, Int64 offset);
        public delegate void MyStreamCbCloseFn(IntPtr cookie);
        public delegate Int64 MyStreamCbSizeFn(IntPtr cookie);

        private IntPtr handler_ptr;
        private MyStreamCbOpenFn handler;

        private MyStreamCbReadFn readfn;
        private MyStreamCbCloseFn closefn;


        private string _videoPlayerId = "";
        private string _videoPlayerTitle = "";

        Int64 counter = 0;

        private CircularBuffer _videoBuffer;

        bool stopFlag = false;

        public override event EventHandler<MediaStatus> onVideoOut;
        private IntPtr _mpvHandle;

        private Int64 _videoViewHandle;
        private long _volume = 100;

        private int _id = 0;

        public override string GetName()
        {
            return "MPV";
        }

        private void debug(string msg)
        {
            Log.Information("MPVMediaPlayer: " + msg);
        }

        public void startEventLoop()
        {
            Task.Run(() => {
                try
                {
                    EventLoop();
                }
                catch (Exception e)
                {
                    debug("Eventloop Exception : " + e.ToString());
                }
            });

        }

        public int GetPropertyInt(string propertyName)
        {
            mpv_get_property(_mpvHandle, GetUtf8Bytes(propertyName), mpv_format.MPV_FORMAT_INT64, out IntPtr buffer);
            return buffer.ToInt32();
        }

        public string GetPropertyString(string propertyName)
        {
            mpv_error error = mpv_get_property(_mpvHandle, GetUtf8Bytes(propertyName), mpv_format.MPV_FORMAT_STRING, out IntPtr buffer);

            if (error == mpv_error.MPV_ERROR_SUCCESS)
            {
                string ret = ConvertFromUtf8(buffer);
                mpv_free(buffer);
                return ret;
            }

            return "";
        }


        public void EventLoop()
        {
            while (true)
            {

                if (_mpvHandle == IntPtr.Zero)
                    break;

                IntPtr ptr = LibMpv.mpv_wait_event(_mpvHandle, -1);
                mpv_event evt = (mpv_event)Marshal.PtrToStructure(ptr, typeof(mpv_event));

                try
                {
                    switch (evt.event_id)
                    {
                        case (mpv_event_id.MPV_EVENT_VIDEO_RECONFIG):
                            //reconfigureVideo();
                            break;

                        case (mpv_event_id.MPV_EVENT_LOG_MESSAGE):
                            var logdata = (mpv_event_log_message)Marshal.PtrToStructure(evt.data, typeof(mpv_event_log_message));
                            debug(ConvertFromUtf8(logdata.text).Trim());
                            break;

                        case (mpv_event_id.MPV_EVENT_PLAYBACK_RESTART):
                            MediaStatus info = new MediaStatus();

                            int width = GetPropertyInt("dwidth");
                            int height = GetPropertyInt("dheight");

                            info.VideoHeight = (uint)height;
                            info.VideoWidth = (uint)width;

                            //string data = GetPropertyString("audio-device-list");
                            string data = "";

                            //data = GetPropertyString("video-format");
                            
                            data = GetPropertyString("video-codec");
                            info.VideoCodec = data;
                            data = GetPropertyString("audio-codec-name");
                            info.AudioCodec = data;
                            data = GetPropertyString("audio-device");
                            debug(data);
                            data = GetPropertyString("audio-params/samplerate");
                            uint.TryParse(data, out info.AudioRate);

                            onVideoOut?.Invoke(this,info);
                            break;
                        default: debug("MPV Event: " + evt.event_id.ToString()); break;
                    }
                }
                catch (Exception Ex)
                {
                    debug(Ex.ToString());
                }
            }

            debug("Closing Event Loop");

        }

        public MPVMediaPlayer(Int64 VideoViewHandle)
        {
            handler = StreamCBOpenFN;
            handler_ptr = Marshal.GetFunctionPointerForDelegate(handler);
            _videoViewHandle = VideoViewHandle;
        }

        bool ts_sync = false;

        Int64 MyStreamReadFn(IntPtr cookie, IntPtr buf, Int64 numbytes)
        {
            if (stopFlag == true)
                return 0;

            try
            {
                int timeout = 0;

                while (_videoBuffer.Count < 2000)
                {

                    if (stopFlag == true)
                    {
                        //Log.Information("Stop Requested");
                        return 0;
                    }

                    if (timeout > 5000)
                    {
                        Log.Information("MyStream : Read Timeout");
                        return 0;
                    }

                    Thread.Sleep(5);
                    timeout += 5;
                }

                int queue_count = _videoBuffer.Count;

                if (queue_count > 0)
                {
                    //RawTSData raw_ts_data = null;
                    byte raw_ts_data = 0;

                    Int64 buildLen = numbytes;

                    if (queue_count < buildLen)
                    {
                        buildLen = queue_count;
                    }

                    byte[] ts_data = new byte[buildLen];

                    int counter = 0;

                    while (counter < buildLen)
                    {
                        if (_videoBuffer.Count > 0)
                        {
                            raw_ts_data = _videoBuffer.Dequeue();


                            if (ts_sync == false && raw_ts_data != 0x47)
                            {
                                buildLen--;
                                continue;
                            }
                            else
                            {
                                ts_sync = true;
                                ts_data[counter++] = raw_ts_data;
                            }
                        }
                        else
                        {
                            Log.Information("Warning: Failing to dequeue, nothing to dequeue: TSStream");
                        }
                    }

                    Marshal.Copy(ts_data.ToArray(), 0, buf, ts_data.Length);
                    return ts_data.Length;

                }
            }
            catch (Exception ex)
            {
                Log.Information("Stream Read Callback Exception: " + ex.Message);
            }

            Log.Information("TS StreamInput: Shouldn't be here");

            return 0;
        }

        void MyStreamCloseFn(IntPtr cookie)
        {
            debug("Close Callback Called");
        }


        int StreamCBOpenFN(String userdata, String uri, ref MPV_STREAM_CB_INFO info)
        {
            debug("StreamCBOpenFN Called");
            //debug(userdata);
            //debug(uri);

            readfn = MyStreamReadFn;
            closefn = MyStreamCloseFn;

            info.ReadFn = Marshal.GetFunctionPointerForDelegate(readfn);
            info.CloseFn = Marshal.GetFunctionPointerForDelegate(closefn);

            return 0;
        }

        public static IntPtr AllocateUtf8IntPtrArrayWithSentinel(string[] arr, out IntPtr[] byteArrayPointers)
        {
            int numberOfStrings = arr.Length + 1; // add extra element for extra null pointer last (sentinel)
            byteArrayPointers = new IntPtr[numberOfStrings];
            IntPtr rootPointer = Marshal.AllocCoTaskMem(IntPtr.Size * numberOfStrings);
            for (int index = 0; index < arr.Length; index++)
            {
                var bytes = LibMpv.GetUtf8Bytes(arr[index]);
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
                byteArrayPointers[index] = unmanagedPointer;
            }
            Marshal.Copy(byteArrayPointers, 0, rootPointer, numberOfStrings);
            return rootPointer;
        }

        private void DoMpvCommand(params string[] args)
        {
            IntPtr[] byteArrayPointers;
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out byteArrayPointers);
            LibMpv.mpv_command(_mpvHandle, mainPtr);
            foreach (var ptr in byteArrayPointers)
            {
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(mainPtr);
        }

        public override void Close()
        {
            //if (_mpvHandle != IntPtr.Zero)
            //    LibMpv.mpv_destroy(_mpvHandle);
        }

        public override int GetVolume()
        {
            return (int)_volume;
        }

        public override void Initialize(CircularBuffer TSDataQueue)
        {
            _videoBuffer = TSDataQueue;
        }

        public override void Play()
        {
            counter = 0;
            ts_sync = false;
            stopFlag = false;

            if (_mpvHandle != IntPtr.Zero)
            {
                debug("Destroy");
                LibMpv.mpv_destroy(_mpvHandle);
            }

            _mpvHandle = LibMpv.mpv_create();

            debug("start event loop");
            startEventLoop();


            mpv_initialize(_mpvHandle);
            mpv_request_log_messages(_mpvHandle, "error");
            mpv_set_option_string(_mpvHandle, LibMpv.GetUtf8Bytes("keep-open"), LibMpv.GetUtf8Bytes("always"));
            //var windowID = _videoView.Handle.ToInt64();
            var windowID = _videoViewHandle;
            LibMpv.mpv_set_option(_mpvHandle, LibMpv.GetUtf8Bytes("wid"), LibMpv.mpv_format.MPV_FORMAT_INT64, ref windowID);

            //mpv_stream_cb_add_ro(_mpvHandle, "myprotocol", "", handler_ptr);
            mpv_stream_cb_add_ro(_mpvHandle, "myprotocol", "", handler);

            _videoBuffer.Clear();

            DoMpvCommand("loadfile", "myprotocol://fake");
            //DoMpvCommand("loadfile", "udp://127.0.0.1:4003");        }
        }

        public override void SetVolume(int Volume)
        {
            _volume = Volume;
            if (_mpvHandle != IntPtr.Zero)
            {
                try
                {
                    LibMpv.mpv_set_option(_mpvHandle, LibMpv.GetUtf8Bytes("volume"), mpv_format.MPV_FORMAT_INT64, ref _volume);
                }
                catch (Exception ex)
                {
                    Log.Information("Error setting volume for MediaPlayer MPV: " + ex.Message);
                }
            }
        }

        public override void SnapShot(string FileName)
        {
            Log.Information("MPV Snapshot: " + Path.GetDirectoryName(FileName) + "\\");

            LibMpv.mpv_set_option_string(_mpvHandle, LibMpv.GetUtf8Bytes("screenshot-directory"), LibMpv.GetUtf8Bytes(Path.GetDirectoryName(FileName) + "\\"));
            LibMpv.mpv_set_option_string(_mpvHandle, LibMpv.GetUtf8Bytes("screenshot-template"), LibMpv.GetUtf8Bytes("ot_mpv_%n"));
            LibMpv.mpv_set_option_string(_mpvHandle, LibMpv.GetUtf8Bytes("screenshot-format"), LibMpv.GetUtf8Bytes("png"));
            DoMpvCommand("screenshot", "video");
        }

        public override void Stop()
        {
            stopFlag = true;
        }

        public override int getID()
        {
            return _id;
        }

        public override void Initialize(CircularBuffer TSDataQueue, int ID)
        {
            _id = ID;
            Initialize(TSDataQueue);
        }
    }
}
