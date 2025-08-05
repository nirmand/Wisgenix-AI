Pre-requisites
1. Azure Subscription
2. Azure App Service Plan and Azure Web App
3. Azure SQL Server and Azure SQL Database
4. GitHub repository and action

Step-1: Create service principle (i.e., and App Registration in Entra ID) to allow GitHub to connect Azure


--CREATE USER [nirman.doshi456@hotmail.com] FROM EXTERNAL PROVIDER;

-- Add the user to the db_datareader role to give them read permissions.
ALTER ROLE db_datareader ADD MEMBER [nirman.doshi456@hotmail.com];
GO

-- Add the user to the db_datawriter role to give them write permissions.
ALTER ROLE db_datawriter ADD MEMBER [nirman.doshi456@hotmail.com];
GO