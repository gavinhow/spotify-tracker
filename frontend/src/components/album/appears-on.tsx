import React from 'react';
import { gql } from '@/__generated__';
import { TrackPageParams } from '@/app/(authenticated)/track/[trackId]/page';
import { getClient } from '@/lib/client';
import { SongRow, SongRowImage } from '@/components/ui/song-row';
import { ChevronRight } from 'lucide-react';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';

const query = gql(/* GraphQL */`
  query GetAlbumsForTrack($trackId: String!) {
    track(trackId: $trackId) {
      id
      album {
        id
        name
        imageUrl
      }
    }
  }
`)

const AppearsOn = async ({ trackId }: TrackPageParams) => {
  const { data } = await (await getClient()).query({
    query,
    variables: {
      trackId
    },
  })

  const album = data?.track.album;

  if (!album) {
    return null;
  }
  return (
    <div>
      <div className={'text-xl font-bold mb-3'}>Appears on</div>
      <div>
        <LinkWithGlobalFilters href={`/album/${album.id}`} key={album.id}>
          <SongRow>
            <SongRowImage src={album.imageUrl} alt={album.name}/>
            <div>{album.name}</div>
            <div className="ml-auto text-muted-foreground"><ChevronRight/></div>
          </SongRow>
        </LinkWithGlobalFilters>
      </div>
    </div>
  );
};

export default AppearsOn;
