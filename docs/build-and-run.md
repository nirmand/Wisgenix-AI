# Building and Running API and Web App

## Creating database
- Open appSettings.[Environment].json
- Set DatabasetType (1 = AzureSQL, 2 = SqLite)
- Set ConnectionString to AzureSQL database or Sqlite file
- Open Terminal in VS Code
- Navigate to root **src** folder
- Execute relevant command

**To create a new database from scratch**
```
dotnet ef migrations add InitialCreate --project Wisgenix.Data --startup-project Wisgenix.API
dotnet ef database update --project Wisgenix.Data --startup-project Wisgenix.API
```

**To update existing database**
```
dotnet ef migrations add Migration-2103251900 --project Wisgenix.Data --startup-project Wisgenix.API
dotnet ef database update --project Wisgenix.Data --startup-project Wisgenix.API
```

## Hosting app on local server
### Wisgenix.Api (Kestrel)
If you don't have a dedicated hosting subscription on Azure, and if you are looking to host and run app on local server. This is considered as a seperate environment as per **appSettings.localhost.json**.
- Create a new folder for hosting (example - E:\Wisgenix\localhost)
- Open PowerShell
- Navigate to **.\src\Wisgenix.API** folder of source code
- Publish website in Release mode to newly created localhost folder
```
dotnet publish --configuration Release --output "E:\Wisgenix\localhost\Wisgenix.API"
```
- Navigate to E:\Wisgenix\localhost\Webgenix.API
- Set environment to localhost
```
 $env:ASPNETCORE_ENVIRONMENT = "localhost"
```
- Install and trust certificate
```
dotnet dev-certs https --trust
```
- Run API project
```
dotnet Wisgenix.API.dll
```
- Browse to HTTPS access URL configured in **appSettings.localhost.json** (example - https://localhost:5081)

### Wisgenix.Web (React APP)
- [TBC]
