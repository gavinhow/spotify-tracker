variable postgres_connection {
  type = object({
    host     = string,
    port     = number,
    database = string,
    username = string,
    password = string,
    sslmode  = string,
  })
  sensitive = true
  description = "postgres database connection details"
}

variable database_name {
  type = string
  default = "spotify"
  description = "postgres database connection details"
}

variable "environment" {
  type        = string
  description = "The environment in which the function app is being created in."
}