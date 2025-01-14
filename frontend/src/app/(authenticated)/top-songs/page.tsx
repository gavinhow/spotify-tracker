import React from 'react';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import TopSongs from '@/components/top-songs/top-songs';

export interface TopSongsPageSearchParams {
  artistId?: string;
}

const TopSongsPage = async ({ searchParams }: {
  searchParams: Promise<TopSongsPageSearchParams & GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);
  const { artistId } = await searchParams;
  return (
    <div className="m-4">
      <h1 className="text-3xl font-bold mb-6">Top Songs</h1>
      <TopSongs {...globalFilters} artistId={artistId} />
    </div>
  );
};

export default TopSongsPage;
