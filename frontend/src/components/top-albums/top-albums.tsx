'use client'
import React, { useEffect, useState } from 'react';
import { gql } from '@/__generated__';
import { useWindowScroll } from '@uidotdev/usehooks';
import { useSuspenseQuery } from '@apollo/client';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import { TopSongsPageSearchParams } from '@/app/(authenticated)/top-songs/page';
import { cn } from '@/lib/utils';
import { ArrowUp, ChevronRight } from 'lucide-react';
import InfiniteScroll from 'react-infinite-scroll-component';
import { SongRow, SongRowImage, SongRowPlayCount } from '@/components/ui/song-row';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';


const query = gql(/* GraphQL */`
  query GetTopAlbums($userId: String!,$artistId: String, $from: DateTime!, $to: DateTime!, $after: String) {
    topAlbums(userId: $userId, artistId: $artistId, from: $from, to: $to, after: $after, first: 20) {
      pageInfo {
        hasNextPage
      }
      edges {
        cursor
        node {
          albumId
          album {
            name
            imageUrl
          }
          playCount
        }
      }
    }
  }
`
)

const TopSongs = ({ user, from, to, artistId }: GlobalFilters & TopSongsPageSearchParams) => {
  const [{ y }, scrollTo] = useWindowScroll();
  const [lastCursor, setLastCursor] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState<boolean>(false);

  const { data, fetchMore, refetch } = useSuspenseQuery(query, {
    variables: {
      from, to, userId: user, artistId
    }
  })

  useEffect(()=> {
    refetch({
      from, to, userId: user, artistId
    })
  }, [artistId, from, refetch, to, user])

  useEffect(() => {
    setHasMore(data.topAlbums?.pageInfo.hasNextPage ?? false);
    setLastCursor(data.topAlbums?.edges?.at(-1)?.cursor ?? null)
  }, [data])

  const songs = data.topAlbums?.edges ?? [];

  return (
    <div className="relative">
      <div className={cn(`fixed bottom-0 right-0 pb-5 pr-5`, y !== null && y > 50 ? '' : 'hidden')}>
        <div onClick={() => scrollTo({
          top: 0,
          behavior: 'smooth'
        })} className="rounded-full bg-black text-white p-3 px-5 flex items-center">
          <div>Scroll to top</div>
          <ArrowUp className="ml-2"/></div>
      </div>
      <InfiniteScroll
        dataLength={songs.length}
        next={() => {
          fetchMore({
            variables: {
              after: lastCursor
            }
          })
        }}
        hasMore={hasMore}
        loader={<h4>Loading...</h4>}
        endMessage={
          songs.length > 0 ? <p style={{ textAlign: 'center' }}>
            <b>Yay! You have seen it all</b>
          </p> : null
        }
        className={cn('grid w-full')}>
        {songs.map(item => {
          return (
            <LinkWithGlobalFilters className="hover:bg-green-600/15 p-2 rounded-md" href={`/album/${item.node.albumId}`} key={item.node.albumId}>
              <SongRow className="flex-1">
                <SongRowImage src={item.node.album.imageUrl} alt={item.node.album.name}/>
                <div className="font-bold text-xl">{item.node.album.name}</div>
                <SongRowPlayCount>{item.node.playCount}</SongRowPlayCount>
                <div>
                  <ChevronRight />
                </div>
              </SongRow>
            </LinkWithGlobalFilters>
          );

        })}
      </InfiniteScroll>
    </div>
  );
};

export default TopSongs;
