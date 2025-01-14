import React, { HTMLAttributes } from 'react';
import { SongRow, SongRowImage, SongRowPlayCount } from '@/components/ui/song-row';

interface TopAlbumRowProps {
  name: string;
  imageUrl: string;
  playCount: number;
}

const SmallTopAlbumRow = (props: TopAlbumRowProps) => {
  return (
    <SongRow>
      <SongRowImage className="w-12 h-12" src={props.imageUrl} alt={props.name}/>
      <div className="font-bold text-xl">{props.name}</div>
      <SongRowPlayCount>{props.playCount}</SongRowPlayCount>
    </SongRow>
  );
};


const LargeTopAlbumRow = (props: TopAlbumRowProps) => {
  return (
    <SongRow>
      <SongRowImage src={props.imageUrl} alt={props.name}/>
      <div className="font-bold text-xl">{props.name}</div>
      <SongRowPlayCount>{props.playCount}</SongRowPlayCount>
    </SongRow>
  );
};



export  { LargeTopAlbumRow, SmallTopAlbumRow};
