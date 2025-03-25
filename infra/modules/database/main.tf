resource "azurerm_mssql_server" "sql" {
  name                         = var.sql_server
  resource_group_name          = var.resource_group
  location                     = var.location
  version                      = "12.0"
  administrator_login          = "adminuser"
  administrator_login_password = var.admin_password
}

resource "azurerm_mssql_database" "db" {
  name           = var.sql_database
  server_id      = azurerm_mssql_server.sql.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 1
  sku_name       = "S0"
}
