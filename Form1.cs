using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Picsie.Managers;

namespace Picsie
{
    public partial class Form1 : Form
    {
        private FileManager fileManager;

        private Point mouseGrab = new Point(-1, -1);
        private Point imageGrabbed = new Point(-1, -1);
        private Point grabPoint = new Point(-1, -1);
        private bool grabbed = false;
        private Size maxSize;
        private Point formCentre;
        private float zoom = 1.0f;

        private const int CS_DROPSHADOW = 0x20000;

        public Form1() : this("") { }
        //public Form1() : this(@"C:\Users\Andy\Downloads\test 1.jpg") { }

        public Form1(string filename)
        {
            InitializeComponent();
            maxSize = getScreenArea();

            fileManager = new FileManager(this, VideoPanel);

            this.MouseWheel += new MouseEventHandler(Image_MouseWheel);
            FocusChecker.Enabled = true;
            this.ShowInTaskbar = Config.showOnTaskbar;
            if (!string.IsNullOrEmpty(filename))
                load(filename);
            else
                MessageBox.Show("Don't launch Picsie by itself.  Open media with it instead.");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private bool load(string filename)
        {
            return load(filename, false);
        }

        private bool load(string filename, bool loadLarge)
        {
            setToolTip("");
            if (!FileManager.IsValidFile(filename))
                return false;

            zoom = 1.0f;
            fileManager.Load(filename);
            if (loadLarge)
                fileManager.loadLarge();
            preScale();
            if (AppManager.IsSoleInstance())
                centreImage(false);
            else
                bringOnScreen();
            this.Refresh();
            formCentre = getFormCentre();
            setToolTip(filename);
            return true;
        }

        private void setToolTip(string text) 
        {
            if (VideoPanel.Visible)
            {
                ToolTip.SetToolTip(VideoPanel, text);
            }
            else
            {
                ToolTip.SetToolTip(this, text);
            }
        }

        private void preScale()
        {
            Utility.Out("Prescaling.");
            Size mediaSize = fileManager.getMediaSize();
            Utility.Out("Media size: " + mediaSize);
            this.Size = mediaSize;
            if (mediaSize.Width > maxSize.Width || mediaSize.Height > maxSize.Height)
            {
                Utility.Out("Too big, zooming out");
                zoom = (float) (Math.Min((float) maxSize.Width / (float) mediaSize.Width, (float) maxSize.Height / (float) mediaSize.Height));
                Utility.Out("Zoom set to " + zoom);
                zoomOut(false);
            }
            Utility.Out("Initial zoom: " + zoom);
        }

        private Rectangle getWorkingArea()
        {
            return Screen.GetWorkingArea(this);
        }

        private void bringOnScreen()
        {
            Rectangle rect = getWorkingArea();
            if (this.Left < rect.Left || this.Right > rect.Right || this.Top < rect.Top || this.Bottom > rect.Bottom)
                centreImage(false);
        }

        private void Image_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) zoomIn();
            if (e.Delta < 0) zoomOut(true);   
        }

        private bool doZoom(bool refresh)
        {
            if (fileManager.MediaType == FileManager.MediaTypes.Broken || fileManager.MediaType == FileManager.MediaTypes.Pending)
            {
                this.Size = Config.MessageSize;
                zoom = 1.0f;
                return true;
            }
            Size initSize = fileManager.getMediaSize();
            int newWidth = (int) Math.Round((float) initSize.Width * zoom);
            int newHeight = (int) Math.Round((float) initSize.Height * zoom);
            if (newWidth > maxSize.Width) {
                double factor = (double) newWidth / (double) maxSize.Width;
                newWidth = maxSize.Width;
                newHeight = (int) ((double) newHeight / factor);
            }
            if (newHeight > maxSize.Height) {
                double factor = (double) newHeight / (double) maxSize.Height;
                newHeight = maxSize.Height;
                newWidth = (int) ((double) newWidth / factor);
            }
            this.SuspendLayout();
            this.Width = newWidth;
            this.Height = newHeight;
            this.ResumeLayout();
            if (refresh)
                this.Refresh();
            Utility.Out(this.Width + "x" + this.Height + " @ x" + zoom);
            return true;
        }

        private void zoomIn()
        {
            if (!canZoomIn())
                return;
            Utility.Out("Can zoom in");
            zoom += Config.ZoomStep;
            bool zoomed = doZoom(true);
            if (!zoomed) {
                zoom -= Config.ZoomStep;
            }
        }

        private void zoomOut(bool refresh)
        {
            if (!canZoomOut())
                return;
            Utility.Out("Can zoom out");
            zoom -= Config.ZoomStep;
            doZoom(true);
        }

        private bool canZoomIn() {
            if (this.Width >= maxSize.Width || this.Height >= maxSize.Height)
                return false;
            return true;
        }

        private bool canZoomOut() {
            if (zoom < Config.MinZoom)
                return false;
            if (this.Width < Config.MinSize.Width && this.Height < Config.MinSize.Height)
                return false;
            return true;
        }

