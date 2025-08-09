**Steps to enable access of App Service on SQL database**
1. Create Virtual Network in Azure Resource Group
2. Create two subnets within the Virtual Network
2.1 Subnet-1 for outbound connections from Azure Web App
2.2 Subnet-2 for Private Endpoint

```
CREATE USER [Wisgenix-ContentService] FROM EXTERNAL PROVIDER;
-- Add the user to the db_datareader role to give them read permissions. 
ALTER ROLE db_datareader ADD MEMBER [Wisgenix-ContentService]; 
GO

-- Add the user to the db_datawriter role to give them write permissions. 
ALTER ROLE db_datawriter ADD MEMBER [Wisgenix-ContentService]; 
GO
```