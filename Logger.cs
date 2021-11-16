using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LabToys
{
    public class Logger
    {
        public Logger( string filePath )
            : this( filePath, DateTime.Now )
        {
            
        }

        //-----------------------------------------------------------------------------------------
        public Logger( string filePath, DateTime startTime )
        {
            //devide path into specific categories
            path = System.IO.Path.GetDirectoryName(filePath);
            fileName = System.IO.Path.GetFileName(filePath);
            fileFullPath = System.IO.Path.GetFullPath(filePath);

            Console.WriteLine("path: " + path);
            Console.WriteLine("fileName: " + fileName);
            Console.WriteLine("fileFullPath: " + fileFullPath);

            //device path into elements and test that its existing
            string[] pathsElements = path.Split(System.IO.Path.DirectorySeparatorChar);
            string testPath = "";
            for (int i = 0; i < pathsElements.Length; i++)
            {
                testPath = System.IO.Path.Combine(new string[] { testPath, pathsElements[i] });
                if (!Directory.Exists(testPath))
                {
                    Directory.CreateDirectory(testPath);
                    Console.WriteLine("Path created: " + testPath);
                }
            }

            if (!OpenFile())
            {
                Console.WriteLine("Open file issue");
            }

            this.startTime = startTime;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        private string path = "";
        private string fileName = "";
        private string fileFullPath = "";
        private FileStream file = null;

        private int maxUnsaved = 10;
        private int maxLines = 10000;
        private int unsaved = 0;
        private int linesInFile = 0;
        private long allLines = 0;
        private int fileIdx = 0;

        private string[] headers = new string[0];
        private string lineEnding = "\n";
        private string columnSeparator = ";";

        private DateTime startTime;
        private long idx = 0;

        private bool includeDate = true;
        private bool includeTime = true;
        private bool includeIdx = false;
        private bool includeTimeFromStart = false;

        //-----------------------------------------------------------------------------------------
        public string Path { get => path; }
        public string FileName { get => fileName; }

        public int MaxUnsaved { get => maxUnsaved; set => maxUnsaved = value; }
        public int MaxLines { get => maxLines; set => maxLines = value; }
        public long AllLines { get => allLines; }

        public string[] Headers { get => headers; }
        public string LineEnding { get => lineEnding; set => lineEnding = value; }

        public bool IncludeDate { get => includeDate; set => includeDate = value; }
        public bool IncludeTime { get => includeTime; set => includeTime = value; }
        public bool IncludeIdx { get => includeIdx; set => includeIdx = value; }
        public bool IncludeTimeFromStart { get => includeTimeFromStart; set => includeTimeFromStart = value; }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        private bool WriteLine( string line, bool reOpen=false )
        {
            line += lineEnding;
            try
            {
                file.Write(Encoding.ASCII.GetBytes( line ), 0, line.Length);
            }
            catch
            {
                return false;
            }

            allLines++;
            linesInFile++;
            unsaved++;
            if( unsaved >= maxUnsaved
                || reOpen )
            {
                unsaved = 0;
                return ReOpen();
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------
        private bool OpenFile()
        {
            try
            {
                file = File.Open(fileFullPath, FileMode.Append);
            }
            catch
            {
                file = null;
                return false;
            }
            return true;
        }

        //-----------------------------------------------------------------------------------------
        private bool ReOpen()
        {
            Console.WriteLine("ReOpen");
            CloseFile();
            return OpenFile();
        }

        //-----------------------------------------------------------------------------------------
        public void CloseFile()
        {
            file.Close();
        }

        //-----------------------------------------------------------------------------------------
        public bool MakeHeaders( string[] columnsName )
        {
            if( columnsName.Length == 0 
                && headers.Length != 0 )
            {
                return false;
            }

            string header = "";
            headers = new string[4];
            int idx = 0;

            //predefined columns
            if( includeDate )
            {
                header += "Date" + columnSeparator;
                headers[idx++] = "Date";
            }

            if( includeTime )
            {
                header += "Time" + columnSeparator;
                headers[idx++] = "Time";
            }

            if( includeIdx )
            {
                header += "Idx" + columnSeparator;
                headers[idx++] = "Idx";
            }

            if( includeTimeFromStart )
            {
                header += "Time_from_start" + columnSeparator;
                headers[idx++] = "Time_from_start";
            }

            Array.Resize(ref headers, idx + columnsName.Length);

            //user columns
            for( int i=0; i<columnsName.Length; i++ )
            {
                header += columnsName[i] + columnSeparator;
                headers[idx++] = columnsName[i];
            }

            header = header.Remove(header.Length - columnSeparator.Length);                         //remove last separator
            return WriteLine(header, true );
        }

        //-----------------------------------------------------------------------------------------
        public bool Log( object[] data )
        {
            string line = "";
            DateTime time = DateTime.Now;

            //predefined columns
            if( includeDate )
            {
                line += time.ToString("yyyy.MM.dd") + columnSeparator;
            }

            if( includeTime )
            {
                line += time.ToString("HH:mm:ss") + columnSeparator;
            }

            if( includeIdx )
            {
                line += (++idx).ToString() + columnSeparator;
            }

            if( includeTimeFromStart )
            {
                line += time.Subtract(startTime).TotalSeconds.ToString("0.000") + columnSeparator;
            }

            //user data
            for( int i=0; i<data.Length; i++ )
            {
                line += data[i].ToString() + columnSeparator;
            }

            line = line.Remove(line.Length - columnSeparator.Length);

            return WriteLine(line);
        }
    }
}
