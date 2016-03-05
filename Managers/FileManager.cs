using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Picsie.Messages;
using Picsie.Players;
using Picsie.Utils;

namespace Picsie.Managers
{
    class FileManager
    {
        //Enums
        public enum MediaTypes { Image, Video, Animation, None, Broken, Pending }
        public enum LoadStatus { Success, Fail };

        //Properties
        private MessageScreen errorBox, loadBox;

        private MediaTypes mediaType = MediaTypes.None;
        public MediaTypes MediaType
        {
            get { return mediaType; }
        }

        private GifAnimation gifPlayer;
        public GifAnimation Gif
        {
            get { return gifPlayer; }
        }

        private VideoAnimation videoPlayer;
        public VideoAnimation Video
        {
            get { return videoPlayer; }
        }

        private string filename = "";
        public string Filename
        {
            get { return filename; }
        }

        //Fields
        private Image image = null;
        private Panel videoPanel;
        private Form1 parent;


        //Constructor
        public FileManager(Form1 parent, Panel videoPanel)
        {
            this.parent = parent;
            this.videoPanel = videoPanel;
        }

        //Code
        public bool Load(string filename)
        {
            if (IsValidFile(filename))
                return loadFile(filename, false);
            return false;
        }

        public void loadLarge()
        {
            loadFile(filename, true);
        }

        public static bool IsValidFile(string filename) {
            if (!File.Exists(filename))
                return false;
            FileInfo info = new FileInfo(filename);
            if (getMediaType(info.Extension) == MediaTypes.None)
                return false;
            return true;
        }

        public static bool ShouldLoadFile(FileInfo file) {
            return !(file.Name.StartsWith("._") && file.Length <= 4096);
        }

        private bool loadFile(string filename, bool loadLarge)
        {
            try
            {
                cleanUp();
                MediaTypes tempType = getMediaType(filename);
                if (!loadLarge && isLargeFile(filename))
                {
                    mediaType = MediaTypes.Pending;
                    loadBox = new LoadMessage(filename);
                }
                else if (tempType == MediaTypes.Video)
                    initVideo(filename);
                else if (tempType == MediaTypes.Animation || tempType == MediaTypes.Image)
                {
                    Image tempImg = new Bitmap(filename);
                    image = tempImg;
                    mediaType = MediaTypes.Image;
                    if (tempType == MediaTypes.Animation)
                        initGif(filename);
                }
                videoPanel.Visible = mediaType == MediaTypes.Video;
                this.filename = filename;
                return true;
            }
            catch (Exception e)
            {
                mediaType = MediaTypes.Broken;
                errorBox = new ErrorMessage(filename);
                image = null;
                this.filename = filename;
                Utility.OutError(e, "Error loading " + filename);
                return false;
            }
        }

        private bool isLargeFile(string filename)
        {
            FileInfo info = new FileInfo(filename);
            return info.Length >= Config.largeFileSize;
        }

        private void cleanUp()
        {
            if (videoPlayer != null) {
                videoPlayer.Dispose();
                videoPlayer = null;
            }
            if (gifPlayer != null) {
                gifPlayer.Close();
                gifPlayer = null;
            }
            mediaType = MediaTypes.None;
        }

        private void initGif(string filename)
        {
            gifPlayer = new GifAnimation(filename, image, parent);
            if (gifPlayer.Animated)
                mediaType = FileManager.MediaTypes.Animation;
            else {
                gifPlayer.Close();
                gifPlayer = null;
            }
        }

        private void initVideo(string filename)
        {
            videoPlayer = new VideoAnimation(filename, videoPanel);
            if (videoPlayer.Playable)
            {
                mediaType = MediaTypes.Video;
                videoPlayer.Play();
            }
            else
            {
                mediaType = MediaTypes.Broken;
                errorBox = new ErrorMessage(filename);
            }
        }

        public Image getImage()
        {
            if (mediaType == MediaTypes.Image || mediaType == MediaTypes.Animation)
                return image;
            throw new Exception("Cannot return an image for this type of media: " + mediaType);
        }

