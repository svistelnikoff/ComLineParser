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
        public override void SaveToFile(Command _command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            XElement _command_node = FindCommandNode(_command);
            XElement _xcommand = new XElement(_command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString()));
            if (_command.Type == ECommandTypes.Unsupported &&
               (_command as UnsupportedCommand).Value != null)
            {
                _xcommand.Add(new XAttribute("command", (_command as UnsupportedCommand).Value));
            }
            _command_node.Add(_xcommand);

            SaveLogFile();
        }
    }
}
