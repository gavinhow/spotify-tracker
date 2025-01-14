'use client';

import { ApolloLink, HttpLink, ServerError } from '@apollo/client';
import { onError } from '@apollo/client/link/error';
import { NetworkError } from '@apollo/client/errors';
import { ApolloNextAppProvider,InMemoryCache,  ApolloClient } from '@apollo/experimental-nextjs-app-support';
import { useDemo } from '@/lib/DemoProvider/demo-provider-client';

const responseLogger = new ApolloLink((operation, forward) => {
  return forward(operation).map(result => {
    console.info(JSON.stringify(result));
    return result
  })
})


const errorLink = onError(({ graphQLErrors, networkError }) => {
  if (isServerError(networkError) && networkError.statusCode === 401) {
    // Handle 401: Redirect to login, refresh token, etc.
    console.log('Unauthorised! Redirecting to login...');
    window.location.href = '/api/logout';
    return undefined;
  }

  if (graphQLErrors) {
    graphQLErrors.forEach(({ message, locations, path }) =>
      console.error(`[GraphQL error]: Message: ${message}, Location: ${locations}, Path: ${path}`)
    );
  }
});

function isServerError(networkError: NetworkError | undefined): networkError is ServerError {
  return (networkError as ServerError).statusCode !== undefined;
}

// have a function to create a client for you
function makeClient(token: string | undefined, isDemo: boolean) {
  const authHeader: { ApiKey: string } | {
    Authorization: string
  } = isDemo ? { 'ApiKey': process.env.NEXT_PUBLIC_DEMO_API_KEY ?? '' } : {
    'Authorization': `Bearer ${token}`,
  }
  const httpLink = new HttpLink({
    // this needs to be an absolute url, as relative urls cannot be used in SSR
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

  // use the `ApolloClient` from "@apollo/experimental-nextjs-app-support"
  return new ApolloClient({
      // use the `InMemoryCache` from "@apollo/experimental-nextjs-app-support"
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
        ApolloLink.from([errorLink, httpLink]),
    }
  )
    ;
}

interface ApolloWrapperProps {
  user?: User;
}

// you need to create a component to wrap your app in
export function ApolloWrapper({ children, user }: React.PropsWithChildren & ApolloWrapperProps) {
  const {isDemo} = useDemo();
  return (
    <ApolloNextAppProvider makeClient={() => makeClient(user?.token, isDemo)}>
      {children}
    </ApolloNextAppProvider>
  );
}