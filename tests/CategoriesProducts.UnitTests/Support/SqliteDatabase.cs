using CategoriesProducts.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CategoriesProducts.UnitTests.Support;

/// <summary>
/// Base de datos SQLite en memoria: es relacional (respeta índices únicos y llaves foráneas)
/// y no necesita Docker, así que las pruebas unitarias corren rápido en cualquier máquina.
/// </summary>
public sealed class SqliteDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public SqliteDatabase()
    {
        _connection.Open();
        using var db = CreateContext();
        db.Database.EnsureCreated();
    }

    public AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);

    public void Dispose() => _connection.Dispose();
}
