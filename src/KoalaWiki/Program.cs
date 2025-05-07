using KoalaWiki.Core.DataAccess;
using KoalaWiki.Git;
using KoalaWiki.KoalaWarehouse;
using KoalaWiki.Options;
using KoalaWiki.Provider.PostgreSQL;
using KoalaWiki.Provider.Sqlite;
using Mapster;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

#region Options

OpenAIOptions.ChatModel = builder.Configuration.GetValue<string>("ChatModel");
OpenAIOptions.ChatApiKey = builder.Configuration.GetValue<string>("ChatApiKey");
OpenAIOptions.Endpoint = builder.Configuration.GetValue<string>("Endpoint");

OpenAIOptions.AnalysisModel = builder.Configuration.GetValue<string>("AnalysisModel");
// 如果没设置分析模型则使用默认的
if (string.IsNullOrEmpty(OpenAIOptions.AnalysisModel))
{
    OpenAIOptions.AnalysisModel = OpenAIOptions.ChatModel;
}

#endregion


builder.Services.AddSerilog(Log.Logger);

builder.Services.AddOpenApi();
builder.Services.WithFast();
builder.Services.AddSingleton<WarehouseStore>();
builder.Services.AddSingleton<GitService>();
builder.Services.AddSingleton<DocumentsService>();

builder.Services
    .AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

builder.Services.AddHostedService<WarehouseTask>();

if (builder.Configuration.GetConnectionString("type")?.Equals("postgres", StringComparison.OrdinalIgnoreCase) == true)
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    builder.Services.AddPostgreSQLDbContext(builder.Configuration);
}
else
{
    builder.Services.AddSqliteDbContext(builder.Configuration);
}

builder.Services.AddMapster();

var app = builder.Build();

// 添加自动迁移代码
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IKoalaWikiContext>();
    await dbContext.RunMigrateAsync();
}

app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapFast();


app.Run();