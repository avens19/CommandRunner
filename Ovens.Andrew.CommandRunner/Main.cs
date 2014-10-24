using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Ovens.Andrew.CommandRunner.Common;

namespace Ovens.Andrew.CommandRunner
{
    public class Main
    {
        [ImportMany] private IEnumerable<Lazy<IRunnable, IRunnableName>> _commands = null;
        private CompositionContainer _container;

        public Main()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(ConfigurationManager.AppSettings["extensionsDirectory"]));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public void Run(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("The path to the setup file must be provided");
                    Usage();
                }

                string path = args[0];

                if (!File.Exists(path))
                {
                    Console.WriteLine("Setup file does not exist!");
                    Usage();
                }

                var file = new FileStream(path, FileMode.Open);
                var xml = new XmlSerializer(typeof (SetupFile));
                var setupFile = ((SetupFile) xml.Deserialize(file));

                Log.Initialize(setupFile.LogSourceName);

                if (args.Length > 1)
                {
                    switch (args[1])
                    {
                        case "/install":
                            Log.Install();
                            return;
                        case "/uninstall":
                            Log.Uninstall();
                            return;
                    }
                }

                args = args.Skip(1).ToArray();

                Log.Comment("Starting show renaming");

                Log.Comment("Arguments: {0}", args.Aggregate("", (current, s1) => current + string.Format("{0}&", s1)));
                Parameters.Initialize(args, setupFile);

                var cmds = setupFile.Commands;

                Log.Comment("Order is: {0}",
                    cmds.Aggregate("", (current, command) => current + string.Format("{0},", command.Name)));

                foreach (var item in cmds)
                {
                    Lazy<IRunnable, IRunnableName> command =
                        _commands.FirstOrDefault(cmd => cmd.Metadata.Name == item.Name);
                    if (command == null)
                    {
                        Log.Warning("Command not found: {0}", item);
                        continue;
                    }
                    command.Value.Initialize(item.GetSettingDictionary());
                }

                foreach (var item in cmds)
                {
                    Lazy<IRunnable, IRunnableName> command =
                        _commands.FirstOrDefault(cmd => cmd.Metadata.Name == item.Name);
                    if (command == null)
                    {
                        continue;
                    }
                    command.Value.Condition();
                }

                foreach (var item in cmds)
                {
                    try
                    {
                        Lazy<IRunnable, IRunnableName> command =
                            _commands.FirstOrDefault(cmd => cmd.Metadata.Name == item.Name);
                        if (command == null)
                        {
                            continue;
                        }
                        command.Value.Run().Wait();
                        command.Value.WaitAfter();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("{0} failed with exception: {1}", item.Name, ex.ToString());
                    }
                }

                Log.Comment("Successfully ran commands");
            }
            catch (Exception e)
            {
                if(Log.Initialized)
                    Log.Error(e.ToString());
                else
                    Console.WriteLine(e);

                Console.WriteLine();
                Usage();
            }
        }

        public void Usage()
        {
            Console.WriteLine(
@"********** USAGE**********

Ovens.Andrew.CommandRunner <inputFile> [/install] [/uninstall] [args/params]

inputFile - Path of the input file which tells the program which extensions to run and contains the settings
/install - Installs the log source into the Windows Event Viewer (must run as administrator)
/uninstall - Uninstalls the log source from the Windows Event Viewer (must run as administrator)
args - Space separated arguments that will be passed to each extension
params - & separated parameters that will be passed to each extension

Examples:

1) Argument Mode:

  Ovens.Andrew.CommandRunner Input\Setup.xml arg1 arg2

2) Parameter Mode:

  Ovens.Andrew.CommandRunner Input\Setup.xml param1&param2

3) Log Install

  Ovens.Andrew.CommandRunner Input\Setup.xml /install

4) Log Uninstall

  Ovens.Andrew.CommandRunner Input\Setup.xml /uninstall"
                );
        }
    }
}
