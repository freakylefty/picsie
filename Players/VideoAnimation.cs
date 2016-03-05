using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;

namespace Picsie.Players
{
    class VideoAnimation
    {
        //Static fields
        private static bool Muted = false;
        private static int initVolume = 1;

        //Properties
        private bool isPlayable = false;
        public bool Playable
        {
            get { return isPlayable; }
        }

        public Size Size
        {
            get { return getSize(); }
        }

        private Panel parent;
        private Video videoPlayer;
        private Size size;

        public VideoAnimation(string filename, Panel parent)
        {
            this.parent = parent;
            FileInfo info = new FileInfo(filename);
            if (Managers.FileManager.IsVideo(info.Extension))
            {
                try
                {
                    isPlayable = true;
                    videoPlayer = new Video(filename);
                    this.size = videoPlayer.Size;
                    if (initVolume > 0)
                        initVolume = videoPlayer.Audio.Volume;
                    setVolume();
                    videoPlayer.Owner = parent;
                }
                catch (Exception e)
                {
                    Utility.OutError(e, "Could not load video: " + filename);
                    isPlayable = false;
                    this.Dispose();
                }
            }
        }

        public bool IsStopped()
        {
            return videoPlayer.Stopped || videoPlayer.CurrentPosition >= videoPlayer.Duration;
        }

        public void Play()
        {
            if (!videoPlayer.Playing)
                videoPlayer.Play();
        }

        public void Pause()
        {
            if (!videoPlayer.Paused)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }

        public void ToggleMute()
        {
            Muted = !Muted;
            setVolume();
            /*if (videoPlayer.Audio.Volume == initVolume)
                videoPlayer.Audio.Volume = -10000;
            else
                videoPlayer.Audio.Volume = initVolume;*/
        }

        private void setVolume()
        {
            if (Muted)
                videoPlayer.Audio.Volume = -10000;
            else
                videoPlayer.Audio.Volume = initVolume;
        }

        public void Reset()
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }

        public void Dispose()
        {
            if (videoPlayer != null)
                videoPlayer.Dispose();
            videoPlayer = null;
        }

        private Size getSize()
        {
            if (isPlayable)
                return size;
            return Config.MessageSize;
        }
    }
}
