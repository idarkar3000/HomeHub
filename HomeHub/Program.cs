using HomeHub.Endpoints;
using HomeHub.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddTransient<IRoomRepository>(provider => new PostgresRoomRepository(connectionString));
builder.Services.AddTransient<ITaskRepository>(provider => new PostgresTaskRepository(connectionString));

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
    var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
    roomRepository.InitializeDatabase();
    taskRepository.InitializeDatabase();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapRoomEndpoints();
app.MapTaskEndpoints();

app.Run();