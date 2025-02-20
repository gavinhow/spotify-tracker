include "root" {
  path = find_in_parent_folders("root.hcl")
}

locals {
  # Automatically load environment-level variables
  env_vars = read_terragrunt_config(find_in_parent_folders("env.hcl"))
}


terraform {
  source = "${get_parent_terragrunt_dir()}//modules/logic_app"
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
  mock_outputs_allowed_terraform_commands = ["init", "validate"]
}

dependency "import" {
  config_path = "../import"
}

inputs = {
  database = {
    host     = dependency.db.outputs.database.host
    name     = dependency.db.outputs.database.database
    username = dependency.db.outputs.database.username
    password = dependency.db.outputs.database.password
  }
  function_app = dependency.import.outputs.function_app
}
