using ftpUpDown.Helper;
using ftpUpDown.Model;
using System.Configuration;
using NLog;


namespace ftpUpDown
{
    /// <summary>
    /// downloadServer:FTP伺服器IP
    /// account:帳號
    /// password:密碼
    /// downloadDirector:下載檔案位置
    /// </summary>
    public class StartMoveFile
    {
        private readonly string downloadServer = ConfigurationManager.AppSettings.Get("downloadServer");
        private readonly string account = ConfigurationManager.AppSettings.Get("account");
        private readonly string password = ConfigurationManager.AppSettings.Get("password");
        private readonly string downloadDirector = ConfigurationManager.AppSettings.Get("downloadDirector");
        private readonly string localPath = ConfigurationManager.AppSettings.Get("localPath");
        private readonly string backupPath = ConfigurationManager.AppSettings.Get("backupPath");
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// FTP伺服器下載至本地資料夾
        /// </summary>
        public void StartDownload()
        {
            FtpInfoModel FtpInfoData = SetValue(downloadServer, account, password, downloadDirector, localPath, backupPath);
            FtpClient ftpClient = new FtpClient(FtpInfoData);
            string[] lsFileName = ftpClient.GetFilesDirList();
            if (lsFileName == null) { 
                return; 
            }
            foreach (var file in lsFileName)
            {
                ftpClient.Download(FtpInfoData.Director, file);
                logger.Info("檔案下載{0}", file);
                ftpClient.Backup(FtpInfoData.Director, file);
                logger.Info("檔案備份{0}", file);
                ftpClient.Delete(FtpInfoData.Director, file);
                logger.Info("檔案刪除{0}", file);
            }
        }

        /// <summary>
        /// 傳入App.config參數進來
        /// </summary>
        /// <param downloadServer="server"></param>
        /// <param account="account"></param>
        /// <param password="password"></param>
        /// <param downloadDirector="director"></param>
        /// <param Path="path"></param>
        /// <param Backup="backup"></param>
        /// <returns></returns>
        private FtpInfoModel SetValue(string server, string account, string password, string director, string path, string backup)
        {
            FtpInfoModel FtpInfoData = new FtpInfoModel
            {
                User = account,
                Pwd = password,
                Host = server,
                Director = director,
                Path = path,
                Backup = backup
            };

            return FtpInfoData;
        }
    }
}









///下載FTP下的資料再上傳至另一台FTP伺服器的資料
//public void Start()
//{
//    FtpInfoModel downloadIP = new FtpInfoModel
//    {
//        Host = ConfigurationManager.AppSettings.Get("DownloadServer"),
//        UserName = ConfigurationManager.AppSettings.Get("DownloadID"),
//        Password = ConfigurationManager.AppSettings.Get("DownloadPassword"),
//        Director = ConfigurationManager.AppSettings.Get("DownloadDirector")
//    };
//    FtpInfoModel uploadIP = new FtpInfoModel
//    {
//        Host = ConfigurationManager.AppSettings.Get("UploadServer"),
//        UserName = ConfigurationManager.AppSettings.Get("UploadID"),
//        Password = ConfigurationManager.AppSettings.Get("UploadPassword"),
//        Director = ConfigurationManager.AppSettings.Get("UploadDirector")
//    };

//    FtpClient ftpClient = new FtpClient(downloadIP);
//    FtpClient ftpInvoice = new FtpClient(uploadIP);
//    string[] ls = ftpClient.GetFilesDirList();
//    foreach (var file in ls)
//    {
//        ftpClient.Download(file);
//        ftpInvoice.Upload(file);
//    }
//}