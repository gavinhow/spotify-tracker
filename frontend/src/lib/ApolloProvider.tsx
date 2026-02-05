'use client';

import type { PropsWithChildren } from 'react';
import { ApolloLink, HttpLink } from '@apollo/client';
import { ErrorLink } from '@apollo/client/link/error';
import { CombinedGraphQLErrors, ServerError } from '@apollo/client/errors';
import {
  ApolloClient,
  ApolloNextAppProvider,
  InMemoryCache,
  SSRMultipartLink,
} from '@apollo/client-integration-nextjs';
import { useDemo } from '@/lib/DemoProvider/demo-provider-client';

const errorLink = new ErrorLink(({ error, operation }) => {
  // Check if error is a ServerError with status code 401
  if (typeof window !== 'undefined' && ServerError.is(error) && error.statusCode === 401) {
    // Handle 401: Redirect to login, refresh token, etc.
    console.log('Unauthorised! Redirecting to login...');
    window.location.href = '/api/logout';
    return undefined;
  }

  // Check if error is a GraphQL error
  if (CombinedGraphQLErrors.is(error)) {
    error.errors.forEach(({ message, locations, path }) =>
      console.error(`[GraphQL error]: Message: ${message}, Location: ${locations}, Path: ${path}`)
    );
  }
});

// have a function to create a client for you
function makeClient(token: string | undefined, isDemo: boolean) {
  const authHeader: { ApiKey: string } | {
    Authorization: string
  } = isDemo ? { 'ApiKey': process.env.NEXT_PUBLIC_DEMO_API_KEY ?? '' } : {
    'Authorization': `Bearer ${token}`,
  }
  const httpLink = new HttpLink({
    // Use public API URL for client-side requests (from user's browser)
    // This needs to be an absolute url, as relative urls cannot be used in SSR
    uri: process.env.NEXT_PUBLIC_API_URL + '/graphql',
    headers: authHeader,
    // you can disable result caching here if you want to
    // (this does not work if you are rendering your page with `export const dynamic = "force-static"`)
    // fetchOptions: { cache: "no-store" },
    // you can override the default `fetchOptions` on a per query basis
    // via the `context` property on the options passed as a second argument
    // to an Apollo Client data fetching hook, e.g.:
    // const { data } = useSuspenseQuery(MY_QUERY, { context: { fetchOptions: { cache: "force-cache" }}});
  });

  // use the `ApolloClient` from "@apollo/client-integration-nextjs"
  return new ApolloClient({
      // use the `InMemoryCache` from "@apollo/client-integration-nextjs"
      cache: new InMemoryCache({
        typePolicies: {
          Query: {
            fields: {
              topSongs: {
                keyArgs: ['userId', 'artistId', 'from', 'to'],
                // Concatenate the incoming list items with
                // the existing list items.
                merge(existing = { edges: [] }, incoming) {
                  return {
                    ...incoming,
                    edges: [
                      ...existing.edges,
                      ...incoming.edges,
                    ]
                  }
                },
              },
              topAlbums: {
                keyArgs: ['userId', 'artistId', 'from', 'to'],
                // Concatenate the incoming list items with
                // the existing list items.
                merge(existing = { edges: [] }, incoming) {
                  return {
                    ...incoming,
                    edges: [
                      ...existing.edges,
                      ...incoming.edges,
                    ]
                  }
                },
              },
              topArtists: {
                keyArgs: ['userId', 'from', 'to'],
                // Concatenate the incoming list items with
                // the existing list items.
                merge(existing = { edges: [] }, incoming) {
                  return {
                    ...incoming,
                    edges: [
                      ...existing.edges,
                      ...incoming.edges,
                    ]
                  }
                },
              }
            }
          }
        }
      }),
      link:
        typeof window === 'undefined'
          ? ApolloLink.from([new SSRMultipartLink({ stripDefer: true }), errorLink, httpLink])
          : ApolloLink.from([errorLink, httpLink]),
    }
  )
    ;
}

interface ApolloWrapperProps {
  user?: User;
}

// you need to create a component to wrap your app in
export function ApolloWrapper({ children, user }: PropsWithChildren & ApolloWrapperProps) {
  const {isDemo} = useDemo();
  return (
    <ApolloNextAppProvider makeClient={() => makeClient(user?.token, isDemo)}>
      {children}
    </ApolloNextAppProvider>
  );
}
