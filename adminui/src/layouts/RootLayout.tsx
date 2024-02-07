import React, { Suspense } from 'react';
import { Outlet } from 'react-router-dom';
import Loading from '../componets/Loading';
const RootLayout: React.FC = () => {
  return <Suspense fallback={<Loading />}><Outlet /></Suspense>
};
export default RootLayout;



