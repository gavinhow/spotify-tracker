'use client'
import React from 'react';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { PlusCircle } from 'lucide-react';
import { useGlobalFilter } from '@/components/top-navbar/GlobalFilterProvider';
import { cn } from '@/lib/utils';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';

interface UserSwitcherListProps {
  me: {
    id: string;
    displayName: string;
    imageUrl: string | null | undefined;
  }
  friends: {
    id: string;
    displayName?: string | null | undefined;
    imageUrl?: string | null | undefined ;
  }[]
  onChange?: () => void
}

const UserSwitcherList = ({ me, friends, onChange }: UserSwitcherListProps) => {
  const { user, setUser } = useGlobalFilter();

  const onClick = (userId: string) => {
    setUser(userId)
    onChange?.()
  }

  return (
    <div className="grid gap-2">
      <UserSwitcherItem key={me.id}
                        onClick={() => onClick(me.id)}
                        id={me.id}
                        selected={user === me.id}
                        displayName={me.displayName}
                        imageUrl={me.imageUrl}/>
      {friends.map((friend) => {
        return (
          <UserSwitcherItem key={friend.id}
                            onClick={() => onClick(friend.id)}
                            id={friend.id}
                            selected={user === friend.id}
                            displayName={friend.displayName}
                            imageUrl={friend.imageUrl}/>
        )
      })}
      <LinkWithGlobalFilters href={'/account/friends'} className="flex items-center p-2 hover:bg-green-600/15 rounded-sm">
        <div className="h-8 w-8 flex items-center justify-center mr-2">
          <PlusCircle className=""/>
        </div>
        <div>Add a friend</div>
      </LinkWithGlobalFilters>
    </div>
  );
};

interface UserSwitcherItemProps {
  id: string;
  displayName: string | null | undefined ;
  imageUrl: string | null | undefined ;
  onClick?: () => void;
  selected?: boolean;
}

const UserSwitcherItem = (props: UserSwitcherItemProps) => {
  return <button onClick={props.onClick} key={props.id}
                 className={cn('flex items-center p-2 hover:bg-green-600/15 rounded-sm ', props.selected ? 'bg-green-600/15 ' : '')}>
    <Avatar className="mr-2 h-8 w-8">
      {props.imageUrl && <AvatarImage sizes="64px" src={props.imageUrl} alt="@shadcn"/>}
      <AvatarFallback className="uppercase">{(props.displayName ?? props.id).slice(0, 2) }</AvatarFallback>
    </Avatar>
    <div>
      {props.displayName ?? props.id}
    </div>
  </button>
}

export default UserSwitcherList;
