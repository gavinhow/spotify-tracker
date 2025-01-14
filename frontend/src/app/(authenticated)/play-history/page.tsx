import PlayHistory from '@/components/play-history/play-history';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';

export default async function Home({ searchParams }: { searchParams?: Promise<GlobalFiltersSearchParams> }) {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);

  return (
    <div className="m-4">
      <h1 className="text-3xl font-bold">Play history</h1>
      <PlayHistory {...globalFilters} />
    </div>
  );
}
