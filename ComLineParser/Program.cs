using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComLineParser
{
    class Program
    {
        static readonly string DefaultUserName = "anonym";
        static string _user = DefaultUserName;
        public static string User
        {
            get
            {
                return (_user);
            }
            set
            {
                _user = string.IsNullOrEmpty(value) ? DefaultUserName : value;
            }
        }

        static void Main(string[] args)
        {
            while (!Commander.Exit)
            {
                Commander.ParseCommands(args);
                Commander.ProcessCommands();
                Commander.Exit = Commander.ConfirmExit();
                Thread.Sleep(100);
            }
        }
    }
}
