using NewsFetcherAPI.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<NewsFetcherAPI.Configurators.GNewsSettings>(
    builder.Configuration.GetSection("GNews"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "NewsFetcher API",
        Description = "A simple .NET 8 Web API for fetching and searching news articles",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Irfan Khan",
            Email = "irfank480@gmail.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

// app.UseHttpsRedirection();
app.MapControllers();
app.Run();