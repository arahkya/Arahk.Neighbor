using Arahk.Neighbor.Application;
using Arahk.Neighbor.Infrastructure.DependencyInjection;
using Arahk.Neighbor.Infrastructure.Dev;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Web.Components;
using Arahk.Neighbor.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<AuthSessionState>();
builder.Services.AddScoped<AppShellState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Phase A: create SQLite schema if missing (EnsureCreated). Seed only fills empty rows.
await NeighborDbInitializer.EnsureCreatedAsync(app.Services);

// Development-only verified demo user — never runs in Production; skips if email exists.
await DevUserSeeder.SeedIfDevelopmentAsync(app.Services, app.Environment);

// Development role + permission catalog + defaults; never overwrites existing rows.
await DevRolePermissionSeeder.SeedIfDevelopmentAsync(app.Services, app.Environment);

app.Run();
