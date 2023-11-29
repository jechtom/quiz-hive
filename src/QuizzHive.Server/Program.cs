using Microsoft.AspNetCore.DataProtection;
using QuizzHive.Server;
using QuizzHive.Server.DataLayer;
using QuizzHive.Server.Hubs;
using QuizzHive.Server.Services;
using QuizzHive.Server.State;

var builder = WebApplication
    .CreateBuilder(new WebApplicationOptions()
                   {
                       Args = args,
                       WebRootPath = "../QuizzHive.Client/out"
                   });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddSingleton<SessionCodeGenerator>();
builder.Services.AddSingleton<ClientsManager>();
builder.Services.AddScoped<IRepository<Session>, FileRepository<Session>>();
builder.Services.AddScoped<IRepository<ActiveSessions>, FileRepository<ActiveSessions>>();
builder.Services.AddScoped<SessionActionDispatcher>();
builder.Services.AddScoped<SessionsManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IRepository<Session>>();
    var repo2 = scope.ServiceProvider.GetRequiredService<IRepository<ActiveSessions>>();
    var testDataGenerator = new TestDataGenerator(repo, repo2);
    await testDataGenerator.GenerateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials()); // allow credentials
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = ["index.html"]
});

app.UseStaticFiles();

app.MapControllers();

app.MapHub<AppHub>("/hub");

app.Run();
