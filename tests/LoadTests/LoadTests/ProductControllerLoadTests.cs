using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;

namespace LoadTests
{
    public class ProductControllerLoadTests
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

            var scenario = Scenario.Create("Product_Controller", async context =>
            {
                var createProductStep = Step.Run("create_product", context, async () =>
                {
                    Console.WriteLine($"POST {url}/Product");
                    var json = "{\"name\":\"Test Product\",\"description\":\"Test Description\",\"price\":100.0}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{url}/Product", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var updateProductStep = Step.Run("update_product", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"PUT {url}/Product");
                    var json = $"{{\"id\":{randomId},\"name\":\"Updated Product\",\"description\":\"Updated Description\",\"price\":150.0}}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"{url}/Product", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getAllProductsStep = Step.Run("get_all_products", context, async () =>
                {
                    Console.WriteLine($"GET {url}/Product");
                    var response = await httpClient.GetAsync($"{url}/Product");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var deleteProductStep = Step.Run("delete_product", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"DELETE {url}/Product/{randomId}");
                    var response = await httpClient.DeleteAsync($"{url}/Product/{randomId}");
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
    }
}
