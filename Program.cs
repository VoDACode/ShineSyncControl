//ShineSyncControl.Tests.ExpressionTest expressionTest = new ShineSyncControl.Tests.ExpressionTest();
//expressionTest.Run();


using Microsoft.AspNetCore.Authentication.Cookies;
using ShineSyncControl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration["ConnectionStrings:WebApiDatabase"];
if(connectionString == null)
{
    throw new Exception("Connection string is null");
}

builder.Services.AddSqlServer<DbApp>(connectionString);

builder.Services.AddSwaggerGen(p =>
{
    p.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "",
        Version = "v1"
    });
});
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