        public Size getMediaSize()
        {
            if (mediaType == MediaTypes.Animation || mediaType == MediaTypes.Image)
                return image.Size;
            if (mediaType == MediaTypes.Video)
                return videoPlayer.Size;
            return Config.MessageSize;
        }

        public void pokeVideo()
        {
            if (MediaType == MediaTypes.Video && Video.IsStopped())
                videoPlayer.Play();
        }

        public void Render(Graphics graphics, Size targetSize)
        {
            if (MediaType == MediaTypes.Video) return;
            if (MediaType == MediaTypes.Broken)
            {
                errorBox.Render(graphics);
                return;
            }
            if (MediaType == FileManager.MediaTypes.Pending)
            {
                loadBox.Render(graphics);
                return;
            }
            graphics.DrawImage(getImage(), new Rectangle(new Point(0, 0), targetSize));
        }

        private List<string> getFileList()
        {
            List<string> files = new List<string>();
            FileInfo info = new FileInfo(Filename);

            DirectoryInfo dir = new DirectoryInfo(info.DirectoryName);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (IsValidFile(file.FullName) && ShouldLoadFile(file))
                    files.Add(file.FullName);
            }
            if (files.Count <= 1)
            {
                Utility.Out("No more files");
                return null;
            }
            files.Sort(new NaturalStringComparer());
            return files;
        }

        public string getNextFile() {
            List<string> files = getFileList();
            if (files == null) return "";
            int index = files.IndexOf(Filename);
            index++;
            if (index == files.Count)
                index = 0;
            return files[index];
        }

        public string getNextBatch() {
            List<string> files = getFileList();
            if (files == null) return "";
            Regex partRe = new Regex(@"(^.+) ([0-9]+)\.([^.]+)$");
            Match fileMatch = partRe.Match(filename);
            if (!fileMatch.Success)
                return getNextFile(); // Not currently in a batch, just go to next file
            bool allMatch = doAllMatch(files, fileMatch.Groups[1].Value);
            if (allMatch)
                return getNextFile();

            int index = files.IndexOf(Filename);
            return getStartOfNextBatch(index, files, fileMatch.Groups[1].Value);
        }

