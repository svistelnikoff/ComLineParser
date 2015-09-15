using System;
using System.Collections.Generic;

namespace ComLineParser
{
    public static class Commander
    {
        static readonly Queue<Command> Commands = new Queue<Command>();
        static bool _startUpCommandsExecuted;
        static string _userInput;
        static string[] _userCommands;

        public static bool Exit = false;

        public static void ParseCommands(string[] args)
        {
            if(args.Length > 0 && _startUpCommandsExecuted == false)
            {
                _startUpCommandsExecuted = true;
                _userCommands = args;
            }
            else
            {
                Console.WriteLine("\nEnter command or set of commands:");
                _userInput = Console.ReadLine();
                if (_userInput != null)
                    _userCommands = _userInput.Split((string[]) null, StringSplitOptions.RemoveEmptyEntries);
                else
                    _userCommands = null;

            }
            Commands.Clear();
            if (_userCommands == null || _userCommands.Length == 0) return;
            for(var i = 0; i < _userCommands.Length;)
            {
                var command = ParseCommand(_userCommands[i]);
                command.ParseParameters(_userCommands, ref i);
                Commands.Enqueue(command);
                i++;
            }
        }

        public static void ProcessCommands()
        {
            while (Commands.Count != 0)
            {
                var cmd = Commands.Dequeue();
                cmd.Action();
            }
        }

        public static Command ParseCommand(string value)
        {
            switch (value)
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
                case "-getuser":
                    return (new GetUserCommand());
                case "-setuser":
                    return (new SetUserCommand());
                default:
                    return (new UnsupportedCommand(value));
            }
        }

        public static bool ConfirmExit()
        {
            if (!Exit) return (false);
            Console.WriteLine("Are you sure you want to terminate application ? [y] - terminate");
            var result = Console.ReadKey();
            if (result.Key == ConsoleKey.Y) return (true);
            Console.Write(" - OK ! Working on...\n");
            return (false);
        }
    }
}

