// See https://aka.ms/new-console-template for more information
using RestSharp;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");

//using (HttpClient client = new HttpClient())    // TODO SK: Replace with IHttpClientFactory
//{
//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token");
//    var response = await client.GetAsync("https://www.google.com");
//    var content = await response.Content.ReadAsStringAsync();
//    Console.WriteLine(content);
//}

//string _auth_url = "https://www.strava.com/api/v3/oauth/authorize";
////string _token_url = "https://idp.bexio.com/token";
//string _client_id = "myid";
//string _client_secret = "mysecret";


//// Request token
//var restclient = new RestClient(_auth_url);
//RestRequest request = new RestRequest("request/oauth") { Method = Method.Post };
//request.AddHeader("Accept", "application/json");
//request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
////request.AddParameter("client_id", _client_id);
////request.AddParameter("client_secret", _client_secret);
//request.AddParameter("grant_type", "client_credentials");
//var tResponse = restclient.Execute(request);
//var responseJson = tResponse.Content;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddOAuth("strava", option =>
{
    option.ClientId = "";
    option.ClientSecret = "";
    option.AuthorizationEndpoint = "https://www.strava.com/oauth/authorize";
    option.TokenEndpoint = "https://www.strava.com/oauth/token";
    option.UserInformationEndpoint = "https://www.strava.com/api/v3/athlete";
    option.CallbackPath = "/authcallback/strava";
    //option.Scope.Add("read_all");

});

var app = builder.Build();

//app.Urls.Add("http://localhost:5000");

app.MapGet("/", (HttpContext ctx) =>
{
    return ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
});

app.MapGet("/login", (HttpContext ctx) => 
{ 
    return Results.Challenge(authenticationSchemes: new List<string> { "strava" });
});

app.Run();

