// pages/Home/index.tsx
import React, { useState } from 'react';
import { Navigate, Outlet, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import ContainerList from './ContainerList';
// import TenantList from './TenantList';
// import TenantEdit from './TenantEdit';
// import TenantCreate from './TenantCreate';
// import TenantView from './TenantView';

const ContainerIndex: React.FC = () => {
  return (
    <>
      <Routes>
        <Route index element={<Navigate to={"./list"} />} />
        <Route path="list" element={<React.Suspense fallback={<>...</>}><ContainerList /></React.Suspense>} />
        {/* <Route path="create" element={<TenantCreate />} />
        <Route path="edit/:id" element={<TenantEdit />} />
        <Route path="detail/:id" element={<TenantView />} /> */}
      </Routes>
      <Outlet />
    </>
  )
}

export default ContainerIndex;
