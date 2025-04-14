variable "resource_group" {}
variable "location" {}
variable "kv_name" {}
variable "db_password" {}
variable "tenant_id" {}
variable "service_principle_id" {}
variable "key_permissions" {
  type        = list(string)
  description = "List of key permissions."
  default     = ["List", "Create", "Delete", "Get", "Purge", "Recover", "Update", "GetRotationPolicy", "SetRotationPolicy"]
}
variable "secret_permissions" {
  type        = list(string)
  description = "List of secret permissions."
  default     = ["Set"]
}
variable "msi_id" {}