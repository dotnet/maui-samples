// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AspNetCore.Datasync;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
//using TodoAppService.NET6.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApi(builder.Configuration);
builder.Services.AddAuthorization();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//if (connectionString == null)
//{
//    throw new ApplicationException("DefaultConnection is not set");
//}

//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
//builder.Services.AddDatasyncControllers();

var app = builder.Build();

// Initialize the database
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await context.InitializeDatabaseAsync().ConfigureAwait(false);
//}

// Configure and run the web service.
app.UseAuthentication();
app.UseAuthorization();
//app.MapControllers();
app.Run();