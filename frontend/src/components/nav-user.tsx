'use client'

import {
  BadgeCheck,
  ChevronsUpDown,
  Handshake, LogOut,
} from 'lucide-react'

import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from '@/components/ui/avatar'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from '@/components/ui/sidebar'
import SignOut from '@/components/auth/sign-out';
import LinkWithGlobalFilters from '@/components/link-global-filters/link-global-filters';

export function NavUser({
                          user,
                        }: {
  user: {
    name: string
    email?: string
    avatar: string
  }
}) {
  const { isMobile } = useSidebar()

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <Avatar className="h-8 w-8 rounded-lg">
                <AvatarImage src={user.avatar} alt={user.name}/>
                <AvatarFallback className="rounded-lg uppercase">{user.name.slice(0, 2)}</AvatarFallback>
              </Avatar>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-semibold">{user.name}</span>
                <span className="truncate text-xs">{user.email}</span>
              </div>
              <ChevronsUpDown className="ml-auto size-4"/>
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
            side={isMobile ? 'bottom' : 'right'}
            align="end"
            sideOffset={4}
          >
            <DropdownMenuLabel className="p-0 font-normal">
              <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
                <Avatar className="h-8 w-8 rounded-lg">
                  <AvatarImage src={user.avatar} alt={user.name}/>
                  <AvatarFallback className="rounded-lg uppercase">{user.name.slice(0, 2)}</AvatarFallback>
                </Avatar>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-semibold">{user.name}</span>
                  <span className="truncate text-xs">{user.email}</span>
                </div>
              </div>
            </DropdownMenuLabel>
            <DropdownMenuSeparator/>
            <DropdownMenuGroup>
              <LinkWithGlobalFilters href={'/account'}>
                <DropdownMenuItem>
                  <BadgeCheck/>
                  Account
                </DropdownMenuItem>
              </LinkWithGlobalFilters>
              <LinkWithGlobalFilters href={'/account/friends'}>
                <DropdownMenuItem>
                  <Handshake/>
                  Friends
                </DropdownMenuItem>
              </LinkWithGlobalFilters>
            </DropdownMenuGroup>
            <DropdownMenuSeparator/>
            <SignOut>
              <DropdownMenuItem>
                <LogOut/>
                Log out
              </DropdownMenuItem>
            </SignOut>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
