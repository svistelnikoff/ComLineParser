using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComLineParser
{
    public class BaseCommandLogger : ICommandLogger
    {
        protected const string LogFilePath = "CommandLog.xml";
        protected const string XDocRootName = "ComLineParser";
        protected XElement CommandNodeXElement;
        protected XElement CommandXElement;

        protected XDocument LogFile;

        protected void OpenLogFile()
        {
            try
            {
                LogFile = XDocument.Load(LogFilePath);
                ValidateLogFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to open existing log file: " + e.Message);
                CreateNewLogFile();
            }
        }

        protected void CreateNewLogFile()
        {
            try
            {
                LogFile = new XDocument(
                    new XElement(XDocRootName,
                            new XAttribute("description","command log file for ComLineParser"),
                            new XAttribute("modified", DateTime.Now.ToString())
                        )
                    );
                LogFile.Save(LogFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create log file: " + e.Message);
            }
        }

        protected void ValidateLogFile()
        {
            if (String.Compare(LogFile.Root.Name.LocalName, XDocRootName, StringComparison.OrdinalIgnoreCase) != 0)
                throw (new Exception("File doesn't contain valid root element"));
            XAttribute _xattribute = FindXAttribute(LogFile.Root, "description");
            if (_xattribute == null)
            {
                LogFile.Root.Add(new XAttribute("description", "command log file for ComLineParser"));
            }
            else
            {
                if (String.Compare(_xattribute.Value, "command log file for ComLineParser") != 0)
                    _xattribute.SetValue("command log file for ComLineParser");
            }

            _xattribute = FindXAttribute(LogFile.Root, "modified");
            if (_xattribute == null)
            {
                LogFile.Root.Add(new XAttribute("modified", DateTime.Now.ToString()));
            }
        }

        protected void SaveLogFile()
        {
            try
            {
                LogModified();
                LogFile.Save(LogFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save log file: " + e.Message);
            }
        }

        protected void LogModified()
        {
           XAttribute _xattribute = FindXAttribute(LogFile.Root, "modified");
           if (_xattribute != null) _xattribute.SetValue(DateTime.Now.ToString());
            else LogFile.Root.Add(new XAttribute("modified",DateTime.Now.ToString()));
        }

        protected XElement FindCommandNode(Command _command)
        {
            foreach (XElement _command_node in LogFile.Root.Elements())
            {
                if (String.Compare(_command_node.Name.LocalName,
                                  _command.Type.ToString(),
                                  StringComparison.OrdinalIgnoreCase) == 0)
                    return (_command_node);
            }
            XElement _new_command_node = new XElement(_command.Type.ToString());
            LogFile.Root.Add(_new_command_node);
            return (_new_command_node);
        }

        protected XAttribute FindXAttribute(XElement _xelement, string _xattribute_name)
        {
            if (_xattribute_name == null) return (null);
            return (
                _xelement.Attributes().FirstOrDefault(
                    _xattr => String.Compare(_xattr.Name.LocalName,
                                             _xattribute_name,
                                             StringComparison.OrdinalIgnoreCase) == 0));
        }

        protected void CreateCommandXElement(Command command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            CommandNodeXElement = FindCommandNode(command);
            CommandXElement = new XElement(command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString()),
                            new XAttribute("user", Program.User));
            AddCommandXElementParameters(command);
            CommandNodeXElement.Add(CommandXElement);
        }

        protected virtual void AddCommandXElementParameters(Command command)
        {
            
        }

        public virtual void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }

    }   
}
