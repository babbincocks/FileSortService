namespace FileSortService
{
    partial class FileSortService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.timeMain = new System.Timers.Timer();
            this.fswMain = new System.IO.FileSystemWatcher();
            ((System.ComponentModel.ISupportInitialize)(this.timeMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fswMain)).BeginInit();
            // 
            // timeMain
            // 
            this.timeMain.Interval = 3000D;
            this.timeMain.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Elapsed);
            // 
            // fswMain
            // 
            this.fswMain.EnableRaisingEvents = true;
            this.fswMain.Changed += new System.IO.FileSystemEventHandler(this.fswMain_Changed);
            this.fswMain.Created += new System.IO.FileSystemEventHandler(this.fswMain_Created);
            this.fswMain.Deleted += new System.IO.FileSystemEventHandler(this.fswMain_Deleted);
            this.fswMain.Renamed += new System.IO.RenamedEventHandler(this.fswMain_Renamed);
            // 
            // FileSortService
            // 
            this.ServiceName = "FileSortService";
            ((System.ComponentModel.ISupportInitialize)(this.timeMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fswMain)).EndInit();

        }

        #endregion

        private System.Timers.Timer timeMain;
        private System.IO.FileSystemWatcher fswMain;
    }
}
