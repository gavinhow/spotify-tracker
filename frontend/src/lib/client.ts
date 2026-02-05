import { HttpLink, ApolloLink } from '@apollo/client';
import {
  ApolloClient,
  InMemoryCache,
  registerApolloClient,
} from '@apollo/client-integration-nextjs';
import { getUserCookie } from '@/lib/getUserCookie';
import { ErrorLink } from '@apollo/client/link/error';
import { CombinedGraphQLErrors, ServerError } from '@apollo/client/errors';
import { headers } from 'next/headers';
import { logger } from '@/lib/logger';

const errorLink = new ErrorLink(({ error, operation, forward }) => {
  // Check if error is a ServerError with status code 401
  if (ServerError.is(error) && error.statusCode === 401) {
    logger.warn(
      {
        statusCode: error.statusCode,
        operationName: operation.operationName,
      },
      'GraphQL request returned 401 - authentication required'
    );
    // Handle 401: Redirect to login, refresh token, etc.
    // redirect("/")
  }

  // Check if error is a GraphQL error
  if (CombinedGraphQLErrors.is(error)) {
    error.errors.forEach(({ message, locations, path }) =>
      logger.error(
        {
          operationName: operation.operationName,
          graphqlPath: path?.join('.'),
          locations: locations,
        },
        `GraphQL error: ${message}`
      )
    );
  }
  return forward(operation);
});

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
