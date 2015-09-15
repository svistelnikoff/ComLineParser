using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ComLineParser
{
    public enum ECommandTypes
    {
        Unsupported = 0,
        Help = 1,
        KeyValue = 2,
        Ping = 3,
        Print = 4,
        Exit = 5,
        SetUser = 6,
        GetUser = 7
    }

    public abstract class Command
    {
        protected ECommandTypes type;
        public ECommandTypes Type
        {
            get { return (type); }
        }
        protected string name;
        public string Name
        {
            get { return (name); }
        }
        protected string[] parameters;
        public string[] Parameters
        {
            get { return (parameters); }
        }
        protected string Description;
        protected ICommandLogger CommandLogger;

        public static readonly string[] HelpCommands =     { "/?", "/help", "-help" };
        public static readonly string[] ExitCommands =     { "-exit", "-quit" };
        public static readonly string[] KeyValueCommands = { "-k" };
        public static readonly string[] PingCommands =     { "-ping" };
        public static readonly string[] PrintCommands =    { "-print" };

        public static readonly string[][] AvailableCommands =
        {
            HelpCommands,
            ExitCommands,
            KeyValueCommands,
            PingCommands,
            PrintCommands
        };

        public abstract void Action();
        public virtual void ParseParameters(string[] _params, ref int _index)
        {
             
        }

        public static bool StringIsCommand(string _value)
        {
            foreach (string _cmd_pattern in 
                Command.AvailableCommands.
                SelectMany(T => T).ToList())
            {
                if (String.Compare(_value, _cmd_pattern, StringComparison.OrdinalIgnoreCase) == 0) return (true);
            }
            return (false);
        }

    }

    public class HelpCommand : Command
    {
        public HelpCommand()
        {
            this.Description = "Help command invoked";
            this.name = "Help";
            this.type = ECommandTypes.Help;
            CommandLogger = new GenericCommandLogger();
        }

        public override void Action()
        {
            Console.WriteLine(Description);
            //show help section
            foreach (string str in HelpCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t - help command.\n\t\t   Prints this message and list of available commands.\n");
            //show exit section
            foreach (string str in ExitCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t - exit command.\n\t\t   Terminates application.\n");
            //show key-vlue section
            foreach (string str in KeyValueCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t\t - key-value command.\n\t\t   Creates key-value pairs from user input.");
            Console.WriteLine("\t\t   User input <-k 1 qwerty 2> will result two pairs\n\t\t   to be created: 1-qwerty, 2-null.\n");
            //show ping section
            foreach (string str in PingCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t\t - pinging command.\n\t\t   Creates fixed length \"beep\" sound from pc speaker.\n");
            //show print section
            foreach (string str in PrintCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t\t - print command.\n\t\t   Prints message followed after command into console window.\n");
            CommandLogger.SaveToFile(this);
        }
    }
    
    public class KeyValueCommand : Command
    {
        public KeyValueCommand()
        {
            this.Description = "Key-Value command invoked";
            this.name = "KeyValue";
            this.type = ECommandTypes.KeyValue;
            CommandLogger = new KeyValueCommandLogger();
        }

        public override void Action()
        {
            Console.Write(Description);
            if (parameters != null)
            {
                Console.WriteLine(" - creating pairs:");
                for (int i = 0; i < parameters.Length; i++)
                {
                    Console.Write(parameters[i++] + " = ");
                    Console.Write(parameters[i] + ";\n");
                }
            }
            else
            {
                Console.WriteLine(" without parameters.");
            }
            CommandLogger.SaveToFile(this);
        }

        public override void ParseParameters(string[] _params, ref int _command_index)
        {
            int _param_count = 0, _param_index = _command_index+1;
            while (_param_index < _params.Length)
            {
                if (Command.StringIsCommand(_params[_param_index])) break;
                _param_index++;
                _param_count++;
            }
            if (_param_count == 0) return;

            if (_param_count % 2 != 0)
            {
                parameters = new string[_param_count+1];
                Array.Copy(_params, _command_index+1, parameters, 0, _param_count);
                parameters[_param_count] = "NULL";
            }
            else
            {
                parameters = new string[_param_count];
                Array.Copy(_params, _command_index+1, parameters, 0, _param_count);
            }
            _command_index += _param_count;
        }
    }

    public class PingCommand : Command
    {
        public PingCommand()
        {
            this.Description = "Pinging command invoked";
            this.name = "Ping";
            this.type = ECommandTypes.Ping;
            CommandLogger = new GenericCommandLogger();
        }

        public override void Action()
        {
            Console.WriteLine(Description + " - pinging...");
            (new Thread(()=> Console.Beep(500, 1000))).Start();
            CommandLogger.SaveToFile(this);
        }

    }

    public class PrintCommand : Command
    {
        public PrintCommand()
        {
            this.Description = "Print command invoked";
            this.name = "Print";
            this.type = ECommandTypes.Print;
            CommandLogger = new PrintCommandLogger();
        }

        public override void Action()
        {
            if(parameters == null || parameters[0] == String.Empty)
            {
                Console.WriteLine(Description + ", message is empty");
            }
            else
            {
                Console.WriteLine(Description + " - printing message:" + parameters[0]);
            }
            CommandLogger.SaveToFile(this);
        }

        public override void ParseParameters(string[] _params, ref int _command_index)
        {
            parameters = new string[1] { String.Empty };
            int _parameter_index = _command_index + 1;
            while (_parameter_index < _params.Length && !StringIsCommand(_params[_parameter_index]))
            {
                parameters[0] += _params[_parameter_index++];
                if (_parameter_index < _params.Length) parameters[0] += " ";
                _command_index++; 
            }
        }

    }

    public class SetUserCommand : Command
    {
        public SetUserCommand()
        {
            this.Description = "Set user command invoked";
            this.name = "SetUser";
            this.type = ECommandTypes.Print;
            CommandLogger = new PrintCommandLogger();
        }

        public override void Action()
        {
            if (parameters == null || parameters[0] == String.Empty)
            {
                Console.WriteLine(Description + ", message is empty");
            }
            else
            {
                Console.WriteLine(Description + " - printing message:" + parameters[0]);
            }
            CommandLogger.SaveToFile(this);
        }

        public override void ParseParameters(string[] _params, ref int _command_index)
        {
            parameters = new string[1] { String.Empty };
            int _parameter_index = _command_index + 1;
            while (_parameter_index < _params.Length && !StringIsCommand(_params[_parameter_index]))
            {
                parameters[0] += _params[_parameter_index++];
                if (_parameter_index < _params.Length) parameters[0] += " ";
                _command_index++;
            }
        }

    }


    public class ExitCommand : Command
    {
        public ExitCommand()
        {
            this.Description = "Exit command invoked";
            this.name = "Exit";
            this.type = ECommandTypes.Exit;
            CommandLogger = new GenericCommandLogger();
        }
        public override void Action()
        {
            Console.WriteLine(Description);
            Commander.Exit = true;
            CommandLogger.SaveToFile(this);
        }
    }

    public class UnsupportedCommand : Command
    {
        string _command_value;
        public string Value
        {
            get { return (_command_value); }
        }
        public UnsupportedCommand(string _value)
        {
            this.Description = "Unsupported command";
            this.name = "Unsupported";
            this.type = ECommandTypes.Unsupported;
            _command_value = _value;
            CommandLogger = new UnsupportedCommandLogger();
        }

        public override void Action()
        {
            Console.WriteLine("Command <" + _command_value+ "> - " + "is not supported.");
            Console.WriteLine("Use </?> to see set of allowed commands, <-exit> to terminate");
            CommandLogger.SaveToFile(this);
        }
    }
}