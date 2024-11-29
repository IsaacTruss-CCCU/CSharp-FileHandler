using System.IO;
using System.Security.Permissions;

namespace fileHanding
{
    public class FileHandler
    {
        private string path;
        private string mode;
        private FileInteractor interactor;
        private bool createIfNotFound;

        // Used to store the information about the file being handled
        private Dictionary<string, string> properties;
        private FileInfo fileInfo;

        public FileHandler(string path, string mode, string logpath, bool createIfNotFound = true)
        {
            this.path = path;
            this.mode = mode;
            this.createIfNotFound = createIfNotFound;

            this.interactor = new FileInteractor(logpath, path, mode, createIfNotFound);

            this.fileInfo = new FileInfo(path);

            this.properties = new Dictionary<string, string>();

            updateInfo();
        }

        // Updates the stored information of the file
        private void updateInfo()
        {
            properties.Clear();
            properties.Add("Size", Convert.ToString(fileInfo.Length));
            properties.Add("Creation Time", Convert.ToString(fileInfo.CreationTime));
            properties.Add("Last Access Time", Convert.ToString(fileInfo.LastAccessTime));
            properties.Add("Last Edit Time", Convert.ToString(fileInfo.LastWriteTime));
        }

        public Dictionary<string, string> getFileInfo()
        {
            return properties;
        }

        public bool isReadable()
        {
            try
            {
                FileIOPermission readCheck = new FileIOPermission(FileIOPermissionAccess.Read, path);
                readCheck.Demand();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }

        public bool isWriteable()
        {
            try
            {
                FileIOPermission readCheck = new FileIOPermission(FileIOPermissionAccess.Write, path);
                readCheck.Demand();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }

        public bool isAppendable()
        {
            try
            {
                FileIOPermission readCheck = new FileIOPermission(FileIOPermissionAccess.Append, path);
                readCheck.Demand();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }

            return true;
        }

        public void append(List<string> lines, bool writeLines = true)
        {
            interactor.append(lines, writeLines);
        }

        public void write(List<string> lines, bool writeLines = true)
        {
            interactor.write(lines, writeLines);
        }

        public string[,] readCSV()
        {
            return interactor.readCSV();
        }

        public string[] read()
        {
            return interactor.read();
        }

        public void createFile()
        {
            File.Create(path).Close();
        }

        public void deleteFile()
        {
            File.Delete(path);
        }

        public void moveFile(string newPath)
        {
            File.Move(path, newPath);
            path = newPath;
        }

        public void copyFile(string newPath)
        {
            File.Copy(path, newPath);
        }
    }
}