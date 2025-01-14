import React from 'react';
import TrackNameAndPicture from '@/components/track/track-name-and-picture';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';
import ArtistList from '@/components/track/artist-list';
import AppearsOn from '@/components/album/appears-on';

export interface TrackPageParams {
  trackId: string;
}

const TrackPage = async ({ params, searchParams }: {
  params: Promise<TrackPageParams>,
  searchParams: Promise<GlobalFiltersSearchParams>
}) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams)
  const { trackId } = await params;
  return (
    <div className="m-4 space-y-4">
      <TrackNameAndPicture {...globalFilters} trackId={trackId}/>
      <ArtistList trackId={trackId} />
      <AppearsOn trackId={trackId} />
    </div>
  );
};

export default TrackPage;
