using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineParser
{
    public class PrintCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            XElement _command_node = FindCommandNode(command);
            XElement _xcommand = new XElement(command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString()));
            if (command.Parameters != null &&
                command.Parameters.Length > 0)
            {
                _xcommand.Add(new XAttribute("message", command.Parameters[0]));
            }
            _command_node.Add(_xcommand);

            SaveLogFile();
        }
    }
}
