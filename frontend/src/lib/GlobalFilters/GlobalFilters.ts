
export enum RANGE_OPTION {
  Last_7_days = "Last 7 days",
  Last_30_days = "Last 30 days",
  Last_365_days = "Last 365 days",
  All_time = "All time",
  Custom = "Custom",
}

export interface GlobalFiltersSearchParams {
  user?: string;
  range?: RANGE_OPTION;
  from?: string;
  to?: string;
}

export interface GlobalFilters {
  user: string;
  range: RANGE_OPTION;
  from: Date;
  to: Date;
}
