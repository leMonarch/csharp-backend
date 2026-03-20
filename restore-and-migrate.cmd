@echo off
cd /d "%~dp0"

echo 1. Restauration des paquets NuGet...
dotnet restore
if errorlevel 1 exit /b 1

echo 2. Migration InitialCreate (ignorer si deja creee)...
dotnet ef migrations add InitialCreate 2>nul

echo 3. Mise a jour de la base MySQL...
dotnet ef database update
if errorlevel 1 exit /b 1

echo Termine.
pause
