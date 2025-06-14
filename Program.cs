using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
builder.Services.AddSingleton<MerkleTreeService>();

var app = builder.Build();

// Enable routing to controllers
app.MapControllers();

// Run the web app
app.Run();