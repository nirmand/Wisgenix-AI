Important references:
Standard .gitignore template for Visual Studio/ VS Code projects - https://raw.githubusercontent.com/github/gitignore/main/VisualStudio.gitignore

Notes:
Entity Framework migration commands:
```
dotnet ef migrations add InitialCreate --project AIUpskillingPlatform.Data --startup-project AIUpskillingPlatform.API
dotnet ef database update --project AIUpskillingPlatform.Data --startup-project AIUpskillingPlatform.API
dotnet ef migrations add Migration-2103251900 --project AIUpskillingPlatform.Data --startup-project AIUpskillingPlatform.API
```
