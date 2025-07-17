using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Utils
    {
        static Utils()
        {
            ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            LogsFolder = Path.Combine(ExeDirectory, "Logs");
            Directory.CreateDirectory(LogsFolder);
            OutputLogPath = Path.Combine(LogsFolder, "OutputLog.txt");
        }

        public readonly static string ExeDirectory;
        public readonly static string LogsFolder;
        public readonly static string OutputLogPath;

        

    }
}
