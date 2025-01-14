'use client'
import React, { useMemo } from 'react';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import UserSwitcherList from '@/components/top-navbar/user-switcher-list';
import { useGlobalFilter } from '@/components/top-navbar/GlobalFilterProvider';
import { GetMeAndFriendsQuery } from '@/__generated__/graphql';

interface UserSwitcherClientProps {
  data: GetMeAndFriendsQuery
}


const UserSwitcherClient = ({ data }: UserSwitcherClientProps) => {
  const { user } = useGlobalFilter()
  const [isOpen, setOpen] = React.useState(false);

  const selectedUser = useMemo(() => {
    if (user === undefined || user === data.me?.id) {
      return data.me;
    } else  {
      return data.me?.friends.find(x=> x.id === user);
    }
  }, [data.me, user])

  if (!data.me)
    return null;



  return (
    <Popover open={isOpen} onOpenChange={setOpen}>
      <PopoverTrigger>
        {selectedUser && (<Avatar key={selectedUser.id}>
          { selectedUser.imageUrl !== null && <AvatarImage src={selectedUser.imageUrl} alt={data.me.displayName ?? selectedUser.id}/>}
          <AvatarFallback className="uppercase">{(selectedUser.displayName ?? selectedUser.id).slice(0, 2)}</AvatarFallback>
        </Avatar>)}
      </PopoverTrigger>
      <PopoverContent className="w-auto max-w-screen-sm">
        <UserSwitcherList me={{
          id: data.me.id,
          displayName: data.me.displayName ?? data.me.id,
          imageUrl: data.me.imageUrl,
        }} friends={data.me.friends}
        onChange={() => setOpen(false)} />
      </PopoverContent>
    </Popover>
  );
};

export default UserSwitcherClient;
