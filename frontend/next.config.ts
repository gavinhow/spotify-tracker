import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  /* config options here */
  images: {
    remotePatterns: [
      {
        hostname: 'i.scdn.co'
      }
    ]
  },
  logging: {
    fetches: {
      fullUrl: true,
    },
  },

};

export default nextConfig;
