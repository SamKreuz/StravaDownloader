using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OAuthTest.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AuthenticateStrava(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
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

                    option.ClaimActions.MapJsonKey("Id", "id");
                    option.ClaimActions.MapJsonKey("Firstname", "firstname");
                    option.ClaimActions.MapJsonKey("Lastname", "lastname");

                    option.SaveTokens = true;

                    option.Events.OnCreatingTicket = ctx => { return GetUserInfo(ctx); };
                });

            return builder;
        }

        private static async Task GetUserInfo(OAuthCreatingTicketContext ctx)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);
            using var result = await ctx.Backchannel.SendAsync(request);
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();
            ctx.RunClaimActions(user);
        }
    }
}
