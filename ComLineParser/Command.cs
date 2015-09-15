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
        protected ECommandTypes _type;
        public ECommandTypes Type => (_type);
        protected string _name;
        public string Name => (_name);
        protected string[] _parameters;
        public string[] Parameters => (_parameters);
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

        public static bool StringIsCommand(string value)
        {
            return AvailableCommands.SelectMany(T => T).ToList().
                    Any(cmdPattern => 
                            value.Equals(cmdPattern, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class HelpCommand : Command
    {
        public HelpCommand()
        {
            Description = "Help command invoked";
            _name = "Help";
            _type = ECommandTypes.Help;
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
            _name = "KeyValue";
            _type = ECommandTypes.KeyValue;
            CommandLogger = new KeyValueCommandLogger();
        }

        public override void Action()
        {
            Console.Write(Description);
            if (_parameters != null)
            {
                Console.WriteLine(" - creating pairs:");
                for (int i = 0; i < _parameters.Length; i++)
                {
                    Console.Write(_parameters[i++] + " = ");
                    Console.Write(_parameters[i] + ";\n");
                }
            }
            else
            {
                Console.WriteLine(" without _parameters.");
            }
            CommandLogger.SaveToFile(this);
        }

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            int paramCount = 0, paramIndex = commandIndex+1;
            while (paramIndex < _params.Length)
            {
                if (StringIsCommand(_params[paramIndex])) break;
                paramIndex++;
                paramCount++;
            }
            if (paramCount == 0) return;

            if (paramCount % 2 != 0)
            {
                _parameters = new string[paramCount+1];
                Array.Copy(_params, commandIndex+1, _parameters, 0, paramCount);
                _parameters[paramCount] = "NULL";
            }
            else
            {
                _parameters = new string[paramCount];
                Array.Copy(_params, commandIndex+1, _parameters, 0, paramCount);
            }
            commandIndex += paramCount;
        }
    }

    public class PingCommand : Command
    {
        public PingCommand()
        {
            Description = "Pinging command invoked";
            _name = "Ping";
            _type = ECommandTypes.Ping;
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
            _name = "Print";
            _type = ECommandTypes.Print;
            CommandLogger = new PrintCommandLogger();
        }

        public override void Action()
        {
            if(_parameters == null || _parameters[0] == string.Empty)
            {
                Console.WriteLine(Description + ", message is empty");
            }
            else
            {
                Console.WriteLine(Description + " - printing message:" + _parameters[0]);
            }
            CommandLogger.SaveToFile(this);
        }

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            _parameters = new string[]{ string.Empty };
            int parameterIndex = commandIndex + 1;
            while (parameterIndex < _params.Length && !StringIsCommand(_params[parameterIndex]))
            {
                _parameters[0] += _params[parameterIndex++];
                if (parameterIndex < _params.Length) _parameters[0] += " ";
                commandIndex++; 
            }
        }

    }

    public class SetUserCommand : Command
    {
        public SetUserCommand()
        {
            Description = "Set user command invoked";
            _name = "SetUser";
            _type = ECommandTypes.SetUser;
            CommandLogger = new SetUserCommandLogger();
        }

        public override void Action()
        {
            if (_parameters == null || _parameters[0] == string.Empty)
            {
                Console.WriteLine(Description + ", current user is reseted.");
                CommandLogger.SaveToFile(this);
                Program.User = string.Empty;
            }
            else
            {
                Console.WriteLine(Description + " - new user is set:" + _parameters[0]);
                CommandLogger.SaveToFile(this);
                Program.User = _parameters[0];
            }
        }

        public override void ParseParameters(string[] _params, ref int commandIndex)
        {
            _parameters = new string[] { string.Empty };
            var parameterIndex = commandIndex + 1;
            if (parameterIndex >= _params.Length || StringIsCommand(_params[parameterIndex])) return;
            _parameters[0] = _params[parameterIndex];
            commandIndex++;
        }
    }

    public class GetUserCommand : Command
    {
        public GetUserCommand()
        {
            Description = "Get user command invoked";
            _name = "GetUser";
            _type = ECommandTypes.GetUser;
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
            _name = "Exit";
            _type = ECommandTypes.Exit;
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

        public UnsupportedCommand(string value)
        {
            Description = "Unsupported command";
            _name = "Unsupported";
            _type = ECommandTypes.Unsupported;
            Value = value;
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