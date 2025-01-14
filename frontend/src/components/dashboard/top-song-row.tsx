import React, { HTMLAttributes } from 'react';
import { SongRow, SongRowImage, SongRowPlayCount } from '@/components/ui/song-row';
import { cn } from '@/lib/utils';

interface TopSongRowProps {
  name: string;
  artists: string[];
  imageUrl: string;
  playCount: number;
}

const SmallTopSongRow = (props: TopSongRowProps) => {
  return (
    <SongRow className="@container">
      <SongRowImage className="hidden @xs:block w-12 h-12" src={props.imageUrl} alt={props.name}/>
      <TopSongRowArtistDetails trackName={props.name} artists={props.artists}/>
      <SongRowPlayCount>{props.playCount}</SongRowPlayCount>
    </SongRow>
  );
};


const LargeTopSongRow = (props: TopSongRowProps) => {
  return (
    <SongRow>
      <SongRowImage className="hidden @xs:block" src={props.imageUrl} alt={props.name}/>
      <TopSongRowArtistDetails trackName={props.name} artists={props.artists}/>
      <SongRowPlayCount>{props.playCount}</SongRowPlayCount>
    </SongRow>
  );
};


interface TopSongRowArtistDetailsProps {
  trackName: string;
  artists: string[];
}

const TopSongRowArtistDetails = ({trackName, artists, className, ...props }: TopSongRowArtistDetailsProps & HTMLAttributes<HTMLDivElement>) => {
  return (
    <div className={cn('flex flex-col', className)} {...props}>
      <div className="font-bold text-xl whitespace-normal break-words">{trackName}</div>
      <div>{artists.join(', ')}</div>
    </div>)
}
TopSongRowArtistDetails.displayName = 'TopSongRowArtistDetails';

export  { SmallTopSongRow, LargeTopSongRow, TopSongRowArtistDetails};
