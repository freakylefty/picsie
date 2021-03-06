using System;
using System.Drawing;

namespace Picsie
{
    class Utility
    {
        public static string Pad(string Str, int Width, string With)
        {
            while (Str.Length < Width)
                Str = With + Str;
            return Str;
        }

        public static void OutError(Exception ex)
        {
            OutError(ex, "");
        }

        public static void OutError(Exception ex, string message)
        {
            string text = "+-" + ex.GetType() + "---------";
            Console.WriteLine(text);
            if (message.Length > 0) Console.WriteLine("| " + message.Replace("" + Environment.NewLine, Environment.NewLine + "| "));
            Console.WriteLine("| {0}", ex.Message);
            Console.WriteLine("| {0}", ex.StackTrace.Replace("\n", "\n| "));
            Console.WriteLine("+{0}", Pad("-", text.Length - 1, "-"));
        }

        public static void Out(string S)
        {
            System.Diagnostics.Debug.WriteLine("--> " + S);
        }

        public static int average(int a, int b)
        {
            return (int)Math.Round(((double)a + (double)b) / 2.0);
        }

        public static Size limitTo(Size current, Size max)
        {
            if (current.Width > max.Width || current.Height > max.Height)
            {
                double factor = Math.Max((double)current.Width / (double)max.Width, (double)current.Height / (double)max.Height);
                current.Width = (int)Math.Round(current.Width / factor);
                current.Height = (int)Math.Round(current.Height / factor);
            }
            return current;
        }

        public static int Distance(Point a, Point b)
        {
            return (int)Math.Round(Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
        }
    }
}


