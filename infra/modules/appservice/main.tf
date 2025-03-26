resource "azurerm_service_plan" "app_plan" {
  name                = var.app_service_plan
  location            = var.location
  resource_group_name = var.resource_group
  os_type             = var.app_service_os_type
  sku_name            = var.app_service_sku  
}

resource "azurerm_linux_web_app" "app" {
  name                = var.app_name
  location            = var.location
  resource_group_name = var.resource_group
  service_plan_id = azurerm_service_plan.app_plan.id

  site_config {
    always_on = var.app_service_always_on
  }

  app_settings = {
    "DATABASE_CONNECTION_STRING" = var.db_connection_string
  }
}
