using System.Xml.Linq;

namespace ComLineParser
{
    public class PrintCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }

        protected override void AddCommandXElementParameters(Command command)
        {
            if (command.Parameters != null && command.Parameters.Length > 0)
            {
                CommandXElement.Add(new XAttribute("message", command.Parameters[0]));
            }
        }
    }
}
