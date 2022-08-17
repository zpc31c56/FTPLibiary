using System;
using System.IO;
using System.Net;
using System.Text;
using ftpUpDown.Model;
using NLog;


namespace ftpUpDown.Helper
{
    class FtpClient
    {
        private string host = string.Empty;
        private string director = string.Empty;
        private string user = string.Empty;
        private string pwd = string.Empty;
        private string path = string.Empty;
        private string backup = string.Empty;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 8192;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public bool BinaryMode = true;

        /* Construct Object */
        /// <summary>
        /// FTP伺服器連接
        /// </summary>
        public FtpClient(FtpInfoModel config)
        {
            host = config.Host;
            user = config.User;
            pwd = config.Pwd;
            director = config.Director;
            path = config.Path;
            backup = config.Backup;
        }

        /// <summary>
        /// FTP連線
        /// </summary>
        private  void Connect(string dirName)
        {
            ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + dirName);
            ftpRequest.Credentials = new NetworkCredential(user, pwd);
        }

        /// <summary>
        /// 取得FTP資料夾下檔案
        /// </summary>
        /// <returns>檔案陣列</returns>
        public string[] GetFilesDirList()
        {
            try
            {
                string[] lsResult = null;
                StringBuilder file = new StringBuilder();
                Connect(director);//目錄

                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = ftpRequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string line = reader.ReadLine();
                if (line == null) 
                {
                    logger.Warn("沒有檔案下載");

                    return lsResult;
                };
                while (line != null)
                {
                    file.Append(line);
                    file.Append("\n");
                    line = reader.ReadLine();
                }

                file.Remove(file.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                lsResult = file.ToString().Split('\n');

                return lsResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param 本地目錄="path"></param>
        public static string CreateDirctor(string path)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                path = path + "\\" + now;
                Console.WriteLine(path);
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            return path;
        }

        /// <summary>
        /// FTP伺服器下載文件;資料存取本地端
        /// </summary>
        /// <param dirName="//遠端資料夾"></param>
        /// <param remoteFile="//遠端資料夾"></param>
        public void Download(string dirName, string remoteFile)
        {
            try
            {
                Connect(dirName + "/"  + remoteFile);
                ftpRequest.UseBinary = BinaryMode;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                //string localPath = CreateDirctor(path);
                FileStream FileStream = new FileStream(path + "/" + remoteFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        FileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) 
                {
                    logger.Error(ex.ToString());
                }
                FileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// FTP伺服器下載文件;資料存取記憶體
        /// </summary>
        /// <param name="remoteFile"></param>
        public void Download(string remoteFile)
        {
            try
            {
                Connect(director + "/" + remoteFile);
                ftpRequest.UseBinary = BinaryMode;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                MemoryStream stream = new MemoryStream();
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = stream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        stream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                stream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }


}

        /// <summary>
        /// FTP伺服器上傳文件;資料存取本地端
        /// </summary>
        /// <param dirName="//遠端資料夾"></param>
        /// <param remoteFile="//遠端檔案"></param>
        public void Upload(string dirName,  string remoteFile)
        {
            try
            {
                Connect(dirName + "/" + remoteFile);
                ftpRequest.UseBinary = BinaryMode;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpStream = ftpRequest.GetRequestStream();
                FileStream FileStream = new FileStream(remoteFile, FileMode.Open);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = FileStream.Read(byteBuffer, 0, bufferSize);                
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = FileStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) 
                {
                    logger.Error(ex.ToString());
                }

                FileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) 
            {
                logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// FTP伺服器上傳文件;資料讀取記憶體
        /// </summary>
        /// <param 上傳到FTP檔案位置="remoteFile"></param>
        public void Upload(string remoteFile)
        {
            try
            {
                Connect(director + "/" + remoteFile);
                ftpRequest.UseBinary = BinaryMode;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpStream = ftpRequest.GetRequestStream();
                MemoryStream stream = new MemoryStream();
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = stream.Read(byteBuffer, 0, bufferSize);

                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = stream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) 
                {
                    logger.Error(ex.ToString());
                }
                stream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex) 
            {
                logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// 備份檔案
        /// </summary>
        /// <param 遠端資料夾="dirName"></param>
        /// <param 遠端檔案="remoteFile"></param>
        public void Backup(string dirName, string remoteFile)
        {
            try
            {
                Connect(dirName + "/" + remoteFile);
                ftpRequest.UseBinary = BinaryMode;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                string localPath = CreateDirctor(backup);
                FileStream FileStream = new FileStream(localPath + "/" + remoteFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        FileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }
                FileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
        /// <summary>
        /// 刪除遠端檔案
        /// </summary>
        /// <param 遠端資料夾="dirName"></param>
        /// <param 遠端檔案="remoteFile"></param>
        public void Delete(string dirName, string remoteFile)
        {
            try
            {
                Connect(dirName + "/" + remoteFile);
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
    }
}
