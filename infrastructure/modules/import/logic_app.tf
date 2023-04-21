
###
### Logic App
###

resource "azurerm_logic_app_workflow" "import_logic_app_workflow" {
  name                = "spoti-logic-import-trigger-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
}

###
### Api Connection
###

data "azurerm_managed_api" "postgres" {
  name     = "postgresql"
  location = var.location
}


resource "azurerm_api_connection" "postgres_connection" {
  name                = "postgres_connection"
  resource_group_name = var.resource_group_name
  managed_api_id      = data.azurerm_managed_api.postgres.id
  display_name        = "Spotitrack DB Connection"

  parameter_values = {
    "encryptConnection" = false,
    "server"            = var.database.host,
    "database"          = var.database.name,
    "authType"          = "basic"
    "username"          = var.database.username
    "password"          = var.database.password
  }


  lifecycle {
    # NOTE: since the connectionString is a secure value it's not returned from the API
    ignore_changes = [parameter_values]
  }
}