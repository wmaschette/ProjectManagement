using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Application.Interfaces;
using ProjectManagementAPI.Application.Services;
using ProjectManagementAPI.Infra.Data;
using ProjectManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
builder.Services.AddScoped<IPerformanceReportService, PerformanceReportService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
