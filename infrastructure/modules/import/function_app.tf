provider "azurerm" {
  alias           = "import"
  # Configuration options
  features {}
  subscription_id = var.subscription_id
}


###
### Function App
###
resource "azurerm_storage_account" "import_function_app_storage" {
  name                     = "spotifunc${var.environment}"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  provider                 = azurerm.import
}

resource "azurerm_service_plan" "import_function_app_service_plan" {
  name                = "spoti-asp-func-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
  provider            = azurerm.import
}

resource "azurerm_windows_function_app" "import_function_app" {
  name                = "spoti-func-import-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location

  storage_account_name       = azurerm_storage_account.import_function_app_storage.name
  storage_account_access_key = azurerm_storage_account.import_function_app_storage.primary_access_key
  service_plan_id            = azurerm_service_plan.import_function_app_service_plan.id

  provider = azurerm.import

  site_config {}
}

output "import_function_app_name" {
  value = azurerm_windows_function_app.import_function_app.name
}

output "import_function_app_resource_group_name" {
  value = azurerm_windows_function_app.import_function_app.resource_group_name
}