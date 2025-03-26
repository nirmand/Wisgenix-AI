provider "azurerm" {
  features {}

  subscription_id = var.subscription_id
  client_id       = var.client_id
  client_secret   = var.client_secret
  tenant_id       = var.tenant_id
}

module "appservice" {
  source               = "./modules/appservice"
  resource_group       = var.resource_group
  location             = var.location
  app_name             = var.app_name
  app_service_plan     = var.app_service_plan
  db_connection_string = var.db_connection_string
  app_service_os_type  = var.app_service_os_type
  app_service_sku      = var.app_service_sku
  app_service_always_on = var.app_service_always_on
}

module "database" {
  source         = "./modules/database"
  resource_group = var.resource_group
  location       = var.location
  sql_server     = var.sql_server
  sql_database   = var.sql_database
  admin_password = var.admin_password
}

module "keyvault" {
  source               = "./modules/keyvault"
  resource_group       = var.resource_group
  location             = var.location
  kv_name              = var.kv_name
  db_password          = var.admin_password
  tenant_id            = var.tenant_id
  service_principle_id = var.service_principle_id
}