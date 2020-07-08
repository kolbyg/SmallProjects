using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PowerManagementController.ApplicationControl
{
    public class ipmiutil
    {
        string ExePath = "";
        public ipmiutil(string Path)
        {
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException();
            }
            ExePath = Path;
        }

        public bool InvokeCommand(string Command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = ExePath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Command;
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
            return true;
        }
    }
}
