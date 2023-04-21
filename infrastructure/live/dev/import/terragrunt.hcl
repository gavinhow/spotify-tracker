include "root" {
  path = find_in_parent_folders()
}

locals {
  # Automatically load environment-level variables
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
}

terraform {
  source = "${get_parent_terragrunt_dir()}//modules/import"
}

dependency "db" {
  config_path  = "../db"
  mock_outputs = {
    database = {
      host     = "spotify_mock"
      database = "spotify_mock"
      username = "spotify_mock"
      password = "spotify_mock"
    }
  }
  mock_outputs_allowed_terraform_commands = ["init","validate"]
}

inputs = {
  database = {
    host     = dependency.db.outputs.database.host
    name     = dependency.db.outputs.database.database
    username = dependency.db.outputs.database.username
    password = dependency.db.outputs.database.password
  }
}
