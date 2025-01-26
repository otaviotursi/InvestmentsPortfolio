using NBomber.CSharp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LoadTests
{
    public class UserControllerLoadTests
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

            var scenario = Scenario.Create("User_Controller", async context =>
            {
                var createUserStep = Step.Run("create_user", context, async () =>
                {
                    Console.WriteLine($"POST {url}/User"); 
                    int randomId = new Random().Next(1, 100000);
                    var json = $"{{\"fullName\":\"Test User {randomId}\",\"user\":\"testuser{randomId}\"}}";

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{url}/User", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var updateUserStep = Step.Run("update_user", context, async () =>
                {
                    int randomId = new Random().Next(1, 100);
                    Console.WriteLine($"PUT {url}/User");
                    var json = $"{{\"id\":{randomId},\"fullName\":\"Updated User\",\"user\":\"updateduser\"}}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"{url}/User", content);
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var getAllUsersStep = Step.Run("get_all_users", context, async () =>
                {
                    int randomId = new Random().Next(1, 100);
                    Console.WriteLine($"GET {url}/User");
                    var response = await httpClient.GetAsync($"{url}/User");
                    Console.WriteLine($"Response: {response}");
                    if (response.IsSuccessStatusCode && response.Headers.Date.HasValue)
                    {
                        var responseTime = DateTime.UtcNow - response.Headers.Date.Value.UtcDateTime;
                        return responseTime.TotalMilliseconds < maxMilliseconds ? Response.Ok() : Response.Fail();
                    }
                    return Response.Fail();
                });

                var deleteUserStep = Step.Run("delete_user", context, async () =>
                {
                    int randomId = new Random().Next(1, 10);
                    Console.WriteLine($"DELETE {url}/User/{randomId}");
                    var response = await httpClient.DeleteAsync($"{url}/User/{randomId}");
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
