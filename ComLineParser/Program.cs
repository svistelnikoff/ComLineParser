using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComLineParser
{
    class Program
    {
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
