// pages/Home/index.tsx
import React from 'react';
import { Navigate, Outlet, Route, Routes } from 'react-router-dom';
import ContainerIndex from './Containers';
import DockerDashboard from './DockerDashboard';
// import TenantList from './TenantList';
// import TenantEdit from './TenantEdit';
// import TenantCreate from './TenantCreate';
// import TenantView from './TenantView';

const TenantIndex: React.FC = () => {
  return (
    <>
      <Routes>
        {/* <Route index element={<Navigate to={"./list"} />} /> */}
        <Route index element={<DockerDashboard />} />
        <Route path="container/*" element={<React.Suspense fallback={<>...</>}><ContainerIndex /></React.Suspense>} />
        {/* <Route path="create" element={<TenantCreate />} />
        <Route path="edit/:id" element={<TenantEdit />} />
        <Route path="detail/:id" element={<TenantView />} /> */}
      </Routes>
      <Outlet />
    </>
  )
}

export default TenantIndex;
