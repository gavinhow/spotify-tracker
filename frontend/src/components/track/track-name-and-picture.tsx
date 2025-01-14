import React from 'react';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { Card } from '@/components/ui/card';
import { TrackPageParams } from '@/app/(authenticated)/track/[trackId]/page';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';

const query = gql(/* GraphQL */`
  query TrackNameAndPicture($trackId: String!, $userId: String!, $from: DateTime, $to: DateTime) {
    track(trackId: $trackId) {
      id,
      name,
      albumArtUrl,
      playCount(userId:$userId, from: $from, to: $to )
    }
  }
`)

const AlbumNameAndPicture = async ({ trackId, user, from, to }: TrackPageParams & GlobalFilters) => {
  const { data } = await (await getClient()).query(
    {
      query,
      variables: {
        trackId: trackId,
        userId: user, from, to
      }
    }
  )
  return (
    <div>
      <div className="text-2xl font-bold mb-3">
        {data.track.name}
      </div>
      <div className="flex">
        <Card className="w-1/3 shadow-none border-0 aspect-square relative rounded-md overflow-hidden">
          <img className="absolute h-full w-full" fetchPriority="high" src={data.track.albumArtUrl} style={{
            objectFit: 'cover',
          }} alt={data.track.name}/>
        </Card>
        <div className="w-2/3 flex justify-center flex-col items-center">
          <div className="mb-2">Plays</div>
          <div className="text-3xl font-bold">{data.track.playCount}</div>
        </div>
      </div>
    </div>
  );
};

export default AlbumNameAndPicture;
