variable "subscription_id" {
  type        = string
  description = "The ID of the subscription in which to create the function app."
}

variable "environment" {
  type        = string
  description = "The environment in which the function app is being created in."
}

variable "resource_group_name" {
  type        = string
  description = "The name of the resource group in which to create the function app."
}

variable location {
  type        = string
  description = "The location/region where the function app should be created."
}