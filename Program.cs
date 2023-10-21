//ShineSyncControl.Tests.ExpressionTest expressionTest = new ShineSyncControl.Tests.ExpressionTest();
//expressionTest.Run();


using Microsoft.AspNetCore.Authentication.Cookies;
using ShineSyncControl;
using ShineSyncControl.Services.Email;

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

builder.Services.AddEmailService(e =>
{
    e.Email = builder.Configuration["Services:Email:EmailAddress"];
    e.SenderName = builder.Configuration["Services:Email:SenderName"];
    e.Password = builder.Configuration["Services:Email:Password"];
    e.Host = builder.Configuration["Services:Email:Host"];
    e.Port = int.Parse(builder.Configuration["Services:Email:Port"]);
    e.EmailTemplatesFoulder = builder.Configuration["Services:Email:EmailTemplatesFoulder"];
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

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(p =>
{
    p.SwaggerEndpoint("/swagger/v1/swagger.json", "");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
