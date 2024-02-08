// pages/Home/index.tsx
import React, { useState } from 'react';
import { Navigate, Outlet, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import List from './List';
import { Create } from './Create';
import { Edit } from './Edit';
// import TenantList from './TenantList';
// import TenantEdit from './TenantEdit';
// import TenantCreate from './TenantCreate';
// import TenantView from './TenantView';

const WebDomainIndex: React.FC = () => {
  return (
    <>
      <Routes>
      <Route index element={<Navigate to="list" />} />
        <Route path="list" element={<List />} />
        <Route path="create" element={<Create />} />
        <Route path="edit/:id" element={<Edit />} />
        {/* <Route path="list" element={<WebsiteList />} /> */}
        {/* <Route path="create" element={<TenantCreate />} />

        <Route path="detail/:id" element={<TenantView />} /> */}
      </Routes>
      <Outlet />
    </>
  )
}

export default WebDomainIndex;
