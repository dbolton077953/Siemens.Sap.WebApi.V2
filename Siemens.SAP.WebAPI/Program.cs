using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Siemens.AzureAd.Common;
using Siemens.Sap.ERPConnect.Utilities;
using Siemens.Sap.WebAPI;
using System.Configuration;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Azure AD configuration
builder.Services.AddAzureToApi(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

// Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UnfinishedSerialVisitor", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
    });
});
builder.Services.AddAuthorization(options =>
{
    

    //policy is to to give roles to access
    options.AddPolicy(Policies.SapApiAdminRole,
        Policies.AdminViewPolicy());

});
// Configure cookies to support SameSite policy and ensure OAuth cookies are allowed during the OAuth flow
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

// Swagger configuration with Azure OAuth integration
builder.Services.AddSwaggerWithOAuth(
    builder.Configuration,
    apiName: "Siemens.Sap.WebApi",
    apiVersion: "v1",
    apiDescription: "API documentation Siemens Sap Web API",
    contact: new OpenApiContact
    {
        Email = "Dominic.Bolton@siemens.com",
        Name = "Dominic Bolton"
    },
    authUrl: $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize",
    clientId: builder.Configuration["AzureAd:ClientId"],
    scopeName: builder.Configuration["AzureAd:Scopes"],
    scopeDescription: "Access API"
);
IdentityModelEventSource.ShowPII = true;
IdentityModelEventSource.LogCompleteSecurityArtifact = true;
AppSettings appSettings = new AppSettings();
builder.Configuration.GetSection("AppSettings").Bind(appSettings);

App myApp = new App(builder.Configuration);


//ERPConnect.LIC.SetLic("Y8ZVVZZZYY");
//ERPConnect.LIC.SetLic("YZZZYZ4ZYV"); Z9YYYYVYYY

myApp.WriteToTextFile("Set ERPConnect Licence");
ERPConnect.LIC.SetLic(appSettings.LicenceKey);
myApp.WriteToTextFile("Licence set");


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Swagger middleware for OAuth2 and Swagger UI after app is built
app.UseSwaggerWithOAuth(
    apiName: "Congleton Dashboard",
    apiVersion: "v1",
    swaggerEndpoint: app.Environment.IsDevelopment() ? "/swagger/v1/swagger.json" : builder.Configuration["AppSettings:SwaggerProductionUri"],
    clientId: builder.Configuration["AzureAd:ClientId"],
    redirectUrl: builder.Configuration["AzureAd:AzureRedirectUri"],
    env: app.Environment,
    routePrefix: builder.Configuration["AppSettings:SwaggerRoutePrefix"]
);

app.MapControllers();

app.Run();
