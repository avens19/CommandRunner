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
            if (args.Length < 1)
            {
                Console.WriteLine("The path to the setup file must be provided");
            }

            string path = args[0];

            if (!File.Exists(path))
            {
                throw new ArgumentException("Setup file does not exist!");
            }

            var file = new FileStream(path, FileMode.Open);
            var xml = new XmlSerializer(typeof(SetupFile));
            var setupFile = ((SetupFile)xml.Deserialize(file));

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

            try
            {
                Log.Comment("Arguments: {0}", args.Aggregate("", (current, s1) => current + string.Format("{0}&", s1)));
                Parameters.Initialize(args, setupFile);

                var cmds = setupFile.Commands;

                Log.Comment("Order is: {0}", cmds.Aggregate("", (current, command) => current + string.Format("{0},", command.Name)));

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
                Log.Error(e.ToString());
            }
        }
    }
}
