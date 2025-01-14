import React, { HTMLAttributes } from 'react';
import { Card } from '@/components/ui/card';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { cn } from '@/lib/utils';
import { LargeTopSongRow, SmallTopSongRow } from '@/components/dashboard/top-song-row';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import LinkWithGlobalFilters from '../link-global-filters/link-global-filters';

const query = gql(/* GraphQL */`
  query GetTop3Songs($userId: String!, $from: DateTime!, $to: DateTime!) {
    topSongs(userId: $userId, from: $from, to: $to, first: 5) {
      edges {
        node {
          trackId
          track {
            name
            albumArtUrl
            artists {
              name
            }
          }
          playCount
        }
      }
    }
  }
`);

const TopSongsCard = async ({
                              user,
                              from,
                              to,
                              className,
                              ...props
                            }: GlobalFilters & HTMLAttributes<HTMLDivElement>) => {
  const { data } = await (await getClient()).query({
    query,
    variables: {
      userId: user,
      from: from ?? new Date(0),
      to: to ?? new Date(),
    }
  });

  if (!data.topSongs?.edges || data.topSongs?.edges?.length === 0) {
    return null;
  }


  return (
    <LinkWithGlobalFilters href={"/top-songs"}>
      <Card className={cn('p-6', className)} {...props}>
        <h2 className="text-2xl font-bold mb-6">Top Songs</h2>
        <div className="flex flex-col gap-4">
          {data.topSongs.edges.slice(0, 1).map((edge) => (
            <LargeTopSongRow artists={edge.node.track.artists.map(x => x.name)} playCount={edge.node.playCount}
                             name={edge.node.track.name} imageUrl={edge.node.track.albumArtUrl}
                             key={edge.node.trackId}/>
          ))}
          {data.topSongs.edges.slice(1).map((edge) => (
            <SmallTopSongRow artists={edge.node.track.artists.map(x => x.name)} playCount={edge.node.playCount}
                             name={edge.node.track.name} imageUrl={edge.node.track.albumArtUrl}
                             key={edge.node.trackId}/>
          ))}
        </div>
        <div className="text-center text-muted-foreground mt-4">See more</div>
      </Card>
    </LinkWithGlobalFilters>
  );
};

export default TopSongsCard;
