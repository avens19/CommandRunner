using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ovens.Andrew.CommandRunner.Common;

namespace Ovens.Andrew.CommandRunner.DateRenamer
{
    [Export(typeof(IRunnable))]
    [ExportMetadata("Name", "DateRenamer")]
    public class DateRenamer : IRunnable
    {
        private string _format;
        private string[] _inDirs;
        private string _outDir;

        public void Initialize(Dictionary<string, string> settings)
        {
            _format = settings["dateFormat"];
            _inDirs = settings["inputDirectories"].Split(',');
            _outDir = settings["outputDirectory"];
        }

        public void Condition()
        {
            foreach (var dir in _inDirs)
            {
                if (!Directory.Exists(dir))
                {
                    throw new ArgumentException(string.Format("Input directory does not exist: {0}", dir));
                }
            }
        }

        public Task<bool> Run()
        {
            Log.Comment("Starting DateRenamer run");
            var t = new Task<bool>(() =>
            {
                if (!Directory.Exists(_outDir))
                {
                    Directory.CreateDirectory(_outDir);
                }

                foreach (var dir in _inDirs)
                {
                    foreach (var file in Directory.EnumerateFiles(dir))
                    {
                        DateTime modified = File.GetLastWriteTime(file);
                        string newName = modified.ToString(_format);

                        int dupeValue = 0;

                        while (true)
                        {
                            try
                            {
                                string path = Path.Combine(_outDir, modified.Year.ToString("0000"), modified.Month.ToString("00"));
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                if (dupeValue == 0)
                                {
                                    string newPath = string.Concat(Path.Combine(path, newName), Path.GetExtension(file));
                                    Log.Comment("{0} -> {1}", file, newPath);
                                    File.Move(file, newPath);
                                }
                                else
                                {
                                    string newPath = string.Concat(Path.Combine(path, string.Format("{0}-{1}", newName, dupeValue)), Path.GetExtension(file));
                                    Log.Comment("{0} -> {1}", file, newPath);
                                    File.Move(file, newPath);
                                }

                                break;
                            }
                            catch (IOException) // File with same name exists
                            {
                                if (dupeValue < 50)
                                {
                                    dupeValue++;
                                    continue;
                                }

                                throw;
                            }
                        }
                    }
                }
                
                return true;
            });

            t.Start();

            return t;
        }

        public void WaitAfter()
        {
        }
    }
}
