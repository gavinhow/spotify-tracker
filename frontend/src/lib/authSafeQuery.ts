import type {OperationVariables, QueryOptions} from '@apollo/client';
import {redirect} from 'next/navigation';
import {getClient} from '@/lib/client';
import type {ServerError} from '@apollo/client/errors';


export async function AuthSafeQuery<T = unknown, TVariables extends OperationVariables = OperationVariables>(options: QueryOptions<TVariables, T>) {
  try {
    return await (await getClient()).query(options);
  } catch (error: unknown) {
    // Check if error has networkError property and it's a 401
    if (hasNetworkError(error) && isServerError(error.networkError) && error.networkError.statusCode === 401)
      redirect("/api/logout")

    throw error;
  }
}


function hasNetworkError(error: unknown): error is { networkError: Error | undefined } {
  return typeof error === 'object' && error !== null && 'networkError' in error;
}

function isServerError(networkError: Error | undefined): networkError is ServerError{
  return (networkError as ServerError | undefined)?.statusCode !== undefined;
}