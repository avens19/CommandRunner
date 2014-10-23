using System.Xml.Serialization;

namespace Ovens.Andrew.CommandRunner
{
    /// <summary>
    ///     Used for serializing settings
    /// </summary>
    public class Setting
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}