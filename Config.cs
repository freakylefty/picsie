using System;
using System.Collections.Generic;
using System.Drawing;

namespace Picsie
{
    class Config
    {
        //Private fields
        private static readonly string[] imageExts = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tif", ".tiff" };
        private static readonly string[] videoExts = { ".avi", ".mpg", ".wmv" };
        
        //Settings
        public static readonly double BlurFactor = 0.5;
        public static readonly float ZoomStep = 0.04f;
        public static readonly float MinZoom = 0.05f;
        public static readonly Size MessageSize = new Size(480, 300);
        public static readonly Size MinSize = new Size(32, 32);
        public static readonly int Offset = 10;
        public static List<string> Extensions = new List<string>();
        public static List<string> VideoExtensions = new List<string>();
        public static readonly long largeFileSize = 10000000;

        //Configurable options
        public static readonly bool showOnTaskbar = false;
        public static bool blur = true;

        static Config()
        {
            Extensions = new List<string>();
            VideoExtensions = new List<string>();
            Extensions.AddRange(imageExts);
            Extensions.AddRange(videoExts);
            VideoExtensions.AddRange(videoExts);
            showOnTaskbar = getShowOnTaskbar();
        }

        private static bool getShowOnTaskbar() {
            OperatingSystem os = Environment.OSVersion;
            if (os.Platform == PlatformID.Win32NT)
            {
                Version vs = os.Version;
                //Vista is 6.0
                if ((vs.Major == 6 && vs.Minor > 0) || (vs.Major > 6)) {
                    return true;
                }
            }
            return false;
        }
    }
}
