using HomeHub.Endpoints;
using HomeHub.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddTransient<IRoomRepository>(provider => new PostgresRoomRepository(connectionString));
builder.Services.AddTransient<ITaskRepository>(provider => new PostgresTaskRepository(connectionString));
builder.Services.AddTransient<INoteRepository>(provider => new PostgresNoteRepository(connectionString));

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();
    var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
    var noteRepository = scope.ServiceProvider.GetRequiredService<INoteRepository>();
    roomRepository.InitializeDatabase();
    taskRepository.InitializeDatabase();
    noteRepository.InitializeDatabase();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.MapRoomEndpoints();
app.MapTaskEndpoints();
app.MapNoteEndpoints();

app.Run();

public partial class Program { }