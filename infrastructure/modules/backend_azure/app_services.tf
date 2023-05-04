provider "azurerm" {
  alias           = "import"
  # Configuration options
  features {}
  subscription_id = var.subscription_id
}

resource "azurerm_service_plan" "backend_plan" {
  location            = var.location
  name                = "spoti-backend-plan-${var.environment}"
  resource_group_name = var.resource_group_name
  sku_name            = "B1"
  os_type             = "Linux"
  provider            = azurerm.import
}

resource "azurerm_linux_web_app" "backend" {
  service_plan_id     = azurerm_service_plan.backend_plan.id
  location            = var.location
  name                = "spoti-backend-${var.environment}"
  resource_group_name = var.resource_group_name
  provider            = azurerm.import

  site_config {}
}

output "backend" {
  value     = azurerm_linux_web_app.backend
  sensitive = true
}