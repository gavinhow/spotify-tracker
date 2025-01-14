import { getUserCookie } from '@/lib/getUserCookie';
import { GlobalFilters, GlobalFiltersSearchParams, RANGE_OPTION } from '@/lib/GlobalFilters/GlobalFilters';

export async function getGlobalFiltersOrDefault(filters: GlobalFiltersSearchParams | undefined): Promise<GlobalFilters> {
  const session = await getUserCookie();
  let user;
  if (!session) {
    user = "gavinhow"
  } else {
    user = filters?.user || session.id
  }

  return {
    user: user,
    range: filters?.range ?? RANGE_OPTION.All_time,
    ...getDateFilterOrDefault(filters),
  }
}

export function getDateFilterOrDefault(filters: GlobalFiltersSearchParams | undefined) {
  if (filters?.range === RANGE_OPTION.Custom && filters.from && filters.to) {
    return {
      from: new Date(filters.from),
      to: new Date(filters.to)
    }
  }
  else {
    switch (filters?.range) {
      case RANGE_OPTION.Last_7_days:
        return getLastNDaysRange(7);
      case RANGE_OPTION.Last_30_days:
        return getLastNDaysRange(30);
      case RANGE_OPTION.Last_365_days:
        return getLastNDaysRange(365);
      case RANGE_OPTION.All_time:
        return getAllTimeRange();
      default:
        return getAllTimeRange();
    }
  }
}

// TODO: Unit test
const getLastNDaysRange = (n: number) => {
  const today = new Date();
  const startDate = new Date();
  startDate.setDate(today.getDate() - n + 1);
  return { from: startDate, to: today };
};

const getYearToDateRange = () => {
  const today = new Date();
  const startDate = new Date(today.getFullYear(), 0, 1);
  return { from: startDate, to: today };
};

const getAllTimeRange = () => {
  return { from: new Date(0), to: new Date() };
};