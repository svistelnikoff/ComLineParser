﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineParser
{
    public class KeyValueCommandLogger : BaseCommandLogger
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
