using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineParser
{
    public class UnsupportedCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            XElement _command_node = FindCommandNode(command);
            XElement _xcommand = new XElement(command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString()));
            if (command.Type == ECommandTypes.Unsupported &&
               (command as UnsupportedCommand).Value != null)
            {
                _xcommand.Add(new XAttribute("command", (command as UnsupportedCommand).Value));
            }
            _command_node.Add(_xcommand);

            SaveLogFile();
        }
    }
}
