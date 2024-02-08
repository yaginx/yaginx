import { Layout, Menu, MenuProps } from 'antd';
import React, { Suspense, useEffect, useState } from 'react';
import { Link, Navigate, Outlet, useLocation, useNavigate } from 'react-router-dom';
import { IAuthContext, useAuth } from './RootAuthProvider';
import { ErrorBoundary } from 'react-error-boundary';
import AppFooter from './AppFooter';
import AppHeader from './AppHeader';
import Loading from '@/componets/Loading';
import NoFound from './NoFond';
import Sider from 'antd/es/layout/Sider';
import { UserOutlined, VideoCameraOutlined, UploadOutlined, BarChartOutlined, CloudOutlined, AppstoreOutlined, TeamOutlined, ShopOutlined } from '@ant-design/icons';
import SideBottomMenu from './SideBottomMenu';
import { IMenuLink, menus } from '@/router/menu';
import MenuIcon from '@/MenuIcon';
export const ErrorFallback = ({ error, resetErrorBoundary }: any) => {
  return (
    <div role="alert">
      <p>Something went wrong:</p>
      <pre>{error.message}</pre>
      <button onClick={resetErrorBoundary}>Try again</button>
    </div>
  )
}
const { Content } = Layout;
const UserLayout: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { isAuthorized }: IAuthContext = useAuth();

  useEffect(() => {
    //console.log("Authorized Status Changed:" + isAuthorized);
    if (!isAuthorized) {
      navigate("/login", { replace: true })
    }
  }, [isAuthorized])

  // 设置Title
  useEffect(() => { document.title = location.pathname; }, [location],)
  const items: MenuProps['items'] = [
    UserOutlined,
    VideoCameraOutlined,
    UploadOutlined,
    BarChartOutlined,
    CloudOutlined,
    AppstoreOutlined,
    TeamOutlined,
    ShopOutlined,
    UserOutlined,
    VideoCameraOutlined,
    UploadOutlined,
    BarChartOutlined,
    CloudOutlined,
    AppstoreOutlined,
    TeamOutlined,
    ShopOutlined,
    UserOutlined,
    VideoCameraOutlined,
    UploadOutlined,
    BarChartOutlined,
    CloudOutlined,
    AppstoreOutlined,
    TeamOutlined,
    ShopOutlined,
  ].map((icon, index) => ({
    key: String(index + 1),
    icon: React.createElement(icon),
    label: `nav ${index + 1}`,
  }));
  const topNavMenu = (data: Array<IMenuLink> | undefined) => data?.map(item => renderMenuItem(item, true, "/"));
  const renderMenuItem: any = (item: IMenuLink, isRenderChild: boolean = true, parentPath: string = "") => {
    let targetPath = item.path.startsWith('/') ? item.path : `${parentPath}/${item.path}`
    return {
      key: item.path, label: <Link to={targetPath}><span>{item.title}</span></Link>, icon: item.icon
        ? <MenuIcon name={item.icon} />
        : <></>,
      children: isRenderChild && item.children
        ? item.children?.map(c => renderMenuItem(c, isRenderChild, item.path))
        : null
    };
  }
  const layout = () => {
    return <Layout style={{ height: '100vh', width: '100wh' }} hasSider>
      <Sider style={{ overflow: 'auto', height: '100vh', position: 'fixed', left: 0, top: 0, bottom: 0 }}>
        <div className="logo" >
          YAGINX
        </div>
        <Menu className='sider-menu' theme="dark" mode="inline" defaultSelectedKeys={['4']} items={topNavMenu(menus)} style={{ height: '80vh', overflow: "auto" }} />
        <SideBottomMenu theme="dark"></SideBottomMenu>
      </Sider>
      <Layout className='main-layout' style={{ marginLeft: 200, height: '100vh', width: '100wh', overflow: "auto" }}>
        <Content style={{ margin: '24px 16px 0' }}>
          <ErrorBoundary FallbackComponent={ErrorFallback} onReset={() => { }}>
            <Suspense fallback={<Loading />}><Outlet /></Suspense>
          </ErrorBoundary>
        </Content>
      </Layout>
    </Layout>
  }
  return isAuthorized ? self === top ? layout() : <NoFound /> : <Navigate to={"/login"} replace={true} />
};
export default UserLayout;



