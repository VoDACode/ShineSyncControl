using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ShineSyncControl;
using ShineSyncControl.Models.ConfigOptions;
using ShineSyncControl.Services.DataBus;
using ShineSyncControl.Services.DeviceCommand;
using ShineSyncControl.Services.DeviceManager;
using ShineSyncControl.Services.Email;
using ShineSyncControl.Services.TaskEventWorker;
using System.Text;
using VoDA.WebSockets;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("https://0.0.0.0:7070");
}

builder.Services.Configure<DeviceOption>(
    builder.Configuration.GetSection("Device"));

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration["ConnectionStrings:WebApiDatabase"];
if (connectionString == null)
{
    throw new Exception("Connection string is null");
}

builder.Services.AddSqlServer<DbApp>(connectionString);

builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = builder.Configuration["Services:Redis"];
});


builder.Services.AddSwaggerGen(p =>
{
    p.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "",
        Version = "v1"
    });
});
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(o =>
    {
        o.LoginPath = "/login";
    })
.AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateIssuerSigningKey = true,
           ValidateLifetime = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
       };
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

// add CROS. Access-Control-Allow-Origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
               builder =>
               {
                   builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
               });
});

builder.Services.AddTaskEventWorker();

builder.Services.AddDeviceManager();

builder.Services.AddDataBus();

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

app.UseCors("AllowAll");

app.UseVoDAWebSocket();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
