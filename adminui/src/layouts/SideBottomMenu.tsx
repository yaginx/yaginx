import React from 'react';
import { Menu } from 'antd';
import MenuIcon from '@/MenuIcon';
import { useNavigate } from 'react-router';
const SideBottomMenu: React.FC<any> = (props: any) => {
  const history = useNavigate();
  function handleClick(e: any) {
    console.log(e);
    if (!e.key) {
      return;
    }
    switch (e.key) {
      case 'logout':
        console.log("prepare logout")
        // appStore.logout();
        history("/");
        console.log("appstore logouted")
        break;
    }
  }

  const menuItems = [
    {
      key: "logout",
      label: <span>Logout</span>,
      icon: <MenuIcon name="LogoutOutlined" />
    }
  ]
  //    <Menu.Item key="logout" icon={<MenuIcon name="LogoutOutlined" />}> Logout</Menu.Item>
  return (
    <Menu
      {...props}
      onClick={handleClick}
      mode="inline"
      className="side-bottom-menu" style={{ position: "absolute", bottom: 0 }} items={menuItems} />
  );
};
export default SideBottomMenu;
