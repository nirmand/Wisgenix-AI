output "msi_id" {
  value = azurerm_key_vault.kv.identity.principal_id
  description = "The Managed Service Identity (MSI) ID of the Key Vault"
}