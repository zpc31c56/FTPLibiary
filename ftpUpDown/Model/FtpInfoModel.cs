namespace ftpUpDown.Model
{
    public class FtpInfoModel
    {
        /// <summary>
        /// FTP伺服器IP位置
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// FTP帳號
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// FTP密碼
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// FTP檔案位置
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// 下載檔案路徑
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 備份檔案
        /// </summary>
        public string Backup { get; set; }
    }
}
