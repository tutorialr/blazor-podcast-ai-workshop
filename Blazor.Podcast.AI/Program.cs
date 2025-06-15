using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor.Podcast.AI;
using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var credential = new ApiKeyCredential("PersonalAccessToken");
var options = new OpenAIClientOptions() 
{ 
    Endpoint = new Uri("https://models.inference.ai.azure.com") 
}; 
var client = new OpenAIClient(credential, options);
var chat = client.GetChatClient("gpt-4o-mini").AsIChatClient();
builder.Services.AddSingleton(chat);
// Register Provider
builder.Services.AddSingleton<Provider>();

await builder.Build().RunAsync();
