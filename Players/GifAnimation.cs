using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Picsie.Players
{
    class GifAnimation
    {
        //Static fields
        private static int defaultInterval = 100;
        private static double speedFactorStep = 0.05;

        //Properties
        private bool animated = false;
        public bool Animated
        {
            get { return animated; }
        }

        //Fields
        private double speedFactor = 1.0;
        private int frames = 0;
        private int frame = 0;
        private FrameDimension dimension;
        private Image image;
        private Form1 parent;

        private System.Windows.Forms.Timer ticker;

        public GifAnimation(string filename, Image image, Form1 parent)
        {
            this.parent = parent;
            this.image = image;
            FileInfo info = new FileInfo(filename);
            if (Managers.FileManager.IsGif(info.Extension))
            {
                dimension = new FrameDimension(image.FrameDimensionsList[0]);
                frames = image.GetFrameCount(dimension);
                animated = frames > 1;
                if (animated)
                    initTicker();
            }
        }

        private void initTicker()
        {
            ticker = new System.Windows.Forms.Timer();
            ticker.Interval = GifAnimation.defaultInterval;
            ticker.Enabled = true;
            ticker.Tick += gifTicker_Tick;
        }

        public void Step(int step)
        {
            frame += step;
            if (frame < 0)
                frame = frames - 1;
            else if (frame >= frames)
                frame = 0;
            setFrame(dimension, frame);
        }

        private void setFrame(FrameDimension dimension, int frame)
        {
            image.SelectActiveFrame(dimension, frame);
            int frameDuration = defaultInterval;
            try
            {
                /*
                 * Updated: now using per-frame delay rather than one-off calculation.
                 * http://stackoverflow.com/a/14219338/376079
                 */
                frameDuration = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, (frame * 4)) * 10;
                if (frameDuration == 0) {
                    frameDuration = defaultInterval;
                }
            }
            catch
            {
                frameDuration = defaultInterval;
            }
            ticker.Interval = (int)Math.Round(speedFactor * (double)frameDuration);
        }

        private void gifTicker_Tick(object sender, EventArgs e)
        {
            Step(1);
            parent.Refresh();
        }

        public void SpeedUp()
        {
            if (speedFactor > speedFactorStep)
                speedFactor -= speedFactorStep;
        }

        public void SlowDown()
        {
            speedFactor += speedFactorStep;
        }

        public void Pause()
        {
            ticker.Enabled = !ticker.Enabled;
        }

        public void Reset()
        {
            speedFactor = 1.0;
            ticker.Enabled = true;
        }

        public void StepForward()
        {
            ticker.Enabled = false;
            Step(1);
        }

        public void StepBackward()
        {
            ticker.Enabled = false;
            Step(-1);
        }

        public void Close() {
            Console.WriteLine("Closing gif animation");
            if (ticker == null) {
                return;
            }
            ticker.Enabled = false;
            try {
                ticker.Tick -= gifTicker_Tick;
            } catch (Exception e) {
                Utility.OutError(e, "Error removing timing event for GIF ticker");
            }
            try {
                image.Dispose();
                ticker.Dispose();
            } catch (Exception e) {
                Utility.OutError(e, "Error disposing of animation resources");
            }
            ticker = null;
        }

    }
}