        private void Image_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                centreImage(true);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27) //Escape
                this.Close();
            if (e.KeyChar == 't')
                this.TopMost = !this.TopMost;
            if (e.KeyChar == 'b' || e.KeyChar == 'f')
                Config.blur = !Config.blur;
            if (e.KeyChar == 'c')
                centreImage(false);
            if (e.KeyChar == 'h' || e.KeyChar == '/' || e.KeyChar == '?')
                showHelp();
            if (fileManager.MediaType == FileManager.MediaTypes.Pending)
            {
                if (e.KeyChar == (char)13) //Return
                    load(fileManager.Filename, true);
            }
            else if (fileManager.MediaType == FileManager.MediaTypes.Animation)
            {
                if (e.KeyChar == '+' || e.KeyChar == '=')
                    fileManager.Gif.SpeedUp();
                if (e.KeyChar == '-')
                    fileManager.Gif.SlowDown();
                if (e.KeyChar == '0')
                    fileManager.Gif.Pause();
                if (e.KeyChar == (char)8) //Backspace
                    fileManager.Gif.Reset();
                if (e.KeyChar == ']')
                    fileManager.Gif.StepForward();
                if (e.KeyChar == '[')
                    fileManager.Gif.StepBackward();
            }
            else if (fileManager.MediaType == FileManager.MediaTypes.Video)
            {
                if (e.KeyChar == ' ')
                    fileManager.Video.Pause();
                if (e.KeyChar == (char)8) //Backspace
                    fileManager.Video.Reset();
                if (e.KeyChar == 'm')
                    fileManager.Video.ToggleMute();
            }
            this.Refresh();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Right && e.Control)
                loadNextBatch();
            else if (e.KeyCode == Keys.Left && e.Control)
                loadPreviousBatch();
            else if (e.KeyCode == Keys.Right)
                loadNextFile();
            else if (e.KeyCode == Keys.Left)
                loadPreviousFile();
            else if (e.KeyCode == Keys.Up)
                loadFirstFile();
            else if (e.KeyCode == Keys.Down)
                loadLastFile();
            else if (e.KeyCode == Keys.F1)
                showHelp();
        }

        private void showHelp()
        {
            System.Diagnostics.Process.Start("http://www.aburn.org.uk/picsie/");
        }

        private void centreImage(bool resize)
        {
            this.SuspendLayout();
            Rectangle workingArea = getWorkingArea();
            Point screenOffset = workingArea.Location;
            //Rectangle rect = Screen.GetWorkingArea(this);
            Size sizeToFit = Utility.limitTo(fileManager.getMediaSize(), workingArea.Size);

            //Resize
            if (resize)
                this.Size = sizeToFit;

            //Centre
            Point centre = new Point(Utility.average(workingArea.Left, workingArea.Right), Utility.average(workingArea.Top, workingArea.Bottom));
            centreOn(centre);
            formCentre = getFormCentre();

            //zoom = 1.0f;

            this.ResumeLayout();
            this.Refresh();

        }

        private void centreOn(Point centre)
        {
            this.Location = new Point(centre.X - Utility.average(0, this.Width), centre.Y - Utility.average(0, this.Height));
        }

        private void Image_MouseDown(object sender, MouseEventArgs e)
        {
            //Left button drags
            grabPoint = e.Location;
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                Image_MouseDoubleClick(sender, e);
            }
            else if (e.Button == MouseButtons.Left)
            {
                imageGrabbed = this.Location;
                mouseGrab = e.Location;
                mouseGrab.Offset(this.Location);
                grabbed = true;
            }
        }

        private void Image_MouseUp(object sender, MouseEventArgs e)
        {
            //Left button drags
            if (e.Button == MouseButtons.Left)
            {
                grabbed = false;
                formCentre = getFormCentre();
            }
            //Right button closes
            else if (e.Button == MouseButtons.Right)
            {
                if (Utility.Distance(grabPoint, e.Location) < 10)
                    this.Close();
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (grabbed)
            {
                Point mouseCurr = e.Location;
                mouseCurr.Offset(this.Location);
                int xOff = mouseCurr.X - mouseGrab.X;
                int yOff = mouseCurr.Y - mouseGrab.Y;
                if (xOff == 0 && yOff == 0) return;
                this.Location = new Point(imageGrabbed.X + xOff, imageGrabbed.Y + yOff);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            fileManager.Render(e.Graphics, this.Size);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (fileManager.MediaType == FileManager.MediaTypes.None || fileManager.MediaType == FileManager.MediaTypes.Broken)
                this.Close();
            formCentre = getFormCentre();
        }

        private Size getScreenArea()
        {
            Point p = new Point(0, 0);
            foreach (Screen s in Screen.AllScreens)
            {
                if (s.Bounds.Right > p.X)
                    p.X = s.Bounds.Right;
                if (s.Bounds.Bottom > p.Y)
                    p.Y = s.Bounds.Bottom;
            }
            return new Size(p);
        }

        private void FocusChecker_Tick(object sender, EventArgs e)
        {
            if (fileManager.MediaType == FileManager.MediaTypes.Broken)
            {
                this.Size = Config.MessageSize;
                this.Refresh();
            }
            fileManager.pokeVideo();
            if (ActiveForm == null)
                this.Opacity = Config.blur ? Config.BlurFactor : 1.0;
            else
                this.Opacity = 1.0;
        }

        private void loadNextBatch() {
            string file = fileManager.getNextBatch();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }
        private void loadPreviousBatch() {
            string file = fileManager.getPreviousBatch();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }

        private void loadNextFile()
        {
            string file = fileManager.getNextFile();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }

        private void loadPreviousFile()
        {
            string file = fileManager.getPreviousFile();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }

        private void loadFirstFile()
        {
            string file = fileManager.getFirstFile();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }

        private void loadLastFile()
        {
            string file = fileManager.getLastFile();
            if (!string.IsNullOrEmpty(file))
                load(file);
        }

        private Point getFormCentre()
        {
            return new Point(Utility.average(this.Left, this.Right), Utility.average(this.Top, this.Bottom));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            fileManager.close();
        }
    }
}
