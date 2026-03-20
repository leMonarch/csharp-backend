# Script a executer dans PowerShell : clic droit -> Executer avec PowerShell
# ou dans le terminal : .\restore-and-migrate.ps1

Set-Location $PSScriptRoot

Write-Host "1. Restauration des paquets NuGet..." -ForegroundColor Cyan
dotnet restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "2. Creation de la migration InitialCreate (si pas deja faite)..." -ForegroundColor Cyan
dotnet ef migrations add InitialCreate 2>$null
if ($LASTEXITCODE -ne 0) {
    # La migration existe peut-etre deja
    Write-Host "   (Migration peut-etre deja creee, on continue)" -ForegroundColor Yellow
}

Write-Host "3. Mise a jour de la base MySQL..." -ForegroundColor Cyan
dotnet ef database update
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Termine." -ForegroundColor Green
