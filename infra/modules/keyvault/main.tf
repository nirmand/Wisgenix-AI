data "azurerm_client_config" "current" {}

locals {
  current_user_id = coalesce(var.msi_id, data.azurerm_client_config.current.object_id)
}

resource "azurerm_key_vault" "kv" {
  name                        = var.kv_name
  location                    = var.location
  resource_group_name         = var.resource_group
  tenant_id                   = var.tenant_id
  sku_name                    = "standard"

  access_policy {
    tenant_id = var.tenant_id
    object_id = var.service_principle_id

    key_permissions    = var.key_permissions
    secret_permissions = var.secret_permissions
  }  
}

resource "azurerm_key_vault_secret" "db_password" {
  name         = "DatabasePassword"
  value        = var.db_password
  key_vault_id = azurerm_key_vault.kv.id
}

resource "azurerm_role_assignment" "key_vault_admin" {
  scope                = azurerm_key_vault.kv.id
  role_definition_name = "Key Vault Administrator"
  principal_id         = var.service_principle_id
}