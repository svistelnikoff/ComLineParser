# ComLineParser
Command Line Parser for Kottans, C#

Project created with Visual Studio 2015 Community Edition (default settings for C# console application).

Application parses user input commands from console and executes them. All executed commands are saved
in command log file (xml) located in the same directory with application.  

Available commands
/? /help -help   - help command.
                   Prints this message and list of available commands.

-exit -quit      - exit command.
                   Terminates application.

-k               - key-value command.
                   Creates key-value pairs from user input.
                   User input <-k 1 qwerty 2> will result two pairs
                   to be created: 1-qwerty, 2-null.

-ping            - pinging command.
                   Creates fixed length "beep" sound from pc speaker.

-print           - print command.
                   Prints message followed after command into console window.
