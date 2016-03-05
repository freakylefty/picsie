using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Picsie.Messages
{
    class LoadMessage : MessageScreen
    {
        public LoadMessage(string filename)
        {
            FileInfo info = new FileInfo(filename);
            this.pen = new Pen(Color.FromArgb(128, 128, 192));
            this.brush = new SolidBrush(Color.White);
            this.background = Color.FromArgb(0, 0, 16);
            this.message = "Very large file for a very small viewer: " + Environment.NewLine + "  " + info.Name + Environment.NewLine + Environment.NewLine + "Press return to load anyway";
        }
    }
}
