//using Microsoft.AspNetCore.Authentication;
//using OAuthTest;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication("cookie")
//    .AddCookie("cookie")
//    .AddOAuth("github", option =>
//    {
//        option.SignInScheme = "cookie";
//        option.ClientId = "Ov23liH4NqcEN5ij43L0";
//        option.ClientSecret = "a207807e01bbca5cef1ea090000c700f897caea2";

//        option.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
//        option.TokenEndpoint = "https://github.com/login/oauth/access_token";
//        option.CallbackPath = "/oauth/github-cb";

//        option.SaveTokens = true;

//        option.UserInformationEndpoint = "https://api.github.com/user";
//    });

//var app = builder.Build();

//app.UseAuthentication();

//app.MapGet("/", (HttpContext ctx) =>
//{
//    //return ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
//    return "test";
//});

//app.MapGet("/login", () =>
//{
//    return Results.Challenge(
//        new AuthenticationProperties() { RedirectUri = "https://localhost:5005/" },
//        authenticationSchemes: new List<string>() { "github" });
//});

//app.Run();

//using OAuthTest;

//new OAuthTest.Program().Run();