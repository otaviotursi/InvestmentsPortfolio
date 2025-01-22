namespace LoadTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            int maxMilliseconds = 100;
            int maxRate = 100;
            int executionTimeSeconds = 600;
            UserControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
        }
    }
}