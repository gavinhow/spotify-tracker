include "root" {
  path = find_in_parent_folders("root.hcl")
}

terraform {
  source = "${get_parent_terragrunt_dir()}//modules/db"
}

inputs = {
  database_name = "spotify_dev"
}
