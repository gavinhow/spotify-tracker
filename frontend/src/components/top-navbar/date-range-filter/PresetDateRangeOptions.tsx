import { RANGE_OPTION } from '@/lib/GlobalFilters/GlobalFilters';
import { cn } from '@/lib/utils';

interface PresetDateRangeOptionsProps {
  selected: RANGE_OPTION;
  onClick?: (option: RANGE_OPTION) => void
}

const PresetDateRangeOptions = ({selected, onClick }: PresetDateRangeOptionsProps) => {
  const handleSelection = (option: RANGE_OPTION) => {
    onClick?.(option); // Trigger the onClick event with the calculated range
  };

  return <div className="grid gap-2">
    {Object.values(RANGE_OPTION).map((option) => (
      <div onClick={() => handleSelection(option)} key={option}
           className={cn('flex items-center p-2 px-4 hover:bg-green-600/15 rounded-sm', selected === option ? 'bg-green-600/15' :'')}>
        {option}
      </div>
    ))}
  </div>
}


export default PresetDateRangeOptions;