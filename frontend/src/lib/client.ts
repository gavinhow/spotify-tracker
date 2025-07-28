import { HttpLink, InMemoryCache, ApolloClient, ApolloLink, ServerError } from '@apollo/client';
import { registerApolloClient } from '@apollo/experimental-nextjs-app-support';
import { getUserCookie } from '@/lib/getUserCookie';
import { onError } from '@apollo/client/link/error';
import { NetworkError } from '@apollo/client/errors';
import { headers } from 'next/headers';


const errorLink = onError(({ graphQLErrors, networkError, operation, forward }) => {
  if (isServerError(networkError) && networkError.statusCode === 401) {
    // Handle 401: Redirect to login, refresh token, etc.
    // redirect("/")
  }

  if (graphQLErrors) {
    graphQLErrors.forEach(({ message, locations, path }) =>
      console.error(`[GraphQL error]: Message: ${message}, Location: ${locations}, Path: ${path}`)
    );
  }
  return forward(operation)
});

function isServerError(networkError: NetworkError | undefined): networkError is ServerError {
  return (networkError as ServerError).statusCode !== undefined;
}

export const { getClient } = registerApolloClient(async () => {
  const user = await getUserCookie();
  const headersList = await headers();
  const authHeader: { ApiKey: string } | {
    Authorization: string
  } = headersList.get('is_demo') === "true" ? { 'ApiKey': process.env.NEXT_PUBLIC_DEMO_API_KEY ?? '' } : {
    'Authorization': `Bearer ${user?.token}`,
  }
  return new ApolloClient({
    cache: new InMemoryCache(),
    link: ApolloLink.from([errorLink,
      new HttpLink({
        // Use internal API URL for server-side requests within Docker network
        uri: (process.env.INTERNAL_API_URL || process.env.NEXT_PUBLIC_API_URL) + '/graphql',
        headers: authHeader,
      })
    ]),
  });
});