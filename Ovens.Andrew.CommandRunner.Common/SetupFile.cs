namespace Ovens.Andrew.CommandRunner.Common
{
    public class SetupFile
    {
        // The list of Commands to run with their embedded settings
        public Command[] Commands { get; set; }
        // The comma separated list of the names of the arguments that will be passed to the program, other than the setup file.
        // These names will be used to reference the arguments from the arguments dictionary.
        public string Arguments { get; set; }
        // Same as the Arguments property, except Parameters are provided to the program as a single argument, separated by the '&' symbol
        // NOTE: You may use either Arguments or Parameters, but not both.
        public string Parameters { get; set; }
        // Event viewer log source name
        public string LogSourceName { get; set; }
    }
}
