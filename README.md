## Prerequisites
* [.Net Core SDK 3.1.100](https://download.visualstudio.microsoft.com/download/pr/639f7cfa-84f8-48e8-b6c9-82634314e28f/8eb04e1b5f34df0c840c1bffa363c101/dotnet-sdk-3.1.100-win-x64.exe)
* [SQL Server Express](https://go.microsoft.com/fwlink/?linkid=853017) LocalDB

Get Entity Framework tools if not installed
```bash
dotnet tool install --global dotnet-ef
```

## Prepare project
Create database
```bash
dotnet ef database update -p WmKazTest.Data -s WmKazTest
```
Connection string in `/WmKazTest/appsettings.json`

## Run project
```bash
dotnet run -p WmKazTest
```
If there is no errors in console, project ready to accept observations.

Use `Ctrl + C` for shutdown.

## Testing
Run all tests
```bash
dotnet test WmKazTest.Tests
```
