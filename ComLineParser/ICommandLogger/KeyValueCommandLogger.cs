using System.Xml.Linq;

namespace ComLineParser
{
    public class KeyValueCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }

        protected override void AddCommandXElementParameters(Command command)
        {
            if (command.Parameters == null ||
                command.Parameters.Length <= 0 ||
                command.Parameters.Length%2 != 0) return;
            for (int i = 0; i < command.Parameters.Length; i += 2)
            {
                CommandXElement.Add(new XElement("pair",
                    new XAttribute("key", command.Parameters[i]),
                    new XAttribute("value", command.Parameters[i + 1]))
                    );
            }
        }
    }
}
