import React from 'react';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import TopArtists from '@/components/top-artists/top-artists';


const TopArtistsPage = async ({ searchParams }: {
  searchParams: Promise<GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);
  return (
    <div className="m-4">
      <h1 className="text-3xl font-bold mb-6">Top Songs</h1>
      <TopArtists {...globalFilters} />
    </div>
  );
};

export default TopArtistsPage;
