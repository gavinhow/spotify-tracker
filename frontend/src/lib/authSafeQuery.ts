import type { ApolloQueryResult, OperationVariables } from '@apollo/client/core/types';
import type { QueryOptions } from '@apollo/client/core/watchQueryOptions';
import type { MaybeMasked } from '@apollo/client/masking';
import { redirect } from 'next/navigation';
import { getClient } from '@/lib/client';
import { ApolloError, NetworkError } from '@apollo/client/errors';
import { ServerError } from '@apollo/client';


export async function AuthSafeQuery<T = unknown, TVariables extends OperationVariables = OperationVariables>(options: QueryOptions<TVariables, T>): Promise<ApolloQueryResult<MaybeMasked<T>>> {
  let response: ApolloQueryResult<MaybeMasked<T>>;
  try {
    response = await (await getClient()).query(options)

  } catch (error: unknown) {
    if (isApolloError(error) && isServerError(error.networkError) && error.networkError.statusCode === 401)
      redirect("/api/logout")

    throw error;
  }


  return response;


}


function isApolloError(error: unknown): error is ApolloError {
  return (error as ApolloError | undefined)?.networkError !== undefined;
}

function isServerError(networkError: NetworkError | undefined): networkError is ServerError{
  return (networkError as ServerError | undefined)?.statusCode !== undefined;
}