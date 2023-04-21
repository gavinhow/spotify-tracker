terraform {
  required_providers {
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "1.19.0"
    }

    random = {
      source = "hashicorp/random"
      version = "3.5.1"
    }
  }
}