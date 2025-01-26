using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NBomber.CSharp;
using NBomber.Contracts;

namespace LoadTests
{
    public class CustomerControllerLoadTests
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

            var scenario = Scenario.Create("Customer_Controller", async context =>
            {
                var createCustomerStep = Step.Run("create_customer", context, async () =>
                {
                    Console.WriteLine($"POST {url}/Customer");

                    int randomId = new Random().Next(1, 100000);
                    var json = $"{{\"fullName\":\"Test Customer {randomId}\",\"user\":\"testcustomer{randomId}\"}}";

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{url}/Customer", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var updateCustomerStep = Step.Run("update_customer", context, async () =>
                {
                    int randomId = new Random().Next(1, 100);
                    Console.WriteLine($"PUT {url}/Customer");
                    var json = $"{{\"id\":{randomId},\"fullName\":\"Updated Customer\",\"user\":\"updatedcustomer\"}}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"{url}/Customer", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getAllCustomersStep = Step.Run("get_all_customers", context, async () =>
                {
                    Console.WriteLine($"GET {url}/Customer");
                    var response = await httpClient.GetAsync($"{url}/Customer");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var deleteCustomerStep = Step.Run("delete_customer", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"DELETE {url}/Customer/{randomId}");
                    var response = await httpClient.DeleteAsync($"{url}/Customer/{randomId}");
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
    }
}
