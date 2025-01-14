'use client'

import * as React from 'react'
import { format } from 'date-fns'
import { Calendar as CalendarIcon } from 'lucide-react'

import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import { Popover, PopoverContent, PopoverTrigger, } from '@/components/ui/popover'
import PresetDateRangeOptions from '@/components/top-navbar/date-range-filter/PresetDateRangeOptions';
import { useGlobalFilter } from '@/components/top-navbar/GlobalFilterProvider';
import { Calendar } from '@/components/ui/calendar'
import { RANGE_OPTION } from '@/lib/GlobalFilters/GlobalFilters';

export function DateRangeFilter({
                                  className,
                                }: React.HTMLAttributes<HTMLDivElement>) {
  const [isOpen, setIsOpen] = React.useState(false)
  const { range, from, to, setDateRange } = useGlobalFilter()

  return (
    <div className={cn('flex-1 grid gap-2', className)}>
      <Popover open={isOpen} onOpenChange={setIsOpen}>
        <PopoverTrigger asChild>
          <Button
            id="date"
            variant={'outline'}
            className={cn(
              ' justify-start text-left font-normal',
              !range && 'text-muted-foreground'
            )}
          >
            <CalendarIcon/>
            {range ? (
              range === RANGE_OPTION.Custom && from && to ? <>
                  {format(from, "LLL dd, y")} -{" "}
                  {format(to, "LLL dd, y")}
                </>:
              <span>{range}</span>
            ) : (
              <span>Pick a range</span>
            )}
          </Button>
        </PopoverTrigger>
        <PopoverContent className="" align="start">
          <PresetDateRangeOptions
            selected={range}
            onClick={(option) => {
              setDateRange(option, undefined, undefined)
              if (option !== RANGE_OPTION.Custom)
                setIsOpen(false)
            }}/>
          <div>

          </div>
          <div className={`mt-4 ${range === RANGE_OPTION.Custom ? '' : 'hidden'}`}>
            <Popover>
              <PopoverTrigger asChild>
                <div className=" px-4 p-2 flex space-x-4 items-center">
                  <label>
                    From
                  </label>
                  <Button
                    id="date"
                    variant={'outline'}
                    className={cn(
                      ' justify-start text-left font-normal flex-1',
                      !from && 'text-muted-foreground'
                    )}
                  >
                    <CalendarIcon/>
                    {from ?
                      format(from, 'LLL dd, y')
                      : (
                        <span>Pick a date</span>
                      )}
                  </Button>
                </div>
              </PopoverTrigger>
              <PopoverContent className="w-auto">
                <Calendar
                  mode="single"
                  defaultMonth={from}
                  captionLayout={'dropdown'}
                  startMonth={new Date(2020, 0)}
                  endMonth={new Date()}
                  selected={from}
                  onSelect={(date) => {
                    if (date) setDateRange(RANGE_OPTION.Custom, date, to)
                  }}
                />
              </PopoverContent>
            </Popover>
            <Popover>
              <PopoverTrigger asChild>
                <div className=" px-4 p-2 flex space-x-4 items-center">
                  <label>
                    To
                  </label>
                  <Button
                    id="date"
                    variant={'outline'}
                    className={cn(
                      ' justify-start text-left font-normal flex-1',
                      !to && 'text-muted-foreground'
                    )}
                  >
                    <CalendarIcon/>
                    {to ?
                      format(to, 'LLL dd, y')
                      : (
                        <span>Pick a date</span>
                      )}
                  </Button>
                </div>
              </PopoverTrigger>
              <PopoverContent className="w-auto">
                <Calendar
                  mode="single"
                  defaultMonth={to}
                  captionLayout={'dropdown'}
                  startMonth={new Date(2020, 0)}
                  endMonth={new Date()}
                  selected={to}
                  onSelect={(date) => {
                    if (date) setDateRange(RANGE_OPTION.Custom, from, date)
                  }}
                />
              </PopoverContent>
            </Popover>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  )
}

export default DateRangeFilter;
