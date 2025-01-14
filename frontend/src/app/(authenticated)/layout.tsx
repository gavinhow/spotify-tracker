import React from 'react';
import { GlobalFilterProvider } from '@/components/top-navbar/GlobalFilterProvider';
import { SidebarInset, SidebarProvider, SidebarTrigger } from '@/components/ui/sidebar';
import { AppSidebar } from '@/components/app-sidebar';
import { Separator } from '@/components/ui/separator';
import DateRangeFilter from '@/components/top-navbar/date-range-filter/date-range-filter';
import UserSwitcher from '@/components/top-navbar/user-switcher';
import { ApolloWrapper } from '@/lib/ApolloProvider';
import { getUserCookie } from '@/lib/getUserCookie';
import DemoProvider from '@/lib/DemoProvider/demo-provider';
import { headers } from 'next/headers';
import { gql } from '@/__generated__';
import { AuthSafeQuery } from '@/lib/authSafeQuery';
import { CircleCheckBig } from 'lucide-react';

const query = gql(/* GraphQL */`
  query Get1Play($userId: String!) {
    plays(userId: $userId, first: 1) {
      edges {
        cursor
        node {
          trackId
        }
      }
    }
  }
`
)

const AuthenticatedLayout = async ({ children }: { children: React.ReactNode }) => {
  const user = await getUserCookie();

  const headersList = await headers();
  const isDemo = headersList.get('is_demo') === 'true';

  const { data } = await AuthSafeQuery({
    query,
    variables: {
      userId: user?.id ?? 'gavinhow'
    }
  })

  if (data?.plays?.edges?.length === 0) {
    return <div className="p-2 h-svh w-svw flex flex-row justify-center items-center">
      <div className="text-center space-y-8">
        <div><CircleCheckBig className="mx-auto " size={96}/></div>
        <h1 className="text-3xl mb-2 font-bold">Thanks for signing up!</h1>
        <div>
          <div>No data has been imported from Spotify yet.</div>
          <div>Check back tomorrow to see your dashboard start filling up.</div>
        </div>
      </div>
    </div>
  }

  return (
    <DemoProvider isDemo={isDemo}>
      <ApolloWrapper user={user}>
        <GlobalFilterProvider>
          <SidebarProvider>
            <AppSidebar user={user}/>
            <SidebarInset>
              <header
                className="px-4 mt-2 flex h-16 shrink-0 items-center gap-4 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-12">
                <SidebarTrigger className="block md:hidden [&_svg]:size-8 h-auto w-auto"/>
                <div className="h-full py-2 shrink block md:hidden">
                  <Separator orientation="vertical"/>
                </div>
                <div className="flex flex-1 items-center gap-2">
                  <DateRangeFilter/>
                  <UserSwitcher/>
                </div>
              </header>
              {children}
            </SidebarInset>
          </SidebarProvider>
        </GlobalFilterProvider>
      </ApolloWrapper>
    </DemoProvider>
  );
};

export default AuthenticatedLayout;
