using System;
using System.Collections.Generic;

namespace ComLineParser
{
    public static class Commander
    {
        static Queue<Command> Commands = new Queue<Command>();
        static bool StartUpCommandsExecuted = false;
        static string _user_input;
        static string[] _user_commands;

        public static bool Exit = false;

        public static void ParseCommands(string[] _args)
        {
            if(_args.Length > 0 && StartUpCommandsExecuted == false)
            {
                StartUpCommandsExecuted = true;
                _user_commands = _args;
            }
            else
            {
                Console.WriteLine("\nEnter command or set of commands:");
                _user_input = Console.ReadLine();
                _user_commands = _user_input.Split((string[])null, StringSplitOptions.RemoveEmptyEntries);
            }
            Commands.Clear();
            if (_user_commands == null || _user_commands.Length == 0) return;
            for(int i = 0; i < _user_commands.Length;)
            {
                Command _command = ParseCommand(_user_commands[i]);
                _command.ParseParameters(_user_commands, ref i);
                Commands.Enqueue(_command);
                i++;
            }
        }

        public static void ProcessCommands()
        {
            Command cmd;
            while (Commands.Count != 0)
            {
                cmd = Commands.Dequeue();
                cmd.Action();
            }
        }

        public static Command ParseCommand(string _value)
        {
            switch (_value)
            {
                case "/?":
                case "/help":
                case "-help":
                    return (new HelpCommand());
                case "-exit":
                case "-quit":
                    return (new ExitCommand());
                case "-k":
                    return (new KeyValueCommand());
                case "-ping":
                    return (new PingCommand());
                case "-print":
                    return (new PrintCommand());
                default:
                    return (new UnsupportedCommand(_value));
            }
        }

        public static bool ConfirmExit()
        {
            if(Exit == true)
            {
                Console.WriteLine("Are you sure you want to terminate application ? [y] - terminate");
                ConsoleKeyInfo _result = Console.ReadKey();
                if (_result.Key == ConsoleKey.Y) return (true);
                Console.Write(" - OK ! Working on...\n");
            }
            return (false);
        }
    }
}

