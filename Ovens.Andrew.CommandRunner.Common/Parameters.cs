using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ovens.Andrew.CommandRunner.Common
{
    public static class Parameters
    {
        public static readonly Dictionary<string, string> Args = new Dictionary<string, string>();

        public static void Initialize(string[] args, SetupFile setupFile)
        {
            if (setupFile.Arguments != null && setupFile.Parameters != null)
            {
                throw new Exception("Arguments and Parameters cannot both be present");
            }

            if (setupFile.Arguments != null)
            {
                // Argument mode.

                string[] argNames = setupFile.Arguments.Split(',');

                if (args == null || args.Length != argNames.Length)
                {
                    throw new ArgumentException(string.Format("There should be exactly {0} arguments", argNames.Length));
                }

                for (int i = 0; i < args.Length; i++)
                {
                    Args[argNames[i]] = args[i];

                    Log.Comment("{0}: {1}", argNames[i], args[i]);
                }
            }
            if (setupFile.Parameters != null)
            {
                // Parameter mode. Command line arguments are specified in a single '&' delimited string

                string[] parameters = args[0].Split('&');
                string paramNameString = setupFile.Parameters;
                string[] paramNames = paramNameString.Split(',');

                if (parameters.Length != paramNames.Length)
                {
                    throw new ArgumentException(string.Format("There should be exactly {0} parameters",
                        paramNames.Length));
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    Args[paramNames[i]] = parameters[i];

                    Log.Comment("{0}: {1}", paramNames[i], parameters[i]);
                }
            }
        }
    }
}