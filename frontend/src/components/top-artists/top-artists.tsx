'use client'
import React, { useEffect, useState } from 'react';
import { gql } from '@/__generated__';
import { useWindowScroll } from '@uidotdev/usehooks';
import { useSuspenseQuery } from '@apollo/client';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import { cn } from '@/lib/utils';
import { ArrowUp, ChevronRight } from 'lucide-react';
import InfiniteScroll from 'react-infinite-scroll-component';
import { SongRow, SongRowImage, SongRowPlayCount } from '@/components/ui/song-row';
import LinkWithGlobalFilters from '../link-global-filters/link-global-filters';


const query = gql(/* GraphQL */`
  query GetTopArtists($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {
    topArtists(userId: $userId, from: $from, to: $to, after: $after, first: 20) {
      pageInfo {
        hasNextPage
      }
      edges {
        cursor
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
`
)

const TopSongs = ({ user, from, to }: GlobalFilters) => {
  const [{ y }, scrollTo] = useWindowScroll();
  const [lastCursor, setLastCursor] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState<boolean>(false);

  const { data, fetchMore, refetch } = useSuspenseQuery(query, {
    variables: {
      from, to, userId: user
    }
  })

  useEffect(()=> {
    refetch({
      from, to, userId: user
    })
  }, [ from, refetch, to, user])


  useEffect(() => {
    setHasMore(data.topArtists?.pageInfo.hasNextPage ?? false);
    setLastCursor(data.topArtists?.edges?.at(-1)?.cursor ?? null)
  }, [data])

  const artists = data.topArtists?.edges ?? [];

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
        dataLength={artists.length}
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
          artists.length > 0 ? <p style={{ textAlign: 'center' }}>
            <b>Yay! You have seen it all</b>
          </p> : null
        }
        className={cn('grid w-full')}>
        {artists.map(item => {
          return (
            <LinkWithGlobalFilters className="hover:bg-green-600/15 p-2 rounded-md" href={`/artist/${item.node.artistId}`} key={item.node.artistId}>
              <SongRow className="flex-1" >
                <SongRowImage className="w-12 h-12" src={item.node.artist.imageUrl}
                              alt={item.node.artist.name + ' album art'}/>
                <div>{item.node.artist.name}</div>
                <SongRowPlayCount className="font-normal">
                  {item.node.playCount}
                </SongRowPlayCount>
                <div>
                  <ChevronRight/>
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
