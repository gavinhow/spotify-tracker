"use client"
import React from 'react';
import Link, { LinkProps } from 'next/link';
import { useGlobalFilter } from '@/components/top-navbar/GlobalFilterProvider';
import { useDemo } from '@/lib/DemoProvider/demo-provider-client';

interface LinkWithQueryParamProps extends Omit<React.AnchorHTMLAttributes<HTMLAnchorElement>, keyof LinkProps>, LinkProps {

}

const LinkWithGlobalFilters = ({href, ...props}: LinkWithQueryParamProps) => {
  const { toURLSearchParams } = useGlobalFilter();
  const { isDemo } = useDemo();

  const createFinalUrl = (pathname: string, search: string) => {
    return `${isDemo ? "/demo" : ""}${pathname}${search}`;
  }

  if (typeof href === 'string') {
    const temporaryBase = "https://gavinhow.com"
    const url = new URL(href, temporaryBase);
    const paramsFromGlobalFilter = Object.fromEntries(toURLSearchParams());
    const paramsFromHref = Object.fromEntries(url.searchParams);
    const mergedParams = { ...paramsFromGlobalFilter, ...paramsFromHref };
    url.search = new URLSearchParams(mergedParams).toString();

    const finalUrl = href.startsWith("/") ? createFinalUrl(url.pathname, url.search) : url.toString();
    return <Link href={finalUrl.toString()} {...props} />
  }
  else throw new Error("LinkWithGlobalFilters does not support href of type UrlObject. Use href of type string instead.")

};

export default LinkWithGlobalFilters;
