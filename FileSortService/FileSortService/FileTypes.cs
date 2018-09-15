using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TagLib;
using Ionic.Zip;


namespace FileSortService
{
    abstract class BaseFile
    {
        private string _name;
        private string _path;
        private ulong _fileSize;
        private DateTime _createDate;
        private DateTime _modDate;
        private bool _readOnly;
        private string _extension;
        private string _fileType;

        public BaseFile()
        {
            _name = "";
            _path = "";
            _fileSize = 0;
            _createDate = DateTime.MinValue;
            _modDate = DateTime.MinValue;
            _readOnly = false;
            _extension = "";
        }

        //public File(string name, string filePath, long fileSizeInBytes, DateTime createdDate, DateTime modifiedDate, bool readOnly)
        //{
        //    _name = name;
        //    _path = filePath;
        //    _fileSize = fileSizeInBytes;
        //    _createDate = createdDate;
        //    _modDate = modifiedDate;
        //    _readOnly = readOnly;
        //}

        public BaseFile(string filePath)
        {
            _path = filePath;
            string[] info = FullFileInfo(filePath);
            _name = info[0];
            _fileSize = ulong.Parse(info[1]);
            _createDate = DateTime.Parse(info[2]);
            _modDate = DateTime.Parse(info[3]);
            _readOnly = bool.Parse(info[4]);
            _extension = info[5];

        }

        public string Name
        {
            get { return _name ?? ""; }
        }

        public string Path
        {
            get { return _path ?? ""; }
        }

        public ulong FileSize
        {
            get { return _fileSize; }
        }

        public DateTime CreateDate
        {
            get { return (_createDate != null) ? _createDate : DateTime.MinValue; }
        }

        public DateTime ModifiedDate
        {
            get { return (_modDate != null) ? _modDate : DateTime.MinValue; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
        }

        public string Extension
        {
            get { return _extension ?? ""; }
        }

        public abstract string FileType
        {
            get;
        }
        

        private string[] FullFileInfo(string filePath)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);

            string[] aspects = new string[6];
            aspects[0] = file.Name.Replace(file.Extension, "");
            aspects[1] = file.Length.ToString();
            aspects[2] = file.CreationTime.ToLongDateString();
            aspects[3] = file.LastWriteTime.ToLongDateString();
            aspects[4] = file.IsReadOnly.ToString();
            aspects[5] = file.Extension;

            return aspects;
        }

        public abstract void ViewFile();

        public void MoveFile(string destination)
        {

        }


    }


    class Graphic : BaseFile
    {
        private string _resolution;



        public Graphic() : base()
        {
            _resolution = "0 x 0";
        }

        public Graphic(string filePath) : base(filePath)
        {
            string list = TagLib.FileTypes.AvailableTypes.ToString();
            // bmp in Taglib.Image.NoMetadata.File
            // png in TagLib.Png.File
            // tiff in TagLib.Tiff.File
            if (this.Extension.ToLower() == ".jpg" || this.Extension.ToLower() == ".jpeg")
            {
                TagLib.Jpeg.File pic = new TagLib.Jpeg.File(filePath);
                _resolution = pic.Properties.PhotoWidth.ToString() + " x " + pic.Properties.PhotoHeight.ToString();

            }
            else if (this.Extension.ToLower() == ".png")
            {
                TagLib.Png.File pic = new TagLib.Png.File(filePath);
                _resolution = pic.Properties.PhotoWidth.ToString() + " x " + pic.Properties.PhotoHeight.ToString();
            }
            else if (this.Extension.ToLower() == ".tif" || this.Extension.ToLower() == ".tiff")
            {
                TagLib.Tiff.File pic = new TagLib.Tiff.File(filePath);
                _resolution = pic.Properties.PhotoWidth.ToString() + " x " + pic.Properties.PhotoHeight.ToString();
            }
            else if (this.Extension.ToLower() == ".bmp")
            {
                TagLib.Image.NoMetadata.File pic = new TagLib.Image.NoMetadata.File(filePath);
                _resolution = pic.Properties.PhotoWidth.ToString() + " x " + pic.Properties.PhotoHeight.ToString();
            }

        }

        public string Resolution
        {
            get { return (_resolution != null || _resolution != " x ") ? _resolution : "0 x 0"; }
        }

        public override string FileType
        {
            get { return "Graphic"; }
        }

        public override void ViewFile()
        {
            System.Diagnostics.Process.Start(this.Path);
        }


    }


    class Video : BaseFile
    {

        private int _lengthInSeconds;
        private string _resolution;
        private double _frameRate;
        private string _videoCompression;
        private string _audioCompression;

        public Video() : base()
        {
            _lengthInSeconds = 0;
            _resolution = "";
            _frameRate = 0;
        }
        public Video(string filePath) : base(filePath)
        {

            if (this.Extension.ToLower() == ".wmv")
            {
                TagLib.Asf.File a = new TagLib.Asf.File(filePath);

                _lengthInSeconds = (int)Math.Round(a.Properties.Duration.TotalSeconds);

                _resolution = a.Properties.VideoWidth.ToString() + " x " + a.Properties.VideoHeight.ToString();

                foreach (TagLib.ICodec codec in a.Properties.Codecs)
                {
                    if (codec.MediaTypes == TagLib.MediaTypes.Video)
                    {
                        _videoCompression = codec.Description;
                    }
                    else if (codec.MediaTypes == TagLib.MediaTypes.Audio)
                    {
                        _audioCompression = codec.Description;
                    }
                }


                _frameRate = 0;

            }


            else if (this.Extension.ToLower() == ".mp4")
            {
                TagLib.Mpeg4.File newMpg = new TagLib.Mpeg4.File(filePath);

                _lengthInSeconds = (int)Math.Round(newMpg.Properties.Duration.TotalSeconds);

                _resolution = newMpg.Properties.VideoWidth.ToString() + " x " + newMpg.Properties.VideoHeight.ToString();

                foreach (ICodec codec in newMpg.Properties.Codecs)
                {
                    if (codec.MediaTypes == TagLib.MediaTypes.Video)
                    {
                        _videoCompression = codec.Description;
                    }
                    else if (codec.MediaTypes == TagLib.MediaTypes.Audio)
                    {
                        _audioCompression = codec.Description;
                    }

                    if (codec is TagLib.Mpeg.VideoHeader)
                    {
                        TagLib.Mpeg.VideoHeader G = (TagLib.Mpeg.VideoHeader)codec;
                        _frameRate = G.VideoFrameRate;

                    }
                }
                //TODO: Figure out frame rate.
            }


            else if (this.Extension.ToLower() == ".webm")
            {
                TagLib.Matroska.File a = new TagLib.Matroska.File(filePath);

                _lengthInSeconds = (int)Math.Round(a.Properties.Duration.TotalSeconds);

                _resolution = a.Properties.VideoWidth.ToString() + " x " + a.Properties.VideoHeight.ToString();


            }

        }

        public int SecondLength
        {
            get { return _lengthInSeconds; }
        }

        public string VideoCompression
        {
            get { return _videoCompression ?? ""; }
        }

        public string AudioCompression
        {
            get { return _audioCompression ?? ""; }
        }



        public string Resolution
        {
            get { return _resolution ?? ""; }
        }

        public double FrameRate
        {
            get { return _frameRate; }
        }

        public override string FileType
        {
            get { return "Video"; }
        }

        public override void ViewFile()
        {
            System.Diagnostics.Process.Start(this.Path);
        }



    }


    class Audio : BaseFile
    {
        private string _artist;
        private string _album;
        private string _genre;
        private int _lengthInSeconds;

        public Audio()
        {
            _artist = "";
            _album = "";
            _genre = "";
            _lengthInSeconds = 0;
        }

        public Audio(string filePath) : base(filePath)
        {

            if (this.Extension.ToLower() == ".mp3")
            {
                TagLib.Mpeg.AudioFile audioFile = new TagLib.Mpeg.AudioFile(filePath);
                _artist = audioFile.Tag.FirstAlbumArtist;
                _album = audioFile.Tag.Album;
                _genre = audioFile.Tag.FirstGenre;
                _lengthInSeconds = (int)Math.Round(audioFile.Properties.Duration.TotalSeconds);
            }
            else if (this.Extension.ToLower() == ".wav")
            {
                TagLib.Riff.File audioFile = new TagLib.Riff.File(filePath);
                _artist = audioFile.Tag.FirstAlbumArtist;
                _album = audioFile.Tag.Album;
                _genre = audioFile.Tag.FirstGenre;
                _lengthInSeconds = (int)Math.Round(audioFile.Properties.Duration.TotalSeconds);

            }
            else if (this.Extension.ToLower() == ".aiff")
            {
                TagLib.Aiff.File audioFile = new TagLib.Aiff.File(filePath);
                _artist = audioFile.Tag.FirstAlbumArtist;
                _album = audioFile.Tag.Album;
                _genre = audioFile.Tag.FirstGenre;
                _lengthInSeconds = (int)Math.Round(audioFile.Properties.Duration.TotalSeconds);

            }
            else if (this.Extension.ToLower() == ".flac")
            {
                TagLib.Flac.File audioFile = new TagLib.Flac.File(filePath);
                _artist = audioFile.Tag.FirstAlbumArtist;
                _album = audioFile.Tag.Album;
                _genre = audioFile.Tag.FirstGenre;
                _lengthInSeconds = (int)Math.Round(audioFile.Properties.Duration.TotalSeconds);

            }
            else if (this.Extension.ToLower() == ".aa" || this.Extension.ToLower() == ".aax")
            {
                TagLib.Audible.File audioFile = new TagLib.Audible.File(filePath);
                _artist = audioFile.Tag.FirstAlbumArtist;
                _album = audioFile.Tag.Album;
                _genre = audioFile.Tag.FirstGenre;
                _lengthInSeconds = (int)Math.Round(audioFile.Properties.Duration.TotalSeconds);

            }
        }

        public string Artist
        {
            get { return _artist ?? ""; }
        }

        public string Album
        {
            get { return _album ?? ""; }
        }

        public string Genre
        {
            get { return _genre ?? ""; }
        }

        public int LengthInSeconds
        {
            get { return _lengthInSeconds; }
        }

        public override string FileType
        {
            get { return "Audio"; }
        }

        public override void ViewFile()
        {
            System.Diagnostics.Process.Start(this.Path);
        }


    }


    class Document : BaseFile
    {
        private bool _isBase;

        public Document() : base()
        {

        }

        public Document(string filePath) : base(filePath)
        {
            _isBase = false;
        }

        public Document(string filePath, bool isBase)
        {
            _isBase = isBase;
        }

        public override string FileType
        {
            get { return (_isBase == false) ? "Document" : "Base File"; }
        }


        public override void ViewFile()
        {
            System.Diagnostics.Process.Start(this.Path);
        }

    }


    class Archive : BaseFile
    {
        private int _fileCount;
        private string _encryptionType;

        public Archive() : base()
        {
            _fileCount = 0;
        }

        public Archive(string filePath) : base(filePath)
        {
            try
            {
                if (ZipFile.IsZipFile(filePath))
                {

                    using (ZipFile zip = ZipFile.Read(filePath))
                    {
                        _fileCount = zip.Count;
                        _encryptionType = zip.Encryption.ToString();
                    }
                }
                else
                {
                    _fileCount = 0;
                    _encryptionType = "No Encryption";
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public int FileCount
        {
            get { return _fileCount; }
        }

        public string EncryptionType
        {
            get { return _encryptionType; }
        }

        public override string FileType
        {
            get { return "Archive"; }
        }

        public override void ViewFile()
        {
            System.Diagnostics.Process.Start(this.Path);
        }


    }

}
