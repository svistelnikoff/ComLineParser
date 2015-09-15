using System.Xml.Linq;

namespace ComLineParser
{
    public class UnsupportedCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }

        protected override void AddCommandXElementParameters(Command command)
        {
            if (command.Type != ECommandTypes.Unsupported) return;
            var unsupportedCommand = (UnsupportedCommand)command;
            if (unsupportedCommand.Value != null)
                CommandXElement.Add(new XAttribute("command", unsupportedCommand.Value));
        }
    }
}
