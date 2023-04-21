provider "postgresql" {
  host            = var.postgres_connection.host
  port            = var.postgres_connection.port
  database        = var.postgres_connection.database
  username        = var.postgres_connection.username
  password        = var.postgres_connection.password
  sslmode         = var.postgres_connection.sslmode
  connect_timeout = 15
}

resource "postgresql_database" "spotify" {
  name = var.database_name
}

resource "random_password" "password" {
  length           = 16
  special          = false
}

resource "postgresql_role" "spotify_role" {
  name     = "spotify_${var.environment}"
  login    = true
  password = random_password.password.result
}

resource "postgresql_grant" "spotify_role" {
  database    = postgresql_database.spotify.name
  role        = postgresql_role.spotify_role.name
  object_type = "database"
  privileges  = [
      "CREATE", "CONNECT", "TEMPORARY",
  ]
}

output "database" {
  value = {
    host     = var.postgres_connection.host
    port     = var.postgres_connection.port
    database = postgresql_database.spotify.name
    username = postgresql_role.spotify_role.name
    password = random_password.password.result
  }
  sensitive = true
}