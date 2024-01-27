import MenuIcon from '@/MenuIcon';
import { Avatar, Dropdown, Layout, Menu, Progress, Space } from 'antd';
import React, { Suspense, useEffect, useState } from 'react';
import { Link, Navigate, Outlet, useLocation, useNavigate } from 'react-router-dom';
import routes, { IRouteConfig } from "../router/config";
import logo from '../assets/logo.jpg';
import { LogoutOutlined, MenuFoldOutlined, MenuUnfoldOutlined, UserOutlined } from '@ant-design/icons';
import { VITE_APP_TITLE } from '@c/constant';
import SiteSelector from '@/componets/SiteSelector';
import { useAuth } from './AuthProvider';
import Loading from '@/componets/Loading';
import type { MenuProps } from 'antd';

const { Header, Content, Footer, Sider } = Layout;
const UserLayout: React.FC = () => {
    const navigate = useNavigate();
    const topNavMenu = (data: Array<IRouteConfig>) => renderChildMenu(data.filter(item => !item.isHide), false);
    const location = useLocation();
    const [collapsed, setCollapsed] = useState(false);

    const { authInfo, token, logout }: any = useAuth();

    // 如果Token有值，则认为已登录
    if (!token) {
        return <Navigate to="/system/login" />;
    }

    const renderChildMenu: any = (data: Array<IRouteConfig>, isRenderChild: boolean = true) => {
        return data?.filter(item => !item.isHide).map(item => renderMenuItem(item, isRenderChild));
    }

    const renderMenuItem: any = (item: IRouteConfig, isRenderChild: boolean = true) => {
        let redirectPath = item.redirect != null ? item.redirect : item.path;
        let label = (<Link to={redirectPath} onClick={() => navigate(redirectPath)}><span>{item.title}</span></Link>)
        return { key: item.path, label: label, icon: item.icon ? <MenuIcon name={item.icon} /> : <></>, children: isRenderChild && item.children ? renderChildMenu(item.children) : null };
    }

    const sideNavMenu = (data: Array<IRouteConfig>) => {
        var firstSegmentIndex = location.pathname.indexOf("/", 1);
        let rootPath = location.pathname.substring(0, firstSegmentIndex == -1 ? undefined : firstSegmentIndex);
        return renderChildMenu(data.filter(item => !item.isHide && item.path.startsWith(rootPath))[0].children);//data.filter(item => !item.isHide ).map(mainMenu => renderChildMenu(mainMenu.children));
    };

    const SideCollapseSwitch = () => React.createElement(collapsed ? MenuUnfoldOutlined : MenuFoldOutlined);
    const footerMenuItems = [
        { key: "sideCollapse", title: "侧边栏", icon: <SideCollapseSwitch />, onClick: () => setCollapsed(!collapsed) }
    ];

    // 设置Title
    useEffect(
        () => {
            document.title = location.pathname;
        },
        [location],
    )
    const handleButtonClick = (e: React.MouseEvent<HTMLButtonElement>) => {
        console.log('click left button');
    };
    const handleMenuClick: MenuProps['onClick'] = (e) => {
        console.log('click', e.key);
        switch (e.key) {
            case "userInfo":
                break;
            case "logout":
                logout();
                break;
            default:
                break;
        }
    };
    const menuProps = {
        items: [
            { key: "userInfo", label: "个人信息", icon: <UserOutlined /> },
            { key: "logout", label: "注销登录", icon: <LogoutOutlined /> }
        ],
        onClick: handleMenuClick,
    };
    return (
        <Layout>
            <Header className={"layout_header"} style={{ userSelect: "none", height: "45px", lineHeight: "45px" }}>
                <div className={"layout_header_left"}>
                    <div className='logo'>{VITE_APP_TITLE}</div>
                    <Menu theme="dark" className='topNavMenu' mode="horizontal" items={topNavMenu(routes)} />
                </div>
                <Space className="layout_header_right" direction="horizontal" size="middle">
                    <div className='project_selector'>
                        <span >项目:</span>
                        <SiteSelector />
                    </div>
                    <Dropdown.Button className='user_info' menu={menuProps} icon={<Avatar shape="square" src={logo} size={24} />} onClick={handleButtonClick}>
                        <span className='user_name'> Hi {authInfo.name}</span>
                    </Dropdown.Button>
                </Space>
            </Header >
            <Layout className={"layout_middle"}>
                <Sider width={200} className="site-layout-background" collapsed={collapsed} style={{ userSelect: "none" }}>
                    <Menu mode="inline" style={{ height: '100%', borderRight: 0 }} items={sideNavMenu(routes)} />
                </Sider>
                <Layout style={{ padding: '0 0 0 24px' }}>
                    <Content
                        className="site-layout-background"
                        style={{
                            padding: 18,
                            margin: 0,
                            minHeight: 600,
                            overflow: 'auto'
                        }}>
                        <Suspense fallback={<Loading />}>
                            <Outlet />
                        </Suspense>
                    </Content>
                </Layout>
            </Layout>
            <Footer className={"layout_footer"} style={{ userSelect: "none", padding: "0 10px", lineHeight: "35px" }}>
                <div className='footer_left'>
                    <div className='function_button_list'>
                        <Space>
                            {footerMenuItems.map(item => <div key={item.key} onClick={item.onClick}>{item.icon}</div>)}
                        </Space>
                    </div>
                    <div className='progress_bar'>
                        <Space>
                            <span>Progress:</span>
                            <Progress strokeLinecap="butt" style={{ userSelect: "none" }} percent={75} />
                        </Space>
                    </div>
                </div>
                <div className='footer_middle'>
                    <a href='https://feinian.net' target={"_blank"}>Feinian Studio</a> <span> ©2023 based on </span> <a href='https://ant-design.antgroup.com/components/overview-cn/' target={"_blank"}>Ant Design</a>
                </div>
                {/* <div className='footer_right'>
                </div> */}
            </Footer>
        </Layout >
    )
};
export default UserLayout;

