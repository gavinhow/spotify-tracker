import React from 'react';
import { gql } from '@/__generated__';
import UserSwitcherClient from '@/components/top-navbar/user-switcher-client';
import { AuthSafeQuery } from '@/lib/authSafeQuery';

export const getMeQuery = gql(/* GraphQL */`
  query GetMeAndFriends {
    me {
      id
      displayName
      imageUrl
      friends {
        id
        displayName
        imageUrl
      }
    }
  }`)

const UserSwitcher = async () => {
  const { data } = await AuthSafeQuery({
    query: getMeQuery
  })

  return (
    data && <UserSwitcherClient data={data}/>
  );
};

export default UserSwitcher;
