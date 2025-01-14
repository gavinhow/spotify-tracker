import React from 'react';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { SongRow, SongRowImage } from '@/components/ui/song-row';
import { ChevronRight } from 'lucide-react';
import { TrackPageParams } from '@/app/(authenticated)/track/[trackId]/page';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';

const query = gql(/* GraphQL */`
  query GetArtistsForTrack($trackId: String!) {
    artistsByTrack(trackId: $trackId) {
      artistId
      artist {
        id
        name
        imageUrl
      }
    }
  }
`)

const ArtistList = async ({ trackId }: TrackPageParams) => {
  const { data } = await (await getClient()).query({
    query, variables: {
      trackId
    }
  })
  return (
    <div>
      <div className={'text-xl font-bold mb-3'}>Artists</div>
      <div className="space-y-2 ">
        {data?.artistsByTrack?.map((artist) => (
          <LinkWithGlobalFilters className="block" href={`/artist/${artist.artistId}`} key={artist.artistId}>
            <SongRow>
              <SongRowImage src={artist.artist.imageUrl} alt={artist.artist.name}/>
              <div>{artist.artist.name}</div>
              <div className="ml-auto text-muted-foreground"><ChevronRight/></div>
            </SongRow>
          </LinkWithGlobalFilters>
        ))}
      </div>
    </div>
  );
};

export default ArtistList;
