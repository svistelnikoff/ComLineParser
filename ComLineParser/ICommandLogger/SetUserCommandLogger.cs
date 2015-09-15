using System.Xml.Linq;


namespace ComLineParser
{
    public class SetUserCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }

        protected override void AddCommandXElementParameters(Command command)
        {
            if (command.Parameters == null || command.Parameters.Length <= 0) return;
            CommandXElement.Add(new XAttribute("changed_to", command.Parameters[0]));
        }
    }
}
