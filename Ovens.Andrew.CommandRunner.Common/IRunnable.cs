using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ovens.Andrew.CommandRunner.Common
{
    public interface IRunnable
    {
        /// <summary>
        ///     Sets up the command with the settings provided
        /// </summary>
        /// <param name="settings"> The settings associated with this command from the input file </param>
        void Initialize(Dictionary<string, string> settings);

        /// <summary>
        ///     Performs the command.
        /// </summary>
        /// <returns> Whether the command succeeded </returns>
        Task<bool> Run();

        /// <summary>
        ///     Uses a Thread.Sleep to wait a specified length of time after the Run()
        /// </summary>
        void WaitAfter();

        /// <summary>
        ///     Verifies the prerequisites for the runnable. Must throw on error. This will cause the program run to end.
        /// </summary>
        void Condition();
    }
}