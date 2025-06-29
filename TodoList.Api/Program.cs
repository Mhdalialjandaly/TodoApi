using DataAccess;
using Microsoft.AspNetCore.Diagnostics;
using TodoList.Api.DependeincyInjuction;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.InfrastructureServices(builder.Configuration);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await SystemDbInitializer.Initialize(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/html";

        await context.Response.WriteAsync("<html><body>\r\n");
        await context.Response.WriteAsync("Error!<br>\r\n");

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error != null)
        {
            await context.Response.WriteAsync(
                $"Error: {exceptionHandlerPathFeature.Error.Message}<br>\r\n");
            await context.Response.WriteAsync(
                $"Stack Trace: {exceptionHandlerPathFeature.Error.StackTrace}<br>\r\n");
        }

        await context.Response.WriteAsync("</body></html>\r\n");
    });
});
app.MapControllers();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
