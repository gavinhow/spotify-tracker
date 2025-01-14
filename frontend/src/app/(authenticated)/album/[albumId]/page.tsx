import React from 'react';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import AlbumNameAndPicture from '@/components/album/album-name-and-picture';
import TrackList from '@/components/album/track-list';
import ArtistList from '@/components/album/artist-list';


export interface AlbumPageParams {
  albumId: string;
}

const AlbumPage = async ({ params, searchParams }: {
  params: Promise<AlbumPageParams>,
  searchParams: Promise<GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);

  const { albumId } = await params;

  return (
    <div className="m-4 space-y-4">
      <AlbumNameAndPicture albumId={albumId} />
      <ArtistList albumId={albumId} />
      <TrackList albumId={albumId} {...globalFilters} />
    </div>
  );
};

export default AlbumPage;
