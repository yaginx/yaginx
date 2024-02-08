// pages/Home/index.tsx
import React, { Suspense } from 'react';
import { Outlet, Route, Routes } from 'react-router-dom';
import HostTrafficIndex from './HostTraffics';
// import TenantList from './TenantList';
// import TenantEdit from './TenantEdit';
// import TenantCreate from './TenantCreate';
// import TenantView from './TenantView';

const MonitorIndex: React.FC = () => {
  return (
    <>
      <Routes>
        <Route path="hostTraffic/*" element={<Suspense><HostTrafficIndex /></Suspense>} />
      </Routes>
      <Outlet />
    </>
  )
}

export default MonitorIndex;
