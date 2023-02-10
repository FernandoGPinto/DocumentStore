using DocumentStore.Core.Enums;
using DocumentStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStore.Core.Utilities
{
    public class SophosScanner : IScanner
    {
        private readonly string sav32cliLocation;

        /// <summary>
        /// Creates a new Windows defender scanner
        /// </summary>
        /// <param name="sav32cliLocation">The location of the mpcmdrun.exe file e.g. C:\Program Files\Windows Defender\MpCmdRun.exe</param>
        public SophosScanner(string sav32cliLocation)
        {
            if (!File.Exists(sav32cliLocation))
            {
                throw new FileNotFoundException();
            }

            this.sav32cliLocation = new FileInfo(sav32cliLocation).FullName;
        }

        /// <summary>
        /// Scan a single file
        /// </summary>
        /// <param name="file">The file to scan</param>
        /// <param name="timeoutInMs">The maximum time in milliseconds to take for this scan</param>
        /// <returns>The scan result</returns>
        public ScanResult Scan(string file, int timeoutInMs = 30000)
        {
            if (!File.Exists(file))
            {
                return ScanResult.FileNotFound;
            }

            var fileInfo = new FileInfo(file);

            var process = new Process();

            var startInfo = new ProcessStartInfo(this.sav32cliLocation)
            {
                Arguments = $"SAV32CLI -P=\"{fileInfo.FullName}\"",
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(timeoutInMs);

            if (!process.HasExited)
            {
                process.Kill();
                return ScanResult.Timeout;
            }

            Debug.WriteLine(process.ExitCode);

            switch (process.ExitCode)
            {
                case 0:
                    return ScanResult.NoThreatFound;
                case 2:
                    return ScanResult.ThreatFound;
                default:
                    return ScanResult.Error;
            }
        }
    }
}
