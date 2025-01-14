import React, { HTMLAttributes } from 'react';
import { Card } from '@/components/ui/card';
import Image from 'next/image';
import { cn } from '@/lib/utils';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import LinkWithGlobalFilters from '../link-global-filters/link-global-filters';

const query = gql(/* GraphQL */`
  query GetTopArtist($userId: String!, $from: DateTime!, $to: DateTime!) {
    topArtists(userId: $userId, from: $from, to: $to, first: 1) {
      edges {
        node {
          artistId
          artist {
            name
            imageUrl
          }
          playCount
        }
      }
    }
  }
`);

const TopArtistCard = async ({
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

  const artists = data.topArtists?.edges ?? [];
  const topArtist = artists.length > 0 ? artists?.[0].node: undefined;
  const name = topArtist?.artist.name;
  const imageUrl = topArtist?.artist.imageUrl;
  const playCount = topArtist?.playCount;

  if (!name || !imageUrl) {
    return null;
  }

  return (
    <LinkWithGlobalFilters href={`/artist/${topArtist?.artistId}`}>
      <Card className={cn('aspect-square relative overflow-hidden border-0', className)} {...props}>
        <Image priority={true} sizes="512px" src={imageUrl} style={{ objectFit: 'cover' }} alt={name} fill={true}/>
        <div style={{
          mask: 'linear-gradient(transparent, black, black)'
        }} className="absolute inset-x-0 bottom-0 h-1/3 backdrop-blur-lg"/>
        <div className="absolute inset-x-0 bottom-0 p-6 text-white">
          <h2 className="text-4xl font-bold ">{name}</h2>
          <div className=""><span className="text-lg font-semibold">{playCount}</span> plays</div>
        </div>
      </Card>
    </LinkWithGlobalFilters>
  );
};

export default TopArtistCard;
