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
        public override void SaveToFile(Command _command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            XElement _command_node = FindCommandNode(_command);
            XElement _xcommand = new XElement(_command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString()));
            if (_command.Parameters != null &&
                _command.Parameters.Length > 0)
            {
                _xcommand.Add(new XAttribute("message", _command.Parameters[0]));
            }
            _command_node.Add(_xcommand);

            SaveLogFile();
        }
    }
}
