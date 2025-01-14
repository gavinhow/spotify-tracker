'use client'

import React, { useCallback } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import Error from 'next/error';
import { RANGE_OPTION } from '@/lib/GlobalFilters/GlobalFilters';

interface GlobalFilterContextValue {
  user?: string;
  setUser: (selectedHubId?: string) => void;
  range: RANGE_OPTION;
  from?: Date;
  to?: Date;
  setDateRange: (range: RANGE_OPTION, from: Date | undefined, to: Date | undefined) => void;
  toURLSearchParams: () => URLSearchParams;
}

const GlobalFilterContext = React.createContext<GlobalFilterContextValue>({
  user: undefined,
  setUser: () => {
  },
  range: RANGE_OPTION.All_time,
  from: undefined,
  to: undefined,
  setDateRange: () => {},
  toURLSearchParams: () => {
    return new URLSearchParams();
  },
})

interface GlobalFilterProviderProps {
  children: React.ReactNode
}

export const useGlobalFilter = () => {
  const value = React.useContext(GlobalFilterContext)
  if (!value) {
    throw new Error({ title: 'useGlobalFilter must be used within a GlobalFilterProvider', statusCode: 500 });
  }
  return value;
}

export const GlobalFilterProvider = ({ children }: GlobalFilterProviderProps) => {
  const searchParams = useSearchParams()
  const pathname = usePathname()
  const router = useRouter();

  // Get a new searchParams string by merging the current
  // searchParams with a provided key/value pair
  const createQueryString = useCallback(
    (newParams: { name: string, value: string | undefined }[]) => {
      const params = new URLSearchParams(searchParams.toString())

      newParams.forEach(({ name, value }) => {
        if (value === undefined)
          params.delete(name);
        else
          params.set(name, value);
      });

      return params.toString()
    },
    [searchParams]
  )

  const currentUserValue = searchParams.get('user') ?? undefined
  const currentRangeValue = searchParams.get('range') ?? undefined
  const currentFromValue = searchParams.get('from') ?? undefined
  const currentToValue = searchParams.get('to') ?? undefined

  return (
    <GlobalFilterContext.Provider value={{
      user: currentUserValue,
      range: currentRangeValue as RANGE_OPTION ?? RANGE_OPTION.All_time,
      from: currentFromValue ? new Date(currentFromValue) : undefined,
      to: currentToValue ? new Date(currentToValue) : undefined,
      setUser: (id) => {
        if (id === currentUserValue)
          return
        if (id)
          router.push(pathname + '?' + createQueryString([{
            name: 'user', value: id
          }]))
        else
          router.push(pathname)
      },
      setDateRange: (range: RANGE_OPTION, from: Date | undefined, to: Date | undefined) => {
        router.push(pathname + '?' + createQueryString([
          ...(range ? [{ name: 'range', value: range }] : []),
          { name: 'from', value: from?.toString() },
          { name: 'to', value: to?.toString() }
        ]))
      },
      toURLSearchParams: () => {
        return new URLSearchParams({
          ...(currentFromValue ? { 'from': currentFromValue }: {}),
          ...(currentToValue ? { 'to': currentToValue }: {}),
          ...(currentRangeValue ? { 'range': currentRangeValue }: {}),
          ...(currentUserValue ? { 'user': currentUserValue }: {}),
        })
      }
    }}>
      {children}
    </GlobalFilterContext.Provider>
  );
};