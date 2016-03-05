using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Picsie.Messages
{
    abstract class MessageScreen
    {
        protected SolidBrush brush;
        protected Pen pen;
        protected Font font;
        protected Color background;
        protected string message;
        protected readonly Rectangle messageBox = new Rectangle(Config.Offset, Config.Offset, Config.MessageSize.Width - (2 * Config.Offset), Config.MessageSize.Height - (2 * Config.Offset));

        public void Render(Graphics graphics)
        {
            font = new Font("Arial", 12, FontStyle.Regular);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            graphics.Clear(background);
            graphics.DrawString(message, font, brush, messageBox, format);

            graphics.DrawRectangle(pen, 1, 1, Config.MessageSize.Width - 3, Config.MessageSize.Height - 3);
        }
    }
}
