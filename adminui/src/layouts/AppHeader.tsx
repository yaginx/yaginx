import { IAuthContext, useAuth } from './RootAuthProvider';
import { Avatar, Dropdown, Layout, Space } from 'antd';
import React, { useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import logo from '../assets/logo.jpg';
import { AudioOutlined, LogoutOutlined, SearchOutlined, UserOutlined } from '@ant-design/icons';
import { VITE_APP_TITLE } from '@c/constant';
import type { MenuProps } from 'antd';
import Search from 'antd/lib/input/Search';


const { Header } = Layout;

const AppHeader: React.FC = () => {
  const location = useLocation();

  const { authInfo, logout }: IAuthContext = useAuth();



  // 设置Title
  useEffect(
    () => {
      document.title = location.pathname;
    },
    [location],
  )


  const handleButtonClick = () => {
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

  const suffix = (
    <AudioOutlined
      style={{
        fontSize: 16,
        color: '#1677ff',
      }}
    />
  );

  const onSearch = (value: string) => console.log(value);

  return (
    <Header className={"layout_header"} style={{ userSelect: "none", height: "45px", lineHeight: "45px", paddingInline: "12px" }}>
      <div className={"layout_header_left"}>
        <Space>
          <Link to={"/"}><div className='logo' style={{ color: "orange", fontSize: "26px" }}>{VITE_APP_TITLE}</div></Link>
          <span style={{ display: "inline-block", width: "3px", backgroundColor: "#ccc", height: "25px", borderRadius: "3px", verticalAlign: "middle" }}></span>
          {/* <NavMenu /> */}
          <Search
            placeholder="input search text"
            enterButton={<SearchOutlined />}
            allowClear
            size="middle"
            suffix={suffix}
            onSearch={onSearch}
            // bordered={false}
            style={{ width: 304, verticalAlign: "middle", marginLeft: "10px" }}
          />
        </Space>
      </div>
      <Space className="layout_header_right" direction="horizontal" size="middle">
        {/* <AsyncTaskWidget /> */}
        <Dropdown.Button className='user_info' menu={menuProps} icon={<Avatar shape="square" src={logo} size={24} />} onClick={handleButtonClick}>
          <span className='user_name'> Hi {authInfo.name}</span>
        </Dropdown.Button>
      </Space>
    </Header >
  )
};
export default AppHeader;



