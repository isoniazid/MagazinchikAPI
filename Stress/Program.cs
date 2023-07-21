using NBomber.CSharp;
using NBomber.Configuration;
using NBomber.Contracts;
using NBomber.Http.CSharp;

class Program
{
    static void Main()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("Get All Products", async context =>
        {
            var response = await httpClient.GetAsync("http://localhost:5000/api/product/get-all?limit=50&page=0");

            return response.IsSuccessStatusCode
            ? Response.Ok()
            : Response.Fail();

        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 1000, interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner.RegisterScenarios(scenario).Run();
    }
}