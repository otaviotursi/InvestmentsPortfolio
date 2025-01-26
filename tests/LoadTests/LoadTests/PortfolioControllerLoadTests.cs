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
                // Variável estática para armazenar o GUID
                Guid currentGuid = Guid.NewGuid();
                int deleteCounter = 0;

                var createPortfolioStep = Step.Run("create_portfolio", context, async () =>
                {
                    Console.WriteLine($"POST {url}/Portfolio");

                    // Dados de exemplo para o JSON
                    var operationTypes = new[] { "buy", "sell" };
                    var random = new Random();

                    var json = $@"
                {{
                    ""productId"": ""{currentGuid}"",
                    ""customerId"": 1,
                    ""productName"": ""aapl"",
                    ""amountNegotiated"": 100,
                    ""operationType"": ""{operationTypes[random.Next(operationTypes.Length)]}""
                }}";

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


                var getAllPortfoliosStep = Step.Run("get_all_portfolios", context, async () =>
                {
                    Console.WriteLine($"GET {url}/Portfolio");
                    var response = await httpClient.GetAsync($"{url}/Portfolio");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getPortfoliosStatementStep = Step.Run("get_by_customerId", context, async () =>
                {
                    var random = new Random();

                    Console.WriteLine($"GET {url}/Portfolio/Statement");
                    var response = await httpClient.GetAsync($"{url}/Portfolio/Statement?customerId={random.NextInt64(0,20)}");
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

            scenario = scenario.WithoutWarmUp().WithLoadSimulations(
                Simulation.Inject(rate: maxRate, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(executionTimeSeconds))
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
                    var random = new Random();
                    var randomIndex = random.Next(0, productsArray.Count); // Gera um índice aleatório
                    var selectedProduct = productsArray[randomIndex];

                    productId = selectedProduct["id"].ToString();
                    productName = selectedProduct["name"].ToString();
                }
            }
            return (productId, productName);

        }
    }
}