        private static string getStartOfNextBatch(int index, List<string> files, string partName) {
            int currIndex = index + 1;
            while (currIndex != index) {
                if (currIndex >= files.Count)
                    currIndex = 0;
                var currFile = files[currIndex];
                Regex partRe = new Regex(partName.Replace(@"\", @"\\") + @" ([0-9]+)\.([^.]+)$");
                Match fileMatch = partRe.Match(currFile);
                if (fileMatch.Success == false)
                    return currFile;
                currIndex++;
            }
            return files[index]; // Shouldn't happen, need a safe fallback
        }

        public string getPreviousBatch() {
            List<string> files = getFileList();
            if (files == null) return "";
            Regex partRe = new Regex(@"(^.+) ([0-9]+)\.([^.]+)$");
            Match fileMatch = partRe.Match(filename);
            if (!fileMatch.Success) {
                // Current file not in a batch, so look at previous file
                string prev = getPreviousFile();
                fileMatch = partRe.Match(prev);
                if (!fileMatch.Success)
                    return prev; //Previous file not a batch either, go to it
                //Previous file in a batch, go to the start of it
                string prevStart = getFirstInBatch(files, fileMatch.Groups[1].Value);
                if (prevStart == null)
                    return prev; //Somehow null?
                return prevStart;
            }
            //Currently in a batch
            bool allMatch = doAllMatch(files, fileMatch.Groups[1].Value);
            // If only one batch, just go back one
            if (allMatch)
                return getPreviousFile();
            //If we're not at the start go there first
            string firstInBatch = getFirstInBatch(files, fileMatch.Groups[1].Value);
            if (firstInBatch != null && firstInBatch != filename)
                return firstInBatch;
            //Were at the start of a batch, go to the start of the one before
            

            int index = files.IndexOf(Filename);
            return getStartOfPreviousBatch(index, files, fileMatch.Groups[1].Value);
        }

        private static string getStartOfPreviousBatch(int index, List<string> files, string partName) {
            int prevBatchEndIndex = getEndIndexOfPreviousBatch(index, files, partName);
            if (prevBatchEndIndex < 0)
                return files[index]; // Shouldn't happen, but you never know
            Regex partRe = new Regex(@"(^.+) ([0-9]+)\.([^.]+)$");
            Match fileMatch = partRe.Match(files[prevBatchEndIndex]);
            if (!fileMatch.Success)
                return files[prevBatchEndIndex]; // Previous file not part of a batch
            string prevStartFile = getFirstInBatch(files, fileMatch.Groups[1].Value);
            if (prevStartFile == null)
                return files[prevBatchEndIndex]; // Somehow couldn't match?
            return prevStartFile;
        }

        private static string getFirstInBatch(List<string> files, string partName) {
            foreach(string curr in files) {
                Regex partRe = new Regex(partName.Replace(@"\", @"\\") + @" ([0-9]+)\.([^.]+)$");
                Match fileMatch = partRe.Match(curr);
                if (fileMatch.Success == true)
                    return curr;
            }
            return null;
        }

        private static int getEndIndexOfPreviousBatch(int index, List<string> files, string partName) {
            int currIndex = index - 1;
            while (currIndex != index) {
                if (currIndex < 0)
                    currIndex = files.Count - 1;
                var currFile = files[currIndex];
                Regex partRe = new Regex(partName.Replace(@"\", @"\\") + @" ([0-9]+)\.([^.]+)$");
                Match fileMatch = partRe.Match(currFile);
                if (fileMatch.Success == false)
                    return currIndex;
                currIndex--;
            }
            return -1; // Shouldn't happen, need a safe fallback
        }

        private static bool doAllMatch(List<string> files, string name) {
            if (files.Count == 1)
                return true;
            foreach(var curr in files) {
                Regex partRe = new Regex(name.Replace(@"\", @"\\") + @" ([0-9]+)\.([^.]+)$");
                Match fileMatch = partRe.Match(curr);
                if (fileMatch.Success == false)
                    return false;
            }
            return true;
        }

        public string getPreviousFile()
        {
            List<string> files = getFileList();
            if (files == null) return "";
            int index = files.IndexOf(Filename);
            index--;
            if (index < 0)
                index = files.Count - 1;
            return files[index];
        }

        public string getFirstFile()
        {
            List<string> files = getFileList();
            if (files == null) return "";
            int index = files.IndexOf(Filename);
            if (index == 0)
            {
                Utility.Out("Already on first file");
                return "";
            }
            return files[0];
        }

        public string getLastFile()
        {
            List<string> files = getFileList();
            if (files == null) return "";
            int index = files.IndexOf(Filename);
            int count = files.Count - 1;
            if (index == count)
            {
                Utility.Out("Already on last file");
                return "";
            }
            return files[count];
        }

        public void close()
        {
            if (Video != null)
                Video.Dispose();
        }

        //Static Methods
        public static MediaTypes getMediaType(string filename)
        {
            FileInfo info = new FileInfo(filename);
            string ext = info.Extension.ToLower();
            if (Config.VideoExtensions.Contains(ext))
                return MediaTypes.Video;
            if (ext == ".gif")
                return MediaTypes.Animation;
            if (Config.Extensions.Contains(ext))
                return MediaTypes.Image;
            return MediaTypes.None;
        }

        public static bool IsGif(string ext)
        {
            return ext.ToLower() == ".gif";
        }

        public static bool IsVideo(string ext)
        {
            return Config.VideoExtensions.Contains(ext.ToLower());
        }

        public static bool IsImage(string ext)
        {
            return Config.Extensions.Contains(ext.ToLower()) && !Config.VideoExtensions.Contains(ext.ToLower());
        }
    }
}
