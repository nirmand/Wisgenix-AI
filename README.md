Index:
<a target="_blank" href=".docs/Arch-Diagram.md">Architecture Diagram</a>

Important references:
Standard .gitignore template for Visual Studio/ VS Code projects - [﻿https://raw.githubusercontent.com/github/gitignore/main/VisualStudio.gitignore](https://raw.githubusercontent.com/github/gitignore/main/VisualStudio.gitignore) 

Notes:
Entity Framework migration commands:

```
dotnet ef migrations add InitialCreate --project Wisgenix.Data --startup-project Wisgenix.API
dotnet ef database update --project Wisgenix.Data --startup-project Wisgenix.API
dotnet ef migrations add Migration-2103251900 --project Wisgenix.Data --startup-project Wisgenix.API
```
