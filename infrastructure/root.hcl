locals {
  # Automatically load environment-level variables
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))

  env                             = local.env_vars.locals.environment
  subscription_id                 = local.env_vars.locals.subscription_id
  resource_group_name             = local.env_vars.locals.resource_group_name
  deployment_storage_account_name = local.env_vars.locals.deployment_storage_account_name
}

# Generate Azure providers
generate "providers" {
  path      = "providers_override.tf"
  if_exists = "overwrite_terragrunt"
  contents  = <<EOF
    terraform {
      required_providers {
        azurerm = {
          source = "hashicorp/azurerm"
          version = "4.14.0"
        }
      }
    }

    provider "azurerm" {
        features {}
        subscription_id = "${local.subscription_id}"
    }
EOF
}

remote_state {
  backend = "azurerm"
  config  = {
    subscription_id      = local.subscription_id
    resource_group_name  = local.resource_group_name
    storage_account_name = local.deployment_storage_account_name
    key                  = "${path_relative_to_include()}/terraform.tfstate"
    container_name       = "terraform-state"
  }
  generate = {
    path      = "backend.tf"
    if_exists = "overwrite_terragrunt"
  }
}

inputs = merge(
  local.env_vars.locals,
)