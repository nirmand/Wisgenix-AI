terraform {
  backend "azurerm" {
    resource_group_name  = "terraform-state-rg"
    storage_account_name = "aiupskillingterraform"
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }
}