# MigrarDatos.ps1
Write-Host "Iniciando migracion de SQL Server a SQLite..." -ForegroundColor Green

if (-not (Test-Path "eCommerce.sln")) {
    Write-Host "Error: Ejecuta desde la carpeta raiz (donde esta eCommerce.sln)" -ForegroundColor Red
    exit
}

Write-Host "`nCreando proyecto temporal..." -ForegroundColor Yellow
dotnet new console -n DataMigrator -f net8.0
cd DataMigrator

Write-Host "`nInstalando paquetes..." -ForegroundColor Yellow
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.10
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.10
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.10

Write-Host "`nReferenciando proyecto..." -ForegroundColor Yellow
dotnet add reference ..\eCommerceMVC\eCommerceMVC.csproj

Write-Host "`nCreando codigo de migracion..." -ForegroundColor Yellow

$codigo = @"
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;

var sqlServerConnection = "Server=(localdb)\\mssqllocaldb;Database=DBECOMMERCE;Trusted_Connection=True;TrustServerCertificate=True;";
var sqliteConnection = "Data Source=ecommerce.db";

Console.WriteLine("Migrando de SQL Server a SQLite...\n");

var sqlServerOptions = new DbContextOptionsBuilder<DbecommerceContext>()
    .UseSqlServer(sqlServerConnection).Options;

var sqliteOptions = new DbContextOptionsBuilder<DbecommerceContext>()
    .UseSqlite(sqliteConnection).Options;

using var source = new DbecommerceContext(sqlServerOptions);
using var target = new DbecommerceContext(sqliteOptions);

Console.WriteLine("Creando base de datos SQLite...");
await target.Database.EnsureCreatedAsync();

Console.WriteLine("\nMigrando datos...\n");

try
{
    await MigrarTabla("Categorias", source.Categorias, target.Categorias);
    await MigrarTabla("Marcas", source.Marcas, target.Marcas);
    await MigrarTabla("Productos", source.Productos, target.Productos);
    await MigrarTabla("Clientes", source.Clientes, target.Clientes);
    await MigrarTabla("Usuarios", source.Usuarios, target.Usuarios);
    await MigrarTabla("DireccionesEnvio", source.DireccionesEnvio, target.DireccionesEnvio);
    await MigrarTabla("EstadosPedido", source.EstadosPedido, target.EstadosPedido);
    await MigrarTabla("MetodosPago", source.MetodosPago, target.MetodosPago);
    await MigrarTabla("Cupones", source.Cupones, target.Cupones);
    await MigrarTabla("Ofertas", source.Ofertas, target.Ofertas);
    await MigrarTabla("Ventas", source.Ventas, target.Ventas);
    await MigrarTabla("DetalleVentas", source.DetalleVentas, target.DetalleVentas);
    await MigrarTabla("Carritos", source.Carritos, target.Carritos);
    await MigrarTabla("ProductoEspecificaciones", source.ProductoEspecificaciones, target.ProductoEspecificaciones);
    await MigrarTabla("ProductoImagenes", source.ProductoImagenes, target.ProductoImagenes);
    await MigrarTabla("HistorialPedidos", source.HistorialPedidos, target.HistorialPedidos);
    await MigrarTabla("ConfiguracionesPc", source.ConfiguracionesPc, target.ConfiguracionesPc);
    await MigrarTabla("ConfiguracionesPcDetalles", source.ConfiguracionesPcDetalles, target.ConfiguracionesPcDetalles);

    Console.WriteLine("\nMigracion completada!");
    var fileInfo = new FileInfo("ecommerce.db");
    Console.WriteLine("Archivo: {0}", fileInfo.FullName);
    Console.WriteLine("Tamano: {0:N0} KB", fileInfo.Length / 1024);
}
catch (Exception ex)
{
    Console.WriteLine("\nError: {0}", ex.Message);
    Console.WriteLine("Stack: {0}", ex.StackTrace);
}

async Task MigrarTabla<T>(string nombre, DbSet<T> origen, DbSet<T> destino) where T : class
{
    try
    {
        Console.Write("  -> {0,-30}", nombre);
        var datos = await origen.AsNoTracking().ToListAsync();
        if (datos.Any())
        {
            await destino.AddRangeAsync(datos);
            await destino.Context.SaveChangesAsync();
            Console.WriteLine("OK - {0,5} registros", datos.Count);
        }
        else
        {
            Console.WriteLine("(vacia)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR: {0}", ex.Message);
    }
}
"@

$codigo | Out-File -FilePath "Program.cs" -Encoding UTF8

Write-Host "`nEjecutando migracion (esto puede tardar 1-2 minutos)..." -ForegroundColor Green
dotnet run

if (Test-Path "ecommerce.db") {
    Write-Host "`nCopiando archivo al proyecto..." -ForegroundColor Yellow
    Copy-Item "ecommerce.db" "..\eCommerceMVC\" -Force
    
    $fileSize = (Get-Item "ecommerce.db").Length / 1KB
    Write-Host "Archivo copiado a eCommerceMVC\ecommerce.db ($([math]::Round($fileSize, 2)) KB)" -ForegroundColor Green
} else {
    Write-Host "`nNo se encontro el archivo ecommerce.db" -ForegroundColor Red
}

Write-Host "`nLimpiando archivos temporales..." -ForegroundColor Yellow
cd ..
Remove-Item -Recurse -Force DataMigrator

Write-Host "`nProceso completado!" -ForegroundColor Green
Write-Host "Archivo: eCommerceMVC\ecommerce.db" -ForegroundColor Cyan
Write-Host "Listo para deploy a Azure!" -ForegroundColor Cyan
