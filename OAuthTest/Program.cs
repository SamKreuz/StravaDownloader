using Microsoft.AspNetCore.Authentication;
using OAuthTest.Extensions;
using OAuthTest.Models;
using System.Net.Http.Headers;
using System.Text;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddHttpClient();
        builder.AuthenticateStrava();

        var app = builder.Build();

        app.UseAuthentication();

        app.MapGet("/", (HttpContext ctx) =>
        {
            var claim = ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return claim;
        });

        app.MapGet("/login", () =>
        {
            return Results.Challenge(
                new AuthenticationProperties() { 
                    RedirectUri = "https://localhost:5005/" }, 
                    authenticationSchemes: new List<string>() { "strava" });

            //authenticationSchemes: new List<string>() { "github" });
        });

        //app.Map("/strava-cb", () => Results.Redirect("/"));

        app.MapGet("/getroutes", async (HttpContext ctx, IHttpClientFactory factory) =>
        {
            var list = await GetRoutes(ctx, factory);

            var orderedList = list.OrderBy(x => x.name);

            var sb = new StringBuilder();

            foreach (var item in orderedList)
            {
                sb.AppendLine(item.name + "\n");
            }

            return sb.ToString();
        });

        async Task<List<Activity>> GetRoutes(HttpContext ctx, IHttpClientFactory factory)
        {
            var token = ctx.GetTokenAsync("access_token").Result;

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.strava.com/api/v3/athlete/activities?page=1&per_page=40");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var result = factory.CreateClient().SendAsync(request).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Error");
            }

            var resultString = await result.Content.ReadAsStringAsync();

            var data = await result.Content.ReadFromJsonAsync<List<Activity>>();

            if(data == null)
            {
               return new List<Activity>();
            }

            return data;
        }

        app.Run();
    }
}