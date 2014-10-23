using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;

namespace Ovens.Andrew.CommandRunner.TheRenamer
{
    [Export(typeof (IRunnable))]
    [ExportMetadata("Name", "TheRenamer")]
    public class TheRenamer : IRunnable
    {
        private string _path;
        private string _torrentStatus;
        private string _movieDir;
        private string _tvDir;
        private string _postWaitTime;

        public void Initialize(Dictionary<string, string> settings)
        {
            _path = settings["path"];
            _torrentStatus = settings["torrentStatus"];
            _movieDir = settings["movieDir"];
            _tvDir = settings["tvDir"];
            _postWaitTime = settings["postWaitTime"];
        }

        public Task<bool> Run()
        {
            Log.Comment("Starting TheRenamer run");
            var t = new Task<bool>(() =>
            {
                string path = _path;
                if (Parameters.Args["Directory"].StartsWith(_movieDir, StringComparison.InvariantCultureIgnoreCase))
                {
                    Process.Start(path, "-fetchmovie");
                }
                else if (Parameters.Args["Directory"].StartsWith(_tvDir, StringComparison.InvariantCultureIgnoreCase))
                {
                    Process.Start(path, "-fetch");
                }

                return true;
            });

            t.Start();

            return t;
        }

        public void WaitAfter()
        {
            Thread.Sleep(int.Parse(_postWaitTime));
        }

        public void Condition()
        {
            if (Parameters.Args["Status"] != _torrentStatus)
            {
                throw new ArgumentException(string.Format("Got wrong torrent status. Received: {0}, Expected: {1}",
                    Parameters.Args["Status"], int.Parse(_torrentStatus)));
            }

            if (!Parameters.Args["Directory"].StartsWith(_movieDir, StringComparison.InvariantCultureIgnoreCase) &&
                !Parameters.Args["Directory"].StartsWith(_tvDir, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(string.Format("Torrent is in an unexpected directory. Received: {0}",
                    Parameters.Args["Directory"]));
            }
        }
    }
}