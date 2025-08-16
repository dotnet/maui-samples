using ChatVoice.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register credential service
builder.Services.AddScoped<ICredentialService, CredentialService>();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors();
}

app.UseHttpsRedirection();

// Credential distribution endpoint
app.MapGet("/api/credentials", (ICredentialService credentialService) =>
{
    try
    {
        return Results.Ok(credentialService.GetCredentials());
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetCredentials");

app.Run();
