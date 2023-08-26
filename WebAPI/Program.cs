using Application;
using Persistence;
using Persistence.Context;
const string corsPolicy = "Cors";

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigurePersistence(builder.Configuration);
builder.Services.ConfigureApplication();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

var serviceScope = app.Services.CreateScope();
var dataContext = serviceScope.ServiceProvider.GetService<ApplicationContext>();
dataContext?.Database.EnsureCreated();
app.UseIdentityServer();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => 
{
    endpoints.MapControllers();
});
app.Run();