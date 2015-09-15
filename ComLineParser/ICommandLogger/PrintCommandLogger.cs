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
            CreateCommandXElement(command);
            AddCommandXElementParameters(command);
            CommandNodeXElement.Add(CommandXElement);
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
