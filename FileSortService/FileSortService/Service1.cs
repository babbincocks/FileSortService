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
        Queue<Video> qVideo = new Queue<Video>();
        Queue<Audio> qAudio = new Queue<Audio>();
        Queue<Graphic> qGraphic = new Queue<Graphic>();
        Queue<Document> qDocument = new Queue<Document>();
        Queue<Archive> qArchive = new Queue<Archive>();
        Queue<BaseFile> qFile = new Queue<BaseFile>();
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
            

            fswMain.Filter = "*.*";
            fswMain.Path = Properties.Settings.Default.targetDirectory;
            
            timeMain.Enabled = true;
            
        }

        protected override void OnStop()
        {
            activeCheck = false;
            Properties.Settings.Default.Save();
        }

        internal void TestStartandStop(string[] args)
        {
            this.OnStart(args);

            while(activeCheck)
            {
                
            }

            this.OnStop();
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timeMain.Enabled = false;

            CommitFileChanges(Properties.Settings.Default.backupDirectory);

            lastSearch = DateTime.Now;
            timeMain.Enabled = true;
            
        }

        private void CommitFileChanges(string targetDirectory)
        {

        }

        private string SortFile(string path,string extension)
        {
            string section = "";

            if (GraphicTypes.Contains(extension))
            {
                Graphic newFile = new Graphic(path);
                section = newFile.FileType;
            }
            else if (VideoTypes.Contains(extension))
            {
                Video newFile = new Video(path);
                section = newFile.FileType;
            }
            else if (AudioTypes.Contains(extension))
            {
                Audio newFile = new Audio(path);
                section = newFile.FileType;
            }
            else if (DocumentTypes.Contains(extension))
            {
                Document newFile = new Document(path);
                section = newFile.FileType;
            }
            else if (ArchiveTypes.Contains(extension))
            {
                Archive newFile = new Archive(path);
                section = newFile.FileType;
            }
            else
            {
                Document newFile = new Document(path, true);
                section = newFile.FileType;
            }

            return section;
        }

        private void fswMain_Changed(object sender, FileSystemEventArgs e)
        {
            string createdFile = e.FullPath;
            
            string ext = Path.GetExtension(createdFile);
            string section = SortFile(createdFile, ext);

            string fileName = e.Name;

            WatcherChangeTypes changeType = e.ChangeType;
            //TODO: Check if Changed will fire off if a deletion or anything like that is done.
            if (changeType == WatcherChangeTypes.Changed)
            {
                string checkPath = Path.Combine(Properties.Settings.Default.backupDirectory, section, fileName);

                File.Copy(createdFile, checkPath, true);
            }

            
        }

        private void fswMain_Created(object sender, FileSystemEventArgs e)
        {
            string createdFile = e.FullPath;

            string ext = Path.GetExtension(createdFile);

            string section = SortFile(createdFile, ext);

            string a = e.ChangeType.ToString();
        }

        private void fswMain_Deleted(object sender, FileSystemEventArgs e)
        {
            string deletedFile = Path.GetFileName(e.FullPath);
            
            //Properties.Settings.Default.backupDirectory;
            

            string ext = Path.GetExtension(deletedFile);

            string checkPath = Path.Combine();

            if(!File.Exists(checkPath))
            {
                //TODO: Set it up so a text file is created in place of whatever file is deleted.
            }
            else
            {

            }

            SortFile(deletedFile, ext);

            string a = e.ChangeType.ToString();
        }

        private void fswMain_Renamed(object sender, RenamedEventArgs e)
        {
            string createdFile = e.FullPath;

            string ext = Path.GetExtension(createdFile);

            

            string a = e.ChangeType.ToString();
        }
    }
}
