using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
                            new XAttribute("modified", DateTime.Now.ToString(CultureInfo.CurrentCulture))
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
            if (!LogFile.Root.Name.LocalName.Equals(XDocRootName, StringComparison.OrdinalIgnoreCase))
                throw (new Exception("File doesn't contain valid root element"));
            var xattribute = FindXAttribute(LogFile.Root, "description");
            if (xattribute == null)
            {
                LogFile.Root.Add(new XAttribute("description", "command log file for ComLineParser"));
            }
            else
            {
                if (!xattribute.Value.Equals("command log file for ComLineParser"))
                    xattribute.SetValue("command log file for ComLineParser");
            }

            xattribute = FindXAttribute(LogFile.Root, "modified");
            if (xattribute == null)
            {
                LogFile.Root.Add(new XAttribute("modified", DateTime.Now.ToString(CultureInfo.CurrentCulture)));
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
           var xattribute = FindXAttribute(LogFile.Root, "modified");
           if (xattribute != null) xattribute.SetValue(DateTime.Now.ToString(CultureInfo.CurrentCulture));
            else LogFile.Root.Add(new XAttribute("modified",DateTime.Now.ToString(CultureInfo.CurrentCulture)));
        }

        protected XElement FindCommandNode(Command command)
        {
            foreach (XElement _command_node in LogFile.Root.Elements())
            {
                if (_command_node.Name.LocalName.
                    Equals(command.Type.ToString(), StringComparison.OrdinalIgnoreCase)) return (_command_node);
            }
            var newCommandNode = new XElement(command.Type.ToString());
            LogFile.Root.Add(newCommandNode);
            return (newCommandNode);
        }

        protected XAttribute FindXAttribute(XElement xelement, string xattributeName)
        {
            if (xattributeName == null) return (null);
            return (
                xelement.Attributes().FirstOrDefault(
                    xattr => xattr.Name.LocalName.Equals(xattributeName,StringComparison.OrdinalIgnoreCase)));
        }

        protected void CreateCommandXElement(Command command)
        {
            if (File.Exists(LogFilePath)) OpenLogFile();
            else CreateNewLogFile();

            CommandNodeXElement = FindCommandNode(command);
            CommandXElement = new XElement(command.Name,
                            new XAttribute("invoked", DateTime.Now.ToString(CultureInfo.CurrentCulture)),
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
