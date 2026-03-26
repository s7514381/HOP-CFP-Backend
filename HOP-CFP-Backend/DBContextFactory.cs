using HOP_CFP_Backend.Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class DBContextFactory : IDesignTimeDbContextFactory<DBContext>
{
    public DBContext CreateDbContext(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

#if UAT
        environment = "UAT";
#elif RELEASE
        environment = "Release";
#endif

        Console.WriteLine($"DesignTimeDbContextFactory 正在使用的環境: {environment}");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<DBContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"connectionString: {connectionString}");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"找不到環境 '{environment}' 的連線字串 'DefaultConnection'。");
        }

        builder.UseSqlServer(connectionString, b =>
            b.MigrationsAssembly("HOP-CFP-Backend")
        );

        return new DBContext(builder.Options);
    }
}
