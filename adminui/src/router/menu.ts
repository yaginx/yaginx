export interface IMenuLink {
  path: string;
  title: string;
  icon?: string;
  permission?: string;
  children?: IMenuLink[];
}

export const menus: IMenuLink[] = [
  {
    path: "/",
    title: "仪表盘",
    icon: "HomeOutlined",
  },
  {
    path: "/website",
    title: "网站",
    icon: "TeamOutlined",
    permission: "tenants",
  },
  {
    path: "/domain",
    title: "域名",
    icon: "DeploymentUnitOutlined",
  },
  {
    path: "/monitor/websites",
    title: "网站监控",
    icon: "DeploymentUnitOutlined",
  },
  {
    path: "/monitor/system",
    title: "系统监控",
    icon: "TeamOutlined",
    permission: "tenants",
  },
  {
    path: "/docker",
    title: "容器管理",
    icon: "SettingOutlined",
    permission: "tenants",
    children: [
      {
        path: "container/list",
        title: "容器列表",
        icon: "ProfileOutlined",
        permission: "tenants",
      }
    ],
  },

  {
    path: "/systems",
    title: "系统管理",
    icon: "SettingOutlined",
    permission: "tenants",
    children: [
      {
        path: "/systems/users",
        title: "用户管理",
        icon: "ProfileOutlined",
        permission: "tenants",
      },
      {
        path: "/systems/setting",
        title: "系统设置",
        icon: "ProfileOutlined",
        permission: "tenants",
      }
    ],
  },
];
