// pages/Home/index.tsx
import React, { useState } from 'react';
import { Navigate, Outlet, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import List from './List';
// import TenantList from './TenantList';
// import TenantEdit from './TenantEdit';
// import TenantCreate from './TenantCreate';
// import TenantView from './TenantView';

const HostTrafficIndex: React.FC = () => {
  return (
    <>
      <Routes>
      <Route index element={<Navigate to="list" />} />
        <Route path="list" element={<List />} />
      </Routes>
      <Outlet />
    </>
  )
}

export default HostTrafficIndex;
