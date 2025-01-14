import React, { HTMLAttributes } from 'react';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import { gql } from '@/__generated__';
import { AlbumPageParams } from '@/app/(authenticated)/album/[albumId]/page';
import { getClient } from '@/lib/client';
import { cn } from '@/lib/utils';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';
import { ChevronRight } from 'lucide-react';

const queryAlbumPlays = gql(/* GraphQL */`
  query GetTopSongsByAlbum($userId: String!, $albumId: String!, $from: DateTime!, $to: DateTime!) {
    topSongs(userId: $userId, albumId: $albumId, from: $from, to: $to, first: 50) {
      edges {
        node {
          trackId
          playCount
        }
      }
    }
  }
`)

const queryTrackList = gql(/* GraphQL */`
  query GetTrackList($albumId: String!) {
    album(albumId: $albumId) {
      id
      tracks {
        id
        trackNumber
        name
      }
    }
  }
`)

const TrackList = async ({
                           albumId,
                           from,
                           to,
                           user,
                           className,
                           ...props
                         }: AlbumPageParams & GlobalFilters & HTMLAttributes<HTMLDivElement>) => {
  const { data: albumPlayData } = await (await getClient()).query({
    query: queryAlbumPlays, variables: {
      albumId,
      from,
      to,
      userId: user
    }
  })

  const { data: trackListData } = await (await getClient()).query({
    query: queryTrackList, variables: {
      albumId
    }
  })


  const highestPlay = Math.max(...albumPlayData.topSongs?.edges?.map(track => track.node.playCount) ?? [])


  return (
    <div className={cn('', className)} {...props}>
      <div className={'text-xl font-bold mb-3'}>Songs</div>
      <div className="grid">
        {trackListData.album.tracks
          .map(track => {
            const playData = albumPlayData.topSongs?.edges?.find(x => x.node.trackId === track.id);

            const playPercentage = playData ? (playData.node.playCount / highestPlay) * 100 : 0;

            return (
              <LinkWithGlobalFilters
                key={track.id} href={`/track/${track.id}`}>
                <div
                  className="grid grid-cols-[2rem_1fr_auto_auto] rounded-md mb-2 gap-4 px-4 py-2 transition-colors duration-200 ease-in-out relative overflow-hidden"
                >
                  <div
                    className="absolute inset-0  bg-green-600/75 opacity-25"
                    style={{ width: `${playPercentage}%` }}
                    aria-hidden="true"
                  />
                  <div className="pr-2 text-right">{track.trackNumber}</div>
                  <div className="truncate"> {track.name}</div>
                  <div className="font-bold">{playData?.node.playCount ?? 0}</div>
                  <div className="text-muted-foreground"><ChevronRight /></div>
                </div>
              </LinkWithGlobalFilters>
            );
          })}
      </div>
    </div>
  );
};

export default TrackList;
