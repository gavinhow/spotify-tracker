import React from 'react';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import TopAlbums from '@/components/top-albums/top-albums';

export interface TopAlbumsPageSearchParams {
  artistId?: string;
}

const TopAlbumsPage = async ({ searchParams }: {
  searchParams: Promise<TopAlbumsPageSearchParams & GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);
  const { artistId } = await searchParams;
  return (
    <div className="m-4">
      <h1 className="text-3xl font-bold mb-6">Top Albums</h1>
      <TopAlbums {...globalFilters} artistId={artistId} />
    </div>
  );
};

export default TopAlbumsPage;
