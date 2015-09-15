
namespace ComLineParser
{
    public class GetUserCommandLogger : BaseCommandLogger
    {
        public override void SaveToFile(Command command)
        {
            CreateCommandXElement(command);
            SaveLogFile();
        }
    }
}
