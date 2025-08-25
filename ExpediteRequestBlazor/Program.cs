using Microsoft.AspNetCore.Authorization;
using MudBlazor.Services;

using Indium.Common.Modules;
using Indium.Common.Modules.Auth;
using Indium.Infor.EFContexts;

using Indium.Common.EFContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ExpediteRequestBlazor.EFModels;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using ExpediteRequestBlazor.Modules.Repositories.Interfaces;
using ExpediteRequestBlazor.Modules.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers(); //this may need to be changed back to AddControllersWithViews
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMudServices();

builder.Services.AddDevExpressBlazor();
builder.Services.AddScoped<IAuthorizationHandler, ActiveDirectoryAuthorizationHandler>();
builder.Services.AddScoped<IPermissionService, ActiveDirectoryPermissionService>();
builder.Services.AddScoped<Indium.Common.Models.User>();
builder.Services.AddScoped<ExpediteRequestBlazor.Shared.User>();
builder.Services.AddScoped<TimeZoneService>();

builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDropdownRepository, DropdownRepository>();
builder.Services.AddScoped<IExpediteRequestRepository, ExpediteRequestRepository>();

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDropdownService, DropdownService>();
builder.Services.AddScoped<IExpediteRequestService, ExpediteRequestService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();




// Apply Authorization Policies


builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options =>
{
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});

// Appseting AD Groups for Authorization
builder.Services.AddAuthorization(options =>
{
    IEnumerable<IConfigurationSection> policies = builder.Configuration.GetSection("AuthorizationPolicyGroups").GetChildren();
    foreach (IConfigurationSection policy in policies)
    {
        if (policy.Value is not null)
        {
            string[] adGroups = policy.Value.Split(",").Select(x => x.Trim()).ToArray();
            options.AddPolicy(policy.Key, x =>
                x.RequireAuthenticatedUser().RequireRole(adGroups)
            );
        }
    }
});

builder.Services.AddDbContextFactory<CommonContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("common");
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseSqlServer(connectionString);
    }

    options.EnableSensitiveDataLogging(false);
}, ServiceLifetime.Transient);


builder.Services.AddDbContextFactory<ExpediteRequestContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("app");
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseSqlServer(connectionString);
    }

    options.EnableSensitiveDataLogging(false);
}, ServiceLifetime.Transient);

builder.Services.AddDbContextFactory<IND_APPContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("syteline");
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseSqlServer(connectionString);
    }

    options.EnableSensitiveDataLogging(false);
}, ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Local"))
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
//app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
