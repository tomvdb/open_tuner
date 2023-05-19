namespace opentuner
{
    partial class VideoViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vlcPlayer = new LibVLCSharp.WinForms.VideoView();
            this.ffmpegPlayer = new FlyleafLib.Controls.WinForms.FlyleafHost();
            ((System.ComponentModel.ISupportInitialize)(this.vlcPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // vlcPlayer
            // 
            this.vlcPlayer.BackColor = System.Drawing.Color.Black;
            this.vlcPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vlcPlayer.Location = new System.Drawing.Point(0, 0);
            this.vlcPlayer.MediaPlayer = null;
            this.vlcPlayer.Name = "vlcPlayer";
            this.vlcPlayer.Size = new System.Drawing.Size(800, 450);
            this.vlcPlayer.TabIndex = 0;
            this.vlcPlayer.Text = "videoView1";
            this.vlcPlayer.Visible = false;
            // 
            // ffmpegPlayer
            // 
            this.ffmpegPlayer.AllowDrop = true;
            this.ffmpegPlayer.AutoSize = true;
            this.ffmpegPlayer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ffmpegPlayer.BackColor = System.Drawing.Color.Black;
            this.ffmpegPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ffmpegPlayer.DragMove = true;
            this.ffmpegPlayer.IsFullScreen = false;
            this.ffmpegPlayer.KeyBindings = true;
            this.ffmpegPlayer.Location = new System.Drawing.Point(0, 0);
            this.ffmpegPlayer.Name = "ffmpegPlayer";
            this.ffmpegPlayer.OpenOnDrop = false;
            this.ffmpegPlayer.PanMoveOnCtrl = true;
            this.ffmpegPlayer.PanRotateOnShiftWheel = true;
            this.ffmpegPlayer.PanZoomOnCtrlWheel = true;
            this.ffmpegPlayer.Player = null;
            this.ffmpegPlayer.Size = new System.Drawing.Size(800, 450);
            this.ffmpegPlayer.SwapDragEnterOnShift = true;
            this.ffmpegPlayer.SwapOnDrop = true;
            this.ffmpegPlayer.TabIndex = 1;
            this.ffmpegPlayer.ToggleFullScreenOnDoubleClick = true;
            this.ffmpegPlayer.Visible = false;
            // 
            // VideoViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.ffmpegPlayer);
            this.Controls.Add(this.vlcPlayer);
            this.Name = "VideoViewForm";
            this.Text = "VideoViewForm";
            ((System.ComponentModel.ISupportInitialize)(this.vlcPlayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public LibVLCSharp.WinForms.VideoView vlcPlayer;
        public FlyleafLib.Controls.WinForms.FlyleafHost ffmpegPlayer;
    }
}