using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;
using Newtonsoft.Json.Linq;

namespace LoadTests
{

    public class PortfolioControllerLoadTests
    {
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static void Run(int maxMilliseconds, int maxRate, int executionTimeSeconds)
        {
            string url = "https://localhost:44359";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var scenario = Scenario.Create("Portfolio_Controller", async context =>
            {
                (string productId, string productName) = await getProductAsync(url);
                var operatePortfolioStep = Step.Run("operate_portfolio", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"POST {url}/Portfolio");
                    var json = "{\"productId\":\"00000000-0000-0000-0000-000000000000\",\"customerId\":1,\"productName\":\"Test Product\",\"amountNegotiated\":1000,\"operationType\":\"Buy\"}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{url}/Portfolio", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getPortfolioStep = Step.Run("get_portfolio", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"GET {url}/Portfolio?customerId={randomId}");
                    var response = await httpClient.GetAsync($"{url}/Portfolio?customerId={{randomId}}\"");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getPortfolioStatementStep = Step.Run("get_portfolio_statement", context, async () =>
                {
                    Console.WriteLine($"GET {url}/Portfolio/statement");
                    var response = await httpClient.GetAsync($"{url}/Portfolio/statement");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                return Response.Ok();
            });

            scenario = scenario.WithLoadSimulations(
                Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(0), during: TimeSpan.FromSeconds(10)),
                Simulation.RampingInject(rate: maxRate, interval: TimeSpan.FromSeconds(10), during: TimeSpan.FromSeconds(executionTimeSeconds))
            );

            NBomberRunner.RegisterScenarios(scenario).Run();
        }

        public static async Task<(string productId, string productName)> getProductAsync(string url)
        {
            // Step to get all products
            Console.WriteLine($"GET {url}/Product");
            var getAllProductsResponse = await httpClient.GetAsync($"{url}/Product");
            Console.WriteLine($"Response: {getAllProductsResponse}");
            string productId = "00000000-0000-0000-0000-000000000000";
            string productName = "Test Product";

            if (getAllProductsResponse.IsSuccessStatusCode)
            {
                var responseBody = await getAllProductsResponse.Content.ReadAsStringAsync();
                var productsArray = JArray.Parse(responseBody);
                if (productsArray.Count > 0)
                {
                    var firstProduct = productsArray[0];
                    productId = firstProduct["id"].ToString();
                    productName = firstProduct["name"].ToString();
                }
            }
            return (productId, productName);

        }
    }
}
