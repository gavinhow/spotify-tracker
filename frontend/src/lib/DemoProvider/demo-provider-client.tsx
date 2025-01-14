import React from 'react';
import Error from 'next/error';

const DemoContext = React.createContext<{isDemo: boolean}>({
  isDemo: false,
});

export const useDemo = () => {
  const value = React.useContext(DemoContext)
  if (!value) {
    throw new Error({ title: 'useDemo must be used within a DemoProvider', statusCode: 500 });
  }
  return value;
}

interface DemoProviderClientProps {
  isDemo?: boolean;
  children: React.ReactNode;
}

const DemoProviderClient = ({isDemo, children}: DemoProviderClientProps) => {
  return (
    <DemoContext.Provider value={{
      isDemo: isDemo === undefined ? false : isDemo,
    }}>
      {children}
    </DemoContext.Provider>
  );
};

export default DemoProviderClient;
