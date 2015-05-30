using System;
using System.Diagnostics;

namespace SwitchWiFi
{
    class cmd : IDisposable
    {
        private Process p;
        public cmd()
        {
        }

        public string execute(string cmds)
        {
            p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine(cmds);
            p.StandardInput.WriteLine("exit");
            string s = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return s;
        }

        public void Dispose()
        {
            p.Dispose();
        }
    }
}
