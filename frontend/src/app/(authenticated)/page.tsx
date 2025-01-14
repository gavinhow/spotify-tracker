import TopArtistCard from '@/components/dashboard/top-artist-card';
import TopSongsCard from '@/components/dashboard/top-songs-card';
import TopAlbumsCard from '@/components/dashboard/top-albums-card';
import { GlobalFiltersSearchParams } from '@/lib/GlobalFilters/GlobalFilters';
import { getGlobalFiltersOrDefault } from '@/lib/GlobalFilters/getGlobalFiltersOrDefault';


const Home = async ({ searchParams }: { searchParams?: Promise<GlobalFiltersSearchParams> }) => {
  const globalFilters = await getGlobalFiltersOrDefault(await searchParams);
  if (!globalFilters) {
    return null;
  }
  return (
    <div className="m-4">
      <div
        className="flex flex-col min-h-screen gap-6 font-[family-name:var(--font-geist-sans)]">
        <TopArtistCard {...globalFilters} />
        <TopSongsCard {...globalFilters} />
        <TopAlbumsCard {...globalFilters} />
      </div>
    </div>
  );
};

export default Home;