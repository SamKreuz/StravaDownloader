using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace OAuthTest.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AuthenticateStrava(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(a =>
            {
                a.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //a.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //a.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
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
                });

            return builder;
        }
    }
}
