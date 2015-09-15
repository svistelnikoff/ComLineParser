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
            CreateCommandXElement(command);
            SaveLogFile();
        }

        protected override void AddCommandXElementParameters(Command command)
        {
            if (command.Type == ECommandTypes.Unsupported && (command as UnsupportedCommand).Value != null)
            {
                CommandXElement.Add(new XAttribute("command", (command as UnsupportedCommand).Value));
            }

        }
    }
}
