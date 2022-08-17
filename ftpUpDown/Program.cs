using ftpUpDown;
using NLog;

namespace FTPtransmit
{
    class Program
    {
        static void Main(string[] args)
        {
            StartMoveFile startMoveFile = new StartMoveFile();
            startMoveFile.StartDownload();    
        }
    }
}

