import React, { HTMLAttributes } from 'react';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { cn } from '@/lib/utils';
import { Card } from '@/components/ui/card';
import { LargeTopAlbumRow, SmallTopAlbumRow } from './top-album-row';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import Link from 'next/link';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';


const query = gql(/* GraphQL */`
  query GetTop3Albums($userId: String!, $from: DateTime!, $to: DateTime!) {
    topAlbums(userId: $userId, from: $from, to: $to, first: 5) {
      edges {
        node {
          albumId
          album {
            name
            imageUrl
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

const TopAlbumsCard = async ({
                               user,
                               className,
                               from,
                               to,
                               ...props
                             }: GlobalFilters & HTMLAttributes<HTMLDivElement>) => {
  const { data } = await (await getClient()).query({
    query,
    variables: {
      userId: user,
      from: from,
      to: to,
    }
  });

  if (!data.topAlbums?.edges || data.topAlbums?.edges?.length === 0) {
    return null;
  }

  return (
    <LinkWithGlobalFilters className="block" href={`/top-albums`}>
      <Card className={cn('p-6', className)} {...props}>
        <h2 className="text-2xl font-bold mb-6">Top Albums</h2>
        <div className="flex flex-col gap-4">
          {data.topAlbums.edges.slice(0, 1).map((edge) => (
            <LargeTopAlbumRow playCount={edge.node.playCount}
                              name={edge.node.album.name} imageUrl={edge.node.album.imageUrl}
                              key={edge.node.albumId}/>
          ))}
          {data.topAlbums.edges.slice(1).map((edge) => (
            <SmallTopAlbumRow playCount={edge.node.playCount}
                              name={edge.node.album.name} imageUrl={edge.node.album.imageUrl}
                              key={edge.node.albumId}/>
          ))}
        </div>
        <div className="text-center text-muted-foreground mt-4">See more</div>
      </Card>
    </LinkWithGlobalFilters>
  );
};

export default TopAlbumsCard;
