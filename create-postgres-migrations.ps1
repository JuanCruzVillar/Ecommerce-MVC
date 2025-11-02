# Script para crear migraciones compatibles con PostgreSQL
# Ejecutar desde la raíz del proyecto (donde está la carpeta eCommerceMVC)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Creando Migraciones para PostgreSQL" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en la carpeta correcta
if (-Not (Test-Path "eCommerceMVC")) {
    Write-Host "❌ Error: No se encuentra la carpeta eCommerceMVC" -ForegroundColor Red
    Write-Host "   Ejecuta este script desde la raíz del proyecto" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "✅ Carpeta del proyecto encontrada" -ForegroundColor Green
Write-Host ""

# Navegar a la carpeta del proyecto
cd eCommerceMVC

Write-Host "🗑️ Eliminando migraciones anteriores de SQL Server..." -ForegroundColor Yellow
if (Test-Path "Migrations") {
    Remove-Item -Recurse -Force "Migrations"
    Write-Host "✅ Migraciones anteriores eliminadas" -ForegroundColor Green
} else {
    Write-Host "ℹ️ No hay migraciones anteriores" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📝 Creando migración inicial para PostgreSQL..." -ForegroundColor Yellow
Write-Host ""

# Temporal: configurar para usar PostgreSQL
$env:UsePostgreSQL = "true"
$env:DATABASE_URL = "postgresql://temp:temp@localhost/temp"

# Crear migración
dotnet ef migrations add InitialCreate

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "✅ ¡Migraciones creadas exitosamente!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 Próximos pasos:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Revisa que se creó la carpeta 'Migrations' con los archivos" -ForegroundColor White
    Write-Host ""
    Write-Host "2. Haz commit de los cambios:" -ForegroundColor White
    Write-Host "   git add ." -ForegroundColor Gray
    Write-Host "   git commit -m 'feat: add PostgreSQL support for Azure deployment'" -ForegroundColor Gray
    Write-Host "   git push origin main" -ForegroundColor Gray
    Write-Host ""
    Write-Host "3. Continúa con la configuración de Supabase y Azure" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "❌ Error al crear las migraciones" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Posibles causas:" -ForegroundColor Yellow
    Write-Host "1. No tienes instalado dotnet-ef" -ForegroundColor Gray
    Write-Host "   Solución: dotnet tool install --global dotnet-ef" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. Hay errores en el DbContext" -ForegroundColor Gray
    Write-Host "   Revisa los errores arriba" -ForegroundColor Gray
    Write-Host ""
}

# Volver a la raíz
cd ..

Write-Host ""
Read-Host "Presiona Enter para salir"