namespace opentuner.Classes
{
    public abstract class OTSource
    {
        public delegate void VideoChangeCallback(int video_number, bool start);

        public OTSource() { }

        //public abstract void Initialize();
    }
}
