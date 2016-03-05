using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Picsie.Messages
{
    class ErrorMessage : MessageScreen
    {
        public ErrorMessage(string filename)
        {
            FileInfo info = new FileInfo(filename);
            this.pen = new Pen(Color.FromArgb(255, 192, 192));
            this.brush = new SolidBrush(Color.White);
            this.background = Color.FromArgb(128, 0, 0);
            this.message = "Error loading file: " + Environment.NewLine + "  " + info.Name;
        }
    }
}
