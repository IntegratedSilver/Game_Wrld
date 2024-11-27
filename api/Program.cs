using api.Services;
using api.Services.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogItemService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<FriendRequestService>();
builder.Services.AddScoped<FriendService>();

var connectionString = builder.Configuration.GetConnectionString("GameWrldString");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();


app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethod.Options.ToString())
    {
        context.Response.StatusCode = 200;
        return;
    }
    await next();
});

app.Run();
