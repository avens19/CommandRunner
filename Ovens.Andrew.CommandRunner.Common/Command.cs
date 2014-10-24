using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ovens.Andrew.CommandRunner.Common
{
    /// <summary>
    ///     Used for serializing settings
    /// </summary>
    public class Command
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public Setting[] Settings { get; set; }

        public Dictionary<string, string> GetSettingDictionary()
        {
            return Settings.ToDictionary(sett => sett.Name, sett => sett.Value);
        }
    }
}