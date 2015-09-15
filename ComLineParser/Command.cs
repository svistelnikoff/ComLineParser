using System;
using System.Linq;
using System.Threading;


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
        public ECommandTypes Type => (type);
        protected string name;
        public string Name => (name);
        protected string[] parameters;
        public string[] Parameters => (parameters);
        protected string Description;
        protected ICommandLogger CommandLogger;

        public static readonly string[] HelpCommands =     { "/?", "/help", "-help" };
        public static readonly string[] ExitCommands =     { "-exit", "-quit" };
        public static readonly string[] KeyValueCommands = { "-k" };
        public static readonly string[] PingCommands =     { "-ping" };
        public static readonly string[] PrintCommands =    { "-print" };
        public static readonly string[] GetUserCommands =  { "-getuser"};
        public static readonly string[] SetUserCommands =  { "-setuser" };

        public static readonly string[][] AvailableCommands =
        {
            HelpCommands,
            ExitCommands,
            KeyValueCommands,
            PingCommands,
            PrintCommands,
            GetUserCommands,
            SetUserCommands
        };

        public abstract void Action();
        public virtual void ParseParameters(string[] _params, ref int commandIndex)
        {
             
        }

        public static bool StringIsCommand(string _value)
        {
            return AvailableCommands.SelectMany(T => T).ToList().
                    Any(_cmd_pattern => 
                            String.Compare(_value, _cmd_pattern, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }

    public class HelpCommand : Command
    {
        public HelpCommand()
        {
            Description = "Help command invoked";
            name = "Help";
            type = ECommandTypes.Help;
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
            //show getuser section
            foreach (string str in GetUserCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t - get user command.\n\t\t   Prints current username. Default username is <anonym>.\n");
            //show setuser section
            foreach (string str in SetUserCommands)
                Console.Write(str + " ");
            Console.WriteLine("\t - set user command.\n\t\t   Changes current user. User input <-setuser qwerty> will");
            Console.WriteLine("\t\t   change current user to <qwerty>.\n");
            CommandLogger.SaveToFile(this);
        }
    }
    
    public class KeyValueCommand : Command
    {
        public KeyValueCommand()
        {
            Description = "Key-Value command invoked";
            name = "KeyValue";
            type = ECommandTypes.KeyValue;
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

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            int _param_count = 0, _param_index = commandIndex+1;
            while (_param_index < _params.Length)
            {
                if (StringIsCommand(_params[_param_index])) break;
                _param_index++;
                _param_count++;
            }
            if (_param_count == 0) return;

            if (_param_count % 2 != 0)
            {
                parameters = new string[_param_count+1];
                Array.Copy(_params, commandIndex+1, parameters, 0, _param_count);
                parameters[_param_count] = "NULL";
            }
            else
            {
                parameters = new string[_param_count];
                Array.Copy(_params, commandIndex+1, parameters, 0, _param_count);
            }
            commandIndex += _param_count;
        }
    }

    public class PingCommand : Command
    {
        public PingCommand()
        {
            Description = "Pinging command invoked";
            name = "Ping";
            type = ECommandTypes.Ping;
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
            Description = "Print command invoked";
            name = "Print";
            type = ECommandTypes.Print;
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

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            parameters = new string[]{ String.Empty };
            int _parameter_index = commandIndex + 1;
            while (_parameter_index < _params.Length && !StringIsCommand(_params[_parameter_index]))
            {
                parameters[0] += _params[_parameter_index++];
                if (_parameter_index < _params.Length) parameters[0] += " ";
                commandIndex++; 
            }
        }

    }

    public class SetUserCommand : Command
    {
        public SetUserCommand()
        {
            Description = "Set user command invoked";
            name = "SetUser";
            type = ECommandTypes.SetUser;
            CommandLogger = new SetUserCommandLogger();
        }

        public override void Action()
        {
            if (parameters == null || parameters[0] == string.Empty)
            {
                Console.WriteLine(Description + ", current user is reseted.");
                CommandLogger.SaveToFile(this);
                Program.User = string.Empty;
            }
            else
            {
                Console.WriteLine(Description + " - new user is set:" + parameters[0]);
                CommandLogger.SaveToFile(this);
                Program.User = parameters[0];
            }
        }

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            parameters = new string[] { string.Empty };
            var parameterIndex = commandIndex + 1;
            if (parameterIndex >= _params.Length || StringIsCommand(_params[parameterIndex])) return;
            parameters[0] = _params[parameterIndex];
            commandIndex++;
        }
    }

    public class GetUserCommand : Command
    {
        public GetUserCommand()
        {
            Description = "Get user command invoked";
            name = "GetUser";
            type = ECommandTypes.GetUser;
            CommandLogger = new GetUserCommandLogger();
        }

        public override void Action()
        {
            Console.WriteLine(Description + ", current user is - " + Program.User);
            CommandLogger.SaveToFile(this);
       }
    }


    public class ExitCommand : Command
    {
        public ExitCommand()
        {
            Description = "Exit command invoked";
            name = "Exit";
            type = ECommandTypes.Exit;
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
        public string Value { get; }

        public UnsupportedCommand(string _value)
        {
            Description = "Unsupported command";
            name = "Unsupported";
            type = ECommandTypes.Unsupported;
            Value = _value;
            CommandLogger = new UnsupportedCommandLogger();
        }

        public override void Action()
        {
            Console.WriteLine("Command <" + Value+ "> - " + "is not supported.");
            Console.WriteLine("Use </?> to see set of allowed commands, <-exit> to terminate");
            CommandLogger.SaveToFile(this);
        }
    }
}