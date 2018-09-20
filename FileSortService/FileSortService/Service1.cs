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

        List<FSWRedux> watcherList = new List<FSWRedux>();

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


            //string me = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Properties.Settings.Default.currentUser = Environment.UserName;
            LoadDirectories();


            


            activeCheck = true;
            timeMain.Enabled = true;

        }

        private void LoadDirectories()
        {
            StreamReader directories = new StreamReader("..\\..\\Directories.csv");
            FSWRedux newFSW;
            string currentLine = "", source = "", destination = "";
            bool validLine = false;
            string[] splitLine;

            while (!directories.EndOfStream)
            {
                currentLine = directories.ReadLine();
                validLine = false;

                splitLine = currentLine.Split(',');

                if (splitLine.Length == 3)
                {
                    if (splitLine[1].StartsWith("*."))
                    {
                        if (splitLine[0] != splitLine[2])
                        {
                            validLine = true;
                            source = splitLine[0].Trim().Trim('"');
                            destination = splitLine[2].Trim().Trim('"');
                        }

                    }

                }

                if (validLine)
                {
                    newFSW = CreateFileWatcher(source, destination);

                    if(newFSW != null)
                    {
                        newFSW.EnableRaisingEvents = true;
                        newFSW.Created += fswMain_Created;
                        newFSW.Changed += fswMain_Changed;
                        newFSW.Deleted += fswMain_Deleted;
                        newFSW.Renamed += fswMain_Renamed;

                        watcherList.Add(newFSW);
                    }
                }

            }

        }

        private FSWRedux CreateFileWatcher(string FileSource, string FileDestination)
        {
            FSWRedux newFSW = new FSWRedux(FileDestination);
            //string file = "";

            if (Directory.Exists(FileSource))
            {
                newFSW.Path = FileSource;
            }



            if (newFSW.Path.Length > 0)
            {
                return newFSW;
            }
            else
            {
                return null;
            }

        }

        protected override void OnStop()
        {
            activeCheck = false;

        }

        internal void TestStartandStop(string[] args)
        {
            this.OnStart(args);

            while (activeCheck)
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

            while (qDocument.Any())
            {
                Document curr = qDocument.Dequeue();
            }

            while (qGraphic.Any())
            {
                Graphic curr = qGraphic.Dequeue();
            }

            while (qVideo.Any())
            {
                Video curr = qVideo.Dequeue();
            }

            while (qAudio.Any())
            {
                Audio curr = qAudio.Dequeue();
            }


            while (qArchive.Any())
            {
                Archive curr = qArchive.Dequeue();
            }


            while (qFile.Any())
            {
                BaseFile curr = qFile.Dequeue();

            }

        }

        private string SortFile(string path, string extension)
        {
            string section = "";

            if (GraphicTypes.Contains(extension))
            {
                Graphic newFile = new Graphic(path);
                qGraphic.Enqueue(newFile);
                section = newFile.FileType;
            }
            else if (VideoTypes.Contains(extension))
            {
                Video newFile = new Video(path);
                qVideo.Enqueue(newFile);
                section = newFile.FileType;
            }
            else if (AudioTypes.Contains(extension))
            {
                Audio newFile = new Audio(path);
                qAudio.Enqueue(newFile);
                section = newFile.FileType;
            }
            else if (DocumentTypes.Contains(extension))
            {
                Document newFile = new Document(path);
                qDocument.Enqueue(newFile);
                section = newFile.FileType;
            }
            else if (ArchiveTypes.Contains(extension))
            {
                Archive newFile = new Archive(path);
                qArchive.Enqueue(newFile);
                section = newFile.FileType;
            }
            else
            {
                Document newFile = new Document(path, true);
                qDocument.Enqueue(newFile);
                section = newFile.FileType;
            }

            return section;
        }

        private string SortFile(string path, string extension, bool a)
        {
            string section = "";

            if (GraphicTypes.Contains(extension))
            {

                section = "Graphic";
            }
            else if (VideoTypes.Contains(extension))
            {

                section = "Video";
            }
            else if (AudioTypes.Contains(extension))
            {

                section = "Audio";
            }
            else if (DocumentTypes.Contains(extension))
            {

                section = "Document";
            }
            else if (ArchiveTypes.Contains(extension))
            {

                section = "Archive";
            }
            else
            {

                section = "Base File";
            }

            return section;
        }

        private void fswMain_Changed(object sender, FileSystemEventArgs e)
        {
            //Changed event does not fire for a straight-up deletion or a move away from the target directory. It also does not activate
            //two levels deep (i.e. it won't activate for changes made in "NewFolder" if "Desktop" is the target directory in 
            //"C:/Users/Me/Desktop/FolderStuff/NewFolder".
            FSWRedux creater = (FSWRedux)sender;
            if (e.FullPath != creater.DestinationDirectory)
            {



                string changedFile = e.FullPath;

                string ext = Path.GetExtension(changedFile);

                if (ext.Length < 1)
                {

                    string name = Path.GetFileName(changedFile);
                    string newFold = Path.Combine(Properties.Settings.Default.backupDirectory, name);
                    if (!Directory.Exists(newFold))
                    {
                        Directory.CreateDirectory(newFold);
                    }

                    string[] files = Directory.GetFiles(changedFile);
                    foreach (string file in files)
                    {

                        FileSystemEventArgs newEvent = new FileSystemEventArgs(WatcherChangeTypes.Changed, newFold, Path.GetFileName(file));
                        fswMain_Changed(sender, newEvent);
                    }

                }
                else
                {
                    if (Path.GetDirectoryName(e.FullPath) == Properties.Settings.Default.targetDirectory)
                    {
                        string section = SortFile(changedFile, ext, true);

                        string fileName = e.Name;

                        string checkPath = Path.Combine(Properties.Settings.Default.backupDirectory, section, fileName);

                        File.Copy(changedFile, checkPath, true);
                    }
                    else
                    {

                        string fileDirectory = Path.GetFileName(Path.GetDirectoryName(e.FullPath));
                        string origLocation = Path.Combine(Properties.Settings.Default.targetDirectory, fileDirectory, e.Name);
                        Path.GetDirectoryName(changedFile);
                        File.Copy(origLocation, changedFile, true);

                    }



                }
            }

        }

        private void fswMain_Created(object sender, FileSystemEventArgs e)
        {
            FSWRedux creater = (FSWRedux)sender;
            if (e.FullPath != creater.DestinationDirectory)
            {

                string createdFile = e.FullPath;

                string ext = Path.GetExtension(createdFile);
                if (ext.Length < 1)
                {

                    string name = Path.GetFileName(createdFile);
                    string newFold = Path.Combine(Properties.Settings.Default.backupDirectory, name);
                    if (!Directory.Exists(newFold))
                    {
                        Directory.CreateDirectory(newFold);
                    }

                    string[] files = Directory.GetFiles(createdFile);
                    foreach (string file in files)
                    {

                        FileSystemEventArgs newEvent = new FileSystemEventArgs(WatcherChangeTypes.Changed, newFold, Path.GetFileName(file));
                        fswMain_Changed(sender, newEvent);
                    }

                }
                else
                {
                    string section = SortFile(createdFile, ext);

                    string fileName = e.Name;

                    string checkPath = Path.Combine(Properties.Settings.Default.backupDirectory, section, fileName);

                    File.Copy(createdFile, checkPath, true);
                }
            }
        }

        //[STAThread]
        private void fswMain_Deleted(object sender, FileSystemEventArgs e)
        {

            FSWRedux creater = (FSWRedux)sender;
            if (e.FullPath != creater.DestinationDirectory)
            {

                string deletedFile = Path.GetFileName(e.FullPath);
                string ext = Path.GetExtension(deletedFile);
                string fileName = e.Name;

                if (ext.Length < 1)
                {
                    string foldPath = Path.Combine(Properties.Settings.Default.backupDirectory, fileName);
                    string textPath = Path.Combine(foldPath, fileName + " (Deleted)" + ".txt");
                    Directory.CreateDirectory(foldPath);
                    File.WriteAllText(textPath, e.FullPath + " was deleted or moved on " + DateTime.Now + ".");
                }
                else
                {
                    string section = SortFile(deletedFile, ext, true);

                    string checkPath = Path.Combine(Properties.Settings.Default.backupDirectory, section, fileName);
                    string deletePath = Path.Combine(Properties.Settings.Default.backupDirectory, section, fileName.Replace(ext, "") + " (Deleted)" + ext);

                    if (!File.Exists(checkPath))
                    {
                        File.WriteAllText(deletePath, e.FullPath + " was deleted or moved on " + DateTime.Now + ".");
                    }
                }

                //Properties.Settings.Default.backupDirectory;

                //Thread staThread = new Thread(Action.CreateShell);
                //staThread.Start();
                //Shell shell = new Shell();
                //Folder recycleBin = shell.NameSpace(10);

                //foreach (FolderItem2 newDeleteLocal in recycleBin.Items())
                //{

                //}

            }

        }



        private void fswMain_Renamed(object sender, RenamedEventArgs e)
        {
            FSWRedux creater = (FSWRedux)sender;
            if (e.FullPath != creater.DestinationDirectory)
            {

                string renamedFile = e.FullPath;

                string oldPath = e.OldFullPath;

                string ext = Path.GetExtension(renamedFile);

                if (ext.Length < 1)
                {
                    Directory.Move(oldPath, renamedFile);
                }
                else
                {
                    string section = SortFile(renamedFile, ext);

                    string fileName = e.Name;
                    string oldName = e.OldName;
                    string oldArcPath = Path.Combine(creater.DestinationDirectory, section, oldName);
                    string checkPath = Path.Combine(creater.DestinationDirectory, section, fileName);

                    if (File.Exists(oldArcPath))
                    {
                        File.Move(oldArcPath, checkPath);
                        File.Copy(renamedFile, checkPath, true);
                    }
                    else
                    {
                        File.Copy(renamedFile, checkPath, true);
                    }

                }

            }
        }
    }

    class FSWRedux : FileSystemWatcher
    {
        private string _destination;

        public FSWRedux(string DestinationDirectory)
        {
            _destination = DestinationDirectory;
        }

        public string DestinationDirectory
        {
            get { return _destination; }
            set { _destination = value; }
        }
    }


    //class Action
    //{
    //    internal static void CreateShell()
    //    {
    //        Shell shell = new Shell();
    //        Folder recycleBin = shell.NameSpace(10);

    //        foreach (FolderItem2 newDeleteLocal in recycleBin.Items())
    //        {

    //        }
    //    }
    //}
}
