'use client'

import * as React from 'react'
import {
  AudioLines, DiscAlbum, History, LayoutDashboard, PersonStanding,
} from 'lucide-react'
import { usePathname } from 'next/navigation'

import { NavMain } from '@/components/nav-main'
import { NavUser } from '@/components/nav-user'
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarRail, SidebarTrigger,
} from '@/components/ui/sidebar'
import { useSuspenseQuery } from '@apollo/client';
import { gql } from '@/__generated__';

// This is sample data.
const data = {
  navMain: [
    {
      title: 'Dashboard',
      url: '/',
      icon: LayoutDashboard,
      isActive: true,
    },
    {
      title: 'Play History',
      url: '/play-history',
      icon: History,
    },
    {
      title: 'Top Songs',
      url: '/top-songs',
      icon: AudioLines,
    },
    {
      title: 'Top Albums',
      url: '/top-albums',
      icon: DiscAlbum,
    },
    {
      title: 'Top Artists',
      url: '/top-artists',
      icon: PersonStanding,
    }
  ],
}

const query = gql(/* GraphQL */`
  query GetMe {
    me {
      id
      displayName
      imageUrl
    }
  }`)

export function AppSidebar({ user, ...props }: { user?: User } & React.ComponentProps<typeof Sidebar>) {
  const { data: me } = useSuspenseQuery(query)
  const pathname = usePathname()

  // Update navigation items with active state based on current path
  const navItems = data.navMain.map(item => ({
    ...item,
    isActive: pathname === item.url || (item.url !== '/' && pathname.startsWith(item.url))
  }))

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <div
          className="flex w-full items-center gap-2 overflow-hidden rounded-md p-2 text-left outline-none ring-sidebar-ring transition-[width,height,padding] focus-visible:ring-2 disabled:pointer-events-none disabled:opacity-50 aria-disabled:pointer-events-none aria-disabled:opacity-50 [&>svg]:size-4 [&>svg]:shrink-0 h-8 text-sm">
          <SidebarTrigger/>
          <span className="truncate">
          Spotify Trigger
          </span>
        </div>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={navItems}/>
      </SidebarContent>
      <SidebarFooter>
        {me.me && <NavUser user={{
          name: me.me.displayName ?? user?.displayName ?? 'Demo User',
          email: '',
          avatar: me.me.imageUrl ?? '',
        }}/>}
      </SidebarFooter>
      <SidebarRail/>
    </Sidebar>
  )
}
