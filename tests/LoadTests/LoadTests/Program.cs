namespace LoadTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            int maxMilliseconds = 10000;
            int maxRate = 1;
            int executionTimeSeconds = 240;
            UserControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            CustomerControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            ProductControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            PortfolioControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
        }
    }
}