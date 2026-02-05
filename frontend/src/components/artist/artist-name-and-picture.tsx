import React from 'react';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import { Card } from '@/components/ui/card';

const query = gql(/* GraphQL */`
  query ArtistNameAndPicture($artistId: String!) {
    artist(artistId: $artistId) {
      name,
      imageUrl
    }
  }
`)

interface ArtistNameAndPictureProps {
  artistId: string;
}

const ArtistNameAndPicture = async ({ artistId }: ArtistNameAndPictureProps) => {
  const { data } = await (await getClient()).query(
    {
      query,
      variables: {
        artistId: artistId,
      }
    }
  )

  if (!data) {
    return null;
  }
  return (
    <div>
      <div className="text-2xl font-bold mb-3">
        {data.artist.name}
      </div>
      <Card className="shadow-none border-0 aspect-square w-full relative rounded-md overflow-hidden">
        <img src={data.artist.imageUrl} className="absolute h-full w-full" style={{
          objectFit: 'cover',
        }} alt={data.artist.name}/>
      </Card>
    </div>
  );
};

export default ArtistNameAndPicture;
