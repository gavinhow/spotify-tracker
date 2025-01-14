import React, { HTMLAttributes, ImgHTMLAttributes } from 'react';
import { cn } from '@/lib/utils';

const SongRow = ({ className, ...props }: HTMLAttributes<HTMLDivElement>) => {
  return (
    <div className={cn('flex gap-5 items-center @container', className)} {...props} />)


}
SongRow.displayName = 'SongRow';

const SongRowImage = ({ className, alt, ...props }: ImgHTMLAttributes<HTMLImageElement>) => {
  return (
    <div className={cn('h-16 w-16 relative rounded-xl overflow-hidden shrink-0 hidden @xs:block', className)}>
      <img sizes="64px" style={{ objectFit: 'cover' }} alt={alt} className="absolute h-full w-full" {...props} />
    </div>
  )
}
SongRowImage.displayName = "SongRowImage";

const SongRowPlayCount = ({ className, ...props }: HTMLAttributes<HTMLDivElement>) => {
  return (
    <div className={cn('ml-auto font-bold text-xl', className)} {...props} />
  )
}
SongRowPlayCount.displayName = 'SongRowPlayCount';

export { SongRow, SongRowImage, SongRowPlayCount };
