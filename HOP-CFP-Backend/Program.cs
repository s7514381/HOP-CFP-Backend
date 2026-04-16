using HOP_CFP_Backend.Filter;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Utility;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IServiceCollection services = builder.Services;
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

services.AddHttpContextAccessor();

services.AddMemoryCache();

builder.Services.AddDistributedMemoryCache();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "HOP_CFP_Backend";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.MaxAge = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 如果有帶 Cookie 或 Authorization
    });
});


ConfigureSingletonService(services, configuration);
ConfigureScopedService(services, configuration);

var app = builder.Build();

app.Use(async (context, next) =>
{
    // context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// 自訂上傳路徑的 StaticFiles
var uploadPath = builder.Configuration.GetValue<string>("UploadFilePath");
if (!string.IsNullOrEmpty(uploadPath))
{
    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
    // 建議加上檢查目錄是否存在，避免報錯
    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(fullPath),
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseCors("DevPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureSingletonService(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IMailSender, MailSender>();
    //services.AddSingleton<LineNotifySender>();
    services.AddSingleton<MailSenderConfig>();
    //services.AddSingleton<FileExtensionContentTypeProvider>();
    //services.AddSingleton<BackgroundProcessService>();
    //services.AddSingleton<AzureStoreService>();
    //services.AddSingleton<AzureTableTarget>();
    //services.AddSingleton<GoogleSheetsService>();
}

void ConfigureScopedService(IServiceCollection services, IConfiguration configuration)
{
    var sqlConnString = configuration.GetConnectionString("DefaultConnection");

    services.AddScoped<IDapperRepository>(x =>
    {
        var dapperDBContext = new DapperDBContext(sqlConnString) { SQLType = SQLType.SQLSERVER };
        return new DapperRepository(dapperDBContext);
    });

    services.AddSingleton<IDbConnectionFactory>(x =>
    {
        SqlConnectionFactoryConfig config = new SqlConnectionFactoryConfig
        {
            ConnectionString = sqlConnString,
        };
        return new SqlConnectionFactory(config);
    });

    // 自動掃描並註冊 Services
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var serviceList = assemblies.SelectMany(a => a.GetExportedTypes())
        .Where(x =>
            x.Namespace != null &&
            (x.Namespace.StartsWith("HOP_CFP_Backend.Services") ||
             x.Namespace.StartsWith("HOP_CFP_Backend.Argument")) &&
            x.IsClass && x.IsPublic && !x.IsAbstract);

    foreach (Type type in serviceList) { services.AddScoped(type); }

    services.AddScoped<AuthorizeFilter>();
    services.AddScoped<ApiFilter>();

    services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));
}

internal class Lazier<T> : Lazy<T> where T : class
{
    public Lazier(IServiceProvider provider) : base(provider.GetRequiredService<T>) { }
}