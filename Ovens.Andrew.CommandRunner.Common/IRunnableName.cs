namespace Ovens.Andrew.CommandRunner.Common
{
    public interface IRunnableName
    {
        /// <summary>
        ///     The name of the command. Used for ordering purposes
        /// </summary>
        string Name { get; }
    }
}