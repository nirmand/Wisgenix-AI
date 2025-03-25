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