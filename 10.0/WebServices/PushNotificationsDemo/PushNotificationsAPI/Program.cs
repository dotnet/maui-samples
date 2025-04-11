using PushNotificationsAPI.Authentication;
using PushNotificationsAPI.Services;
using PushNotificationsAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthOptions.DefaultScheme;
    options.DefaultChallengeScheme = ApiKeyAuthOptions.DefaultScheme;
}).AddApiKeyAuth(builder.Configuration.GetSection("Authentication").Bind);

builder.Services.AddSingleton<INotificationService, NotificationHubService>();
builder.Services.AddOptions<NotificationHubOptions>()
    .Configure(builder.Configuration.GetSection("NotificationHub").Bind)
    .ValidateDataAnnotations();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
