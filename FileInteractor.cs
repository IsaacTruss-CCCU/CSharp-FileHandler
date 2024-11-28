using System.IO;
using System.Security.Permissions;

namespace fileHanding
{

    internal class FileInteractor : IDisposable
    {

        private class Logging : IDisposable
        {
            private string path;
            private FileStream fs;
            private StreamWriter sw;

            private bool closed = false;

            private bool disposed = false;
            private object Lock = new object();

            public Logging(string path)
            {
                this.path = path;
                fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
            }

            public void log(string line)
            {
                if (closed) // Ensures the file is open to edit
                {
                    fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs);

                    closed = false;
                }

                sw.WriteLine(line);

                close();
            }

            private void close()
            {
                sw.Flush();
                sw.Close();
                fs.Close();

                closed = true;
            }

            // used to dispose the prorgam when necessary 
            public void Dispose()
            {
                lock (Lock)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if ((!disposed) && disposing)
                {
                    if (closed == false)
                    {
                        close();
                    }

                    fs?.Dispose();
                    sw?.Dispose();

                    disposed = true;
                }
            }

            ~Logging()
            {
                Dispose(false);
            }
        }

        // Used for disposing the class after it's used
        private bool disposed = false;
        private object Lock = new object();

        // Basic information needed to the file
        private string path;
        private string? setMode = null;

        // Modes used to define access to the file
        private FileMode fileMode;
        private FileAccess fileAccess;

        // Used to edit the files depending on the mode
        private FileStream fs;
        private StreamReader sr;
        private StreamWriter sw;

        // Used to ensure the file gets closed
        private bool closed = false;

        // Defines the program should create the file if it does not exist
        private bool createIfNotFound;

        // Used for logging
        private Logging logger;

        public FileInteractor(string logPath, string path, string mode, bool createIfNotFound = true)
        {
            logger = new Logging(logPath);

            this.path = path;
            this.createIfNotFound = createIfNotFound;
            setup(mode);
            
        }

        public bool exists()
        {
            return File.Exists(path);
        }

        private void setup(string parsedMode)
        {
            checkFileExists(); // Ensures there is a file to edit if there is not it makes one

            if (setMode == null)
            {

                if (parsedMode == "A") // Sets up the append mode
                {
                    setMode = "A";

                    fileMode = FileMode.Append;
                    fileAccess = FileAccess.Write;

                    FileIOPermission appendPermission = new FileIOPermission(FileIOPermissionAccess.Append, path);

                    try
                    {
                        appendPermission.Demand();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new UnauthorizedAccessException("Access Denied.");
                    }

                    fs = new FileStream(path, fileMode, fileAccess);
                    sw = new StreamWriter(fs);
                }

                else if (parsedMode == "W") // Sets up the write mode
                {
                    setMode = "W";

                    FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);

                    try
                    {
                        writePermission.Demand();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new UnauthorizedAccessException("Access Denied.");
                    }

                    sw = new StreamWriter(path);
                }

                else if (parsedMode == "R") // Sets up the read mode
                {
                    setMode = "R";

                    fileMode = FileMode.Open;
                    fileAccess = FileAccess.Read;

                    FileIOPermission readPermission = new FileIOPermission(FileIOPermissionAccess.Read, path);

                    try
                    {
                        readPermission.Demand();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new UnauthorizedAccessException("Access Denied.");
                    }

                    fs = new FileStream(path, fileMode, fileAccess);
                    sr = new StreamReader(fs);
                }
            }

            else if (setMode != null) // used to change function to a different handler if nexessary but ensures everythin is finsihed up before hand
            {
                if (!closed)
                {
                    close();
                }

                setMode = null;
                setup(parsedMode);
                closed = false;
            }

            else
            {
                throw new ArgumentException("Invalid Mode");
            }
        }

        private void checkFileExists() // Ensures a file exists and creates one if the user wishes
        {
            if (!File.Exists(path))
            {
                if (createIfNotFound)
                {
                    File.Create(path).Close();

                    logger.log($"{path} created at {DateTime.Now}"); // Logs the file creation
                }

                else
                {
                    throw new FileNotFoundException("File does not exist");
                }
            }
        }

        public void append(List<string> lines, bool writeLines = true)
        {
            if (setMode != "A")
            {
                setup("A");
            }

            if (writeLines)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    sw.WriteLine(lines[i]);
                }
            }

            else
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    sw.Write(lines[i]);
                }
            }

            close();

            logger.log($"{path} appended to at {DateTime.Now}");
        }

        public void write(List<string> lines, bool writeLines = true)
        {
            if (setMode != "W")
            {
                setup("W");
            }

            if (writeLines)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    sw.WriteLine(lines[i]);
                }
            }

            else
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    sw.Write(lines[i]);
                }
            }

            logger.log($"{path} written to at {DateTime.Now}");

            close();
        }

        public string[] read()
        {
            if (setMode != "R")
            {
                setup("R");
            }

            List<string> lines = new List<string>();
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string line = sr.ReadLine();

            while (line != null)
            {
                lines.Add(line);
                line = sr.ReadLine();
            }

            close();

            logger.log($"{path} read at {DateTime.Now}");

            return lines.ToArray();
        }

        public void close() // CLoses the file based off of the mode
        {
            if (setMode == "A")
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            else if (setMode == "W")
            {
                sw.Flush();
                sw.Close();
            }

            else if (setMode == "R")
            {
                sr.Close();
                fs.Close();
            }

            setMode = null;
            closed = true;
        }

        public void Dispose()
        {
            lock (Lock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((!disposed) && disposing)
            {
                if (closed == false)
                {
                    close();
                }

                fs?.Dispose();
                sw?.Dispose();
                sr?.Dispose();
                logger?.Dispose();

                disposed = true;
            }
        }

        ~FileInteractor()
        {
            Dispose(false);
        }
    }
}
