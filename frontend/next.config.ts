import type {NextConfig} from 'next';

const nextConfig: NextConfig = {
  /* config options here */
  images: {
    remotePatterns: [
      {
        hostname: 'i.scdn.co'
      }
    ]
  },
  serverExternalPackages: ["pino", "pino-pretty"]
};

export default nextConfig;
