using System;

namespace VkCelebrationScheduler.Logging
{
    public class SimpleConsoleLogger : ISimpleLogger
    {
        public void WriteException(Exception ex)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            Console.ForegroundColor = color;
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
