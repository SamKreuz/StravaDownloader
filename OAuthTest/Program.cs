using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using OAuthTest;
using OAuthTest.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        OAuthCreatingTicketContext contextData = null;

        //builder.AuthenticateStrava();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOAuth("strava", option =>
            {
                option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                option.ClientId = Config.ClientId;
                option.ClientSecret = Config.ClientSecret;

                option.Scope.Add("activity:read_all");
                option.AuthorizationEndpoint = "http://www.strava.com/oauth/authorize";
                option.TokenEndpoint = "https://www.strava.com/oauth/token";
                option.CallbackPath = "/strava-cb";
                option.UserInformationEndpoint = "https://www.strava.com/api/v3/athlete";

                option.SaveTokens = true;

                option.ClaimActions.MapJsonKey("sub", "id");
                option.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

                //option.ClaimActions.MapJsonKey("Id", "id");
                //option.ClaimActions.MapJsonKey("Firstname", "firstname");
                //option.ClaimActions.MapJsonKey("Lastname", "lastname");

                option.Events.OnCreatingTicket = ctx => { return GetUserInfo(ctx); };
                //option.Events. = new OAuthEvents
                //{
                //    OnMessageReceived = ctx =>
                //    {
                //        ctx.Token = ctx.Request.Query["access_token"];
                //        return Task.CompletedTask;
                //    }
                //};
            });

        async Task GetUserInfo(OAuthCreatingTicketContext ctx)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            using var result = await ctx.Backchannel.SendAsync(request);
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();
            ctx.RunClaimActions(user);

            contextData = ctx;
        }

        //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .AddOAuth("github", option =>
        //    {
        //        option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //        option.ClientId = "Ov23liH4NqcEN5ij43L0";
        //        option.ClientSecret = "a207807e01bbca5cef1ea090000c700f897caea2";

        //        option.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        //        option.TokenEndpoint = "https://github.com/login/oauth/access_token";
        //        option.CallbackPath = "/oauth/github-cb";

        //        option.SaveTokens = true;

        //        option.UserInformationEndpoint = "https://api.github.com/user";

        //        //option.ClaimActions.MapJsonKey("sub", "id");
        //        //option.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

        //        //option.Events.OnCreatingTicket = async ctx =>
        //        //{
        //        //    using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
        //        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
        //        //    using var result = await ctx.Backchannel.SendAsync(request);
        //        //    var user = await result.Content.ReadFromJsonAsync<JsonElement>();
        //        //    ctx.RunClaimActions(user);
        //        //};
        //    });

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
                new AuthenticationProperties() { RedirectUri = "https://localhost:5005/" },
                authenticationSchemes: new List<string>() { "strava" });
                //authenticationSchemes: new List<string>() { "github" });
        });

        app.MapGet("/getroutes", async () =>
        {
            var list = await GetRoutes();

            return list.Select(x => x.name);
        });

        async Task<List<Activity>> GetRoutes()
        {
            if (contextData == null)
            {
                return new List<Activity>();
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.strava.com/api/v3/athlete/activities?page=1&per_page=2");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", contextData.AccessToken);
            using var result = await contextData.Backchannel.SendAsync(request);

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