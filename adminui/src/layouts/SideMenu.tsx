import React from "react";
import { Menu } from "antd";
import routes, { IRouteConfig } from "../router/config";
import MenuIcon from "@/MenuIcon";
import { Link, useNavigate } from "react-router-dom";
const { SubMenu } = Menu;

// const SideMenu = () => {
//     const permissions = new Array<string>();
//     const navigate = useNavigate();

//     // 递归渲染菜单
//     const renderMenu = (data: Array<IRouteConfig>) => {
//         return data.map(item => {
//             // if (item.permission && !permissions.includes(item.permission)) {
//             //     return;
//             // }

//             // if (item.routes) {
//             //     return (
//             //         <SubMenu key={item.path} icon={<MenuIcon name={item.icon} />} title={<span>{item.title}</span>}>
//             //             {renderMenu(item.routes)}
//             //         </SubMenu>
//             //     );
//             // }
//             return (
//                 <Menu.Item key={item.path} icon={<MenuIcon name={item.icon} />}>
//                     <Link to={item.path} onClick={() => navigate(item.path)}>
//                         <span>{item.title}</span>
//                     </Link>
//                 </Menu.Item>
//             );
//         });
//     };
//     return (
//         <Menu
//             defaultSelectedKeys={['/']}
//             defaultOpenKeys={['/cms', '/system']}
//             mode="inline"
//         >
//             {renderMenu(routes)}
//         </Menu>
//     )

// }
const SideMenu = (props: any) => {
    const permissions = new Array<string>();
    const navigate = useNavigate();

    // 递归渲染菜单
    const renderMenu = (data: Array<IRouteConfig>) => {
        return data.map(item => {
            // if (item.permission && !permissions.includes(item.permission)) {
            //     return;
            // }

            // if (item.routes) {
            //     return (
            //         <SubMenu key={item.path} icon={<MenuIcon name={item.icon} />} title={<span>{item.title}</span>}>
            //             {renderMenu(item.routes)}
            //         </SubMenu>
            //     );
            // }
            return (
                <Menu.Item key={item.path} icon={<MenuIcon name={item.icon} />}>
                    <Link to={item.path} onClick={() => navigate(item.path)}>
                        <span>{item.title}</span>
                    </Link>
                </Menu.Item>
            );
        });
    };
    return (
        <Menu
            // defaultSelectedKeys={['/']}
            // defaultOpenKeys={['/cms', '/system']}
            mode="inline">
            {/* {renderMenu(routes)} */}
        </Menu>
    )
}

export default SideMenu;