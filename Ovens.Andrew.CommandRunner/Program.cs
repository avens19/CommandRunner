using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Ovens.Andrew.CommandRunner
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var main = new Main();
            main.Run(args);
        }
    }
}