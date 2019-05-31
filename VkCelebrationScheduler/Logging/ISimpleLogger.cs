using System;

namespace VkCelebrationScheduler.Logging
{
    public interface ISimpleLogger
    {
        void WriteException(Exception ex);

        void WriteLine(string text);
    }
}