import { NextRequest, NextResponse } from 'next/server';

export function middleware(req: NextRequest) {
  // Check if the path starts with /demo
  if (req.nextUrl.pathname.startsWith('/demo')) {
    const url = req.nextUrl.clone();

    // Remove '/demo' prefix to match normal routes
    url.pathname = url.pathname.replace('/demo', '');

    // Forward the modified request
    const response = NextResponse.rewrite(url);
    response.headers.set('IS_DEMO', 'true');
    return response;
  }

  return NextResponse.next({
    headers: {
      IS_DEMO: 'false'
    }
  });
}
