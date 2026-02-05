'use client'
import React, { useEffect, useState } from 'react';
import { gql } from '@/__generated__';
import { GlobalFilters } from '@/lib/GlobalFilters/GlobalFilters';
import { SongRow, SongRowImage, SongRowPlayCount } from '@/components/ui/song-row';
import { cn } from '@/lib/utils';
import { TopSongRowArtistDetails } from '@/components/dashboard/top-song-row';
import { format } from 'date-fns';
import { GetPlaysQuery } from '@/__generated__/graphql';
import { useSuspenseQuery } from '@apollo/client/react';
import InfiniteScroll from 'react-infinite-scroll-component';
import { ArrowUp, ChevronRight } from 'lucide-react';
import { useWindowScroll } from '@uidotdev/usehooks';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';


const query = gql(/* GraphQL */`
  query GetPlays($userId: String!, $from: DateTime!, $to: DateTime!, $after: String) {
    plays(userId: $userId, from: $from, to: $to, after: $after, first: 20) {
      pageInfo {
        hasNextPage
      }
      edges {
        cursor
        node {
          trackId
          track {
            name
            albumArtUrl
            artists {
              name
            }
          }
          timeOfPlay
        }
      }
    }
  }
`
)

interface SectionListItem {
  type: 'section'
  date: string
}

interface PlayListItem {
  type: 'item'
  timeOfPlay: string
  trackId: string
  track: {
    name: string
    albumArtUrl: string,
    artists: Array<{ name: string }>
  }
}

type ListItem = SectionListItem | PlayListItem

const PlayHistory = ({ user, from, to }: GlobalFilters) => {
  const [{ y }, scrollTo] = useWindowScroll();
  const [lastCursor, setLastCursor] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState<boolean>(false);

  function flattenData(data: GetPlaysQuery | undefined, lastRenderedSectionDate?: string): ListItem[] {
    if (!data) return [];
    const flattened = [] as ListItem[];
    let tempLastCursor = lastCursor || null;
    let lastDate: string | undefined = lastRenderedSectionDate;
    const plays = data.plays?.edges ?? []

    plays.forEach((item) => {
      const play = item.node;
      const date = format(new Date(play.timeOfPlay), 'yyyy-MM-dd');
      if (date !== lastDate) {
        flattened.push({ type: 'section', date });
        lastDate = date;
      }
      tempLastCursor = item.cursor
      flattened.push({ type: 'item', ...play });
    });

    if (lastCursor !== tempLastCursor) {
      setLastCursor(tempLastCursor);
    }
    const hasNextPage = data.plays?.pageInfo.hasNextPage;
    if (hasNextPage !== hasMore)
      setHasMore(hasNextPage ?? false);
    return flattened;
  }

  const { data, fetchMore } = useSuspenseQuery(query, {
    variables: {
      from, to, userId: user
    }
  })

  const [flattenedData, setFlattenedData] = useState<ListItem[]>([])

  useEffect(() => {
    setFlattenedData(flattenData(data))
  }, [data.plays?.edges])

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
        dataLength={flattenedData.length}
        next={() => {
          fetchMore({
            variables: {
              after: lastCursor
            }
          }).then((results) => {
            const lastRenderedSectionDate = flattenedData
              .slice()
              .reverse()
              .find((entry) => entry.type === 'section')?.date;
            if (!results.data) return;
            setFlattenedData((prev) => [...prev, ...flattenData(results.data, lastRenderedSectionDate)])
          })
        }}
        hasMore={hasMore}
        loader={<h4>Loading...</h4>}
        endMessage={
          flattenedData.length > 0 ? <p style={{ textAlign: 'center' }}>
            <b>Yay! You have seen it all</b>
          </p> : null
        }
        className={cn('grid w-full')}>
        {flattenedData.map(item => {
          if (item.type === 'section') {
            return (<div className="text-center font-bold pt-12 pb-4 text-xl"
                         key={item.date}>{format(new Date(item.date), 'do MMMM yyyy')}</div>)
          }
          if (item.type === 'item') {
            return (
              <LinkWithGlobalFilters className="hover:bg-green-600/15 p-2 rounded-md" href={`/track/${item.trackId}`}
                    key={item.trackId + item.timeOfPlay}>
                <SongRow className="flex-1">
                  <SongRowImage className="w-12 h-12" src={item.track.albumArtUrl}
                                alt={item.track.name + ' album art'}/>
                  <TopSongRowArtistDetails trackName={item.track.name}
                                           artists={item.track.artists.map(x => x.name)}/>
                  <SongRowPlayCount className="font-normal">
                    {format(new Date(item.timeOfPlay), 'HH:mm')}
                  </SongRowPlayCount>
                  <div>
                    <ChevronRight />
                  </div>
                </SongRow>
              </LinkWithGlobalFilters>
            );
          }

          return null;
        })}
      </InfiniteScroll>
    </div>
  );
};

export default PlayHistory;
