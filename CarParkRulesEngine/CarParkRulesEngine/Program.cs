using CarParkRulesEngine.Services;
using CarParkRulesEngine.Services.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Rule engine + handlers
builder.Services.AddScoped<IRuleHandler, FreeParkingHandler>();
builder.Services.AddScoped<IRuleHandler, NoReturnHandler>();
builder.Services.AddScoped<IRuleHandler, ValidationHandler>();
builder.Services.AddScoped<IRuleHandler, PaidParkingHandler>();
builder.Services.AddScoped<IRuleHandler, GracePeriodHandler>();
builder.Services.AddScoped<IRuleHandler, EnforcementHandler>();

builder.Services.AddScoped<RuleEngine>();

// SQL rule repository
builder.Services.AddScoped<IRuleRepository, SqlRuleRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Evaluation}/{action=Index}/{id?}");

app.Run();
