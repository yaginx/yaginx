// App.tsx
import React, { Suspense } from 'react';
import { BrowserRouter, useRoutes } from 'react-router-dom';
import { VITE_BASE_PATH } from '@c/constant';
import routes, { IRouteConfig } from "./router/config";
import { AuthProvider } from './layouts/AuthProvider';
import Loading from './componets/Loading';

const App: React.FC = () => {
  const buildRouteData: any = (routeList?: IRouteConfig[]) => {
    if (routeList == null || routeList.length <= 0)
      return [];
    return routeList.filter((route, i) => route.element !== undefined).map((route, i) => ({ path: route.path, children: buildRouteData(route.children), element: <route.element /> }));
  }
  const RouteElement = () => useRoutes(buildRouteData(routes));
  return (
    <Suspense fallback={<Loading />}>
      <BrowserRouter basename={VITE_BASE_PATH}>
        <AuthProvider>
          <RouteElement />
        </AuthProvider>
      </BrowserRouter>
    </Suspense>
  );
};

export default App;
