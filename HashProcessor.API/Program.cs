using HashProcessor.API;
using HashProcessor.API.Converters;
using HashProcessor.Application.Commands.SaveHashes;
using HashProcessor.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRedis(configuration);
builder.Services.AddRabbitMQ(configuration);
builder.Services.AddSingleton<IHashGenerator, HashGenerator>();
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<SaveHashesCommand>());

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
