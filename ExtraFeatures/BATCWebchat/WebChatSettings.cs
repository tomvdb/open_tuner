using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner.ExtraFeatures.BATCWebchat
{
    public class WebChatSettings
    {
        // chat settings        
        public string sigreport_template = "SigReport: {SN}/{SP} - {DBM} - ({MER}) - {SR} - {FREQ}";
        public string nickname = "NONICK";
        public int chat_font_size = 12;
        public int gui_chat_width = 500;
        public int gui_chat_height = 500;
        public int gui_chat_x = 0;
        public int gui_chat_y = 0;
        public int gui_chat_windowstate = 0;
        public int gui_chat_seperator = 180;
    }
}
