using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;

namespace Ovens.Andrew.CommandRunner.RestartProcess
{
    [Export(typeof(IRunnable))]
    [ExportMetadata("Name", "RestartProcess")]
    public class RestartProcess: IRunnable
    {
        private string _processName;
        private string _programPath;
        private string _arguments;
        private const int MaxRetries = 10;

        public void Initialize(Dictionary<string, string> settings)
        {
            _processName = settings["processName"];
            _programPath = settings["programPath"];
            _arguments = settings["arguments"];
        }

        public Task<bool> Run()
        {
            var t = new Task<bool>(() =>
            {
                int tries = 0;
                try
                {
                    Process[] proc = Process.GetProcessesByName(_processName);
                    proc[0].Kill();
                }
                catch (IndexOutOfRangeException)
                {
                    Log.Comment("Process wasn't running");
                }

                Thread.Sleep(5000);

                while (Process.GetProcessesByName(_processName).Length < 1 && tries <= MaxRetries)
                {
                    try
                    {
                        var startInfo = new ProcessStartInfo(_programPath) {WindowStyle = ProcessWindowStyle.Minimized};

                        if (!string.IsNullOrWhiteSpace(_arguments))
                            startInfo.Arguments = _arguments;

                        Process.Start(startInfo);

                        Thread.Sleep(5000);

                        Process.Start(startInfo);

                        return true;
                    }
                    catch (Exception)
                    {
                        if (tries == MaxRetries)
                            throw;

                        tries++;
                    }
                }

                return false;
            });

            t.Start();

            return t;
        }

        public void WaitAfter()
        {
        }

        public void Condition()
        {
        }
    }
}
