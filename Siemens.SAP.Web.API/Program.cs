using Siemens.Sap.WebAPI;
using System.Configuration;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
