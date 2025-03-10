using AstroToolkitWeb.Data;
using AstroToolkitWeb.Services;
using Microsoft.EntityFrameworkCore;

// Make top-level statements async
await Host();

async Task Host()
{
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AstroToolkitDb"));

// Register services
builder.Services.AddScoped<AstroCalculationService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<DatabaseService>();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
    await DbInitializer.Initialize(dbContext, services, logger);
}

app.Run();
}