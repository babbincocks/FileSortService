using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSortService
{
    public partial class FileSortService : ServiceBase
    {

        bool activeCheck;
        Queue<string> qDirectories = new Queue<string>();
        DateTime lastSearch;

        public List<string> GraphicTypes = new List<string>();
        public List<string> VideoTypes = new List<string>();
        public List<string> AudioTypes = new List<string>();
        public List<string> DocumentTypes = new List<string>();
        public List<string> ArchiveTypes = new List<string>();

        public FileSortService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            GraphicTypes.Add(".jpg");
            GraphicTypes.Add(".jpeg");
            GraphicTypes.Add(".png");
            GraphicTypes.Add(".bmp");
            GraphicTypes.Add(".tiff");

            VideoTypes.Add(".mp4");
            VideoTypes.Add(".wmv");
            VideoTypes.Add(".flv");
            VideoTypes.Add(".ogg");
            VideoTypes.Add(".avi");
            VideoTypes.Add(".webm");
            VideoTypes.Add(".gif");

            AudioTypes.Add(".mp3");
            AudioTypes.Add(".wav");
            AudioTypes.Add(".flac");
            AudioTypes.Add(".aiff");
            AudioTypes.Add(".aax");
            AudioTypes.Add(".aa");

            DocumentTypes.Add(".pdf");
            DocumentTypes.Add(".docx");
            DocumentTypes.Add(".txt");

            ArchiveTypes.Add(".zip");
            ArchiveTypes.Add(".rar");
            ArchiveTypes.Add(".7z");

            activeCheck = true;
            //string me = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Properties.Settings.Default.currentUser = Environment.UserName;
            Properties.Settings.Default.Save();
            
        }

        protected override void OnStop()
        {
            activeCheck = false;
        }

        internal void TestStartandStop(string[] args)
        {
            this.OnStart(args);

            while(timeMain.Enabled)
            {
                Console.ReadLine();
            }

            this.OnStop();
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeMain.Enabled = false;

            

            lastSearch = DateTime.Now;
            timeMain.Enabled = activeCheck;
            
        }

        private void fswMain_Changed(object sender, FileSystemEventArgs e)
        {
            string createdFile = e.FullPath;

            string a = e.ChangeType.ToString();
        }

        private void fswMain_Created(object sender, FileSystemEventArgs e)
        {
            string createdFile = e.FullPath;

            string a = e.ChangeType.ToString();
        }

        private void fswMain_Deleted(object sender, FileSystemEventArgs e)
        {
            string createdFile = e.FullPath;

            string a = e.ChangeType.ToString();
        }

        private void fswMain_Renamed(object sender, RenamedEventArgs e)
        {
            string createdFile = e.FullPath;

            string a = e.ChangeType.ToString();
        }
    }
}
