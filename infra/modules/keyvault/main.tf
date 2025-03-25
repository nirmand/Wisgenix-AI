resource "azurerm_key_vault" "kv" {
  name                        = var.kv_name
  location                    = var.location
  resource_group_name         = var.resource_group
  tenant_id                   = var.tenant_id
  sku_name                    = "standard"
}

resource "azurerm_key_vault_secret" "db_password" {
  name         = "DatabasePassword"
  value        = var.db_password
  key_vault_id = azurerm_key_vault.kv.id
}