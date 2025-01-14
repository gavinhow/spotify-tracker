import React from 'react';
import ArtistNameAndPicture from '@/components/artist/artist-name-and-picture';
import TopSongsCard from '@/components/artist/top-songs-card';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import TopAlbumsCard from '@/components/artist/top-albums-card';

export interface ArtistPageParams {
  artistId: string;
}

const ArtistPage = async ({ params, searchParams }: {
  params: Promise<ArtistPageParams>,
  searchParams: Promise<GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);

  const { artistId } = await params;
  return (
    <div className="m-4 space-y-4">
      <ArtistNameAndPicture artistId={artistId}/>
      <TopSongsCard artistId={artistId} {...globalFilters} />
      <TopAlbumsCard artistId={artistId} {...globalFilters} />
    </div>
  );
};

export default ArtistPage;
