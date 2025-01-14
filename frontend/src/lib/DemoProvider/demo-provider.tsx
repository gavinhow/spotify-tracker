"use client"
import React from 'react';
import DemoProviderClient from '@/lib/DemoProvider/demo-provider-client';


const DemoProvider = ({ children, isDemo }: { children: React.ReactNode, isDemo: boolean }) => {
  return (
    <DemoProviderClient isDemo={isDemo}>
      {children}
    </DemoProviderClient>
  );
};

export default DemoProvider;
