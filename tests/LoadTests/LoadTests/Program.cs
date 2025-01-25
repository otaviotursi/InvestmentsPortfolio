namespace LoadTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            int maxMilliseconds = 100;
            int maxRate = 10;
            int executionTimeSeconds = 20;
            //UserControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            //CustomerControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            ProductControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
            PortfolioControllerLoadTests.Run(maxMilliseconds, maxRate, executionTimeSeconds);
        }
    }
}