using CategoriesProducts.Api.Data;
using CategoriesProducts.Specs.Support;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.BoDi;
using Testcontainers.MsSql;

namespace CategoriesProducts.Specs.Hooks;

[Binding]
public sealed class ApiHooks
{
    private static MsSqlContainer _sqlServer = null!;
    private static ApiFactory _factory = null!;

    // Un solo SQL Server y una sola API para toda la corrida: arrancar el contenedor tarda varios segundos.
    [BeforeTestRun]
    public static async Task StartApi()
    {
        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();

        var connectionString = new SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "CategoriesProductsTests"
        }.ConnectionString;

        _factory = new ApiFactory(connectionString);
    }

    [AfterTestRun]
    public static async Task StopApi()
    {
        await _factory.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }

    // Cada escenario empieza con la base de datos vacía y su propio cliente HTTP.
    [BeforeScenario]
    public async Task PrepareScenario(IObjectContainer scenarioContainer)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Products.ExecuteDeleteAsync();
        await db.Categories.ExecuteDeleteAsync();

        scenarioContainer.RegisterInstanceAs(_factory.CreateClient());
    }
}
