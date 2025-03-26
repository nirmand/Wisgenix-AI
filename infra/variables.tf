variable "resource_group" {
  description = "The name of the resource group."
  type        = string
}

variable "location" {
  description = "Azure region where resources will be created."
  type        = string
  default     = "East US"
}

variable "app_name" {
  description = "The name of the App Service."
  type        = string
}

variable "app_service_plan" {
  description = "The name of the App Service Plan."
  type        = string
}

variable "sql_server" {
  description = "The name of the Azure SQL Server."
  type        = string
}

variable "sql_database" {
  description = "The name of the Azure SQL Database."
  type        = string
}

variable "admin_password" {
  description = "The administrator password for the SQL Server."
  type        = string
  sensitive   = true
}

variable "kv_name" {
  description = "The name of the Azure Key Vault."
  type        = string
}

variable "db_connection_string" {
  description = "The connection string for the database."
  type        = string
  sensitive   = true
}

variable "app_service_os_type" {
  description = "The operating system for the App Service."
  type        = string
  default     = "Windows"
}

variable "app_service_sku" {
  description = "The SKU for the App Service."
  type        = string
  default     = "S1"
}

variable "tenant_id" {
  description = "The tenant ID for the Azure subscription"
  type        = string
}

variable "subscription_id" {
  description = "The subscription ID for the Azure subscription"
  type        = string
}

variable "service_principle_id" {
  description = "The ID for the Azure service principal"
  type        = string
}

variable "client_id" {
  description = "The client ID for the Azure service principal"
  type        = string
}

variable "client_secret" {
  description = "The client secret for the Azure service principal"
  type        = string
  sensitive   = true
}

variable "db_password" {
  description = "The password for the database"
  type        = string
  sensitive   = true
}

variable "app_service_always_on" {
  description = "Enable Always On for the App Service"
  type        = bool
  default     = true
}