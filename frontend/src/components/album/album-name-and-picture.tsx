import React from 'react';
import { gql } from '@/__generated__';
import { getClient } from '@/lib/client';
import Image from 'next/image';
import { Card } from '@/components/ui/card';

const query = gql(/* GraphQL */`
  query AlbumNameAndPicture($albumId: String!) {
    album(albumId: $albumId) {
      id,
      name,
      imageUrl
    }
  }
`)

interface AlbumNameAndPictureProps {
  albumId: string;
}

const AlbumNameAndPicture = async ({ albumId }: AlbumNameAndPictureProps) => {
  const { data } = await (await getClient()).query(
    {
      query,
      variables: {
        albumId: albumId,
      }
    }
  )
  return (
    <div>
      <div className="text-2xl font-bold mb-3">
        {data.album.name}
      </div>
      <Card className="shadow-none border-0 aspect-square w-full relative rounded-md overflow-hidden">
        <Image priority src={data.album.imageUrl} fill={true} style={{
          objectFit: 'cover',
        }} alt={data.album.name}/>
      </Card>
    </div>
  );
};

export default AlbumNameAndPicture;
