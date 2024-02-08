import React from 'react';
import Redirect from '@/pages/Redirect';
export interface IRouteConfig {
  key?: string;
  // 路由路径, 必要时会带上*等规则匹配模式
  routePath?: string;
  // 导航路径, 浏览器地址栏显示的地址
  navPath?: string;
  // 路由组件
  element?: any;
  // 302 跳转到其他地方
  redirect?: string;
  // 仅出现在Route, 不在Nav中出现
  isHide?: boolean;

  // 不是一个Link, 只做显示
  notLink?: boolean;
  // 路由信息
  title?: string;
  icon?: string;
  // 是否校验权限, false 为不校验, 不存在该属性或者为true 为校验, 子路由会继承父路由的 auth 属性
  auth?: boolean;
  // 菜单权限
  permission?: string;
  children?: IRouteConfig[];
}

const layouts: IRouteConfig[] = [
  {
    routePath: '/',
    title: '/',
    isHide: true,
    element: React.lazy(async () => await import('@/layouts/RootLayout')),
    children: [
      {
        routePath: '/login',
        title: '登录',
        isHide: true,
        element: React.lazy(async () => await import('@/pages/Login')),
      },
      {
        // routePath: '*',
        // title: '授权页面模板',
        // isHide: true,
        element: React.lazy(async () => await import('@/layouts/UserLayout')),
        children: [
          {
            routePath: '/',
            // title: '仪表盘',
            // icon: 'BankOutlined',
            element: React.lazy(async () => await import('@/pages/Home')),
          },
          {
            routePath: 'website/*',
            title: '网站列表',
            icon: 'BankOutlined',
            element: React.lazy(async () => await import('@/pages/Websites')),
          },
          {
            routePath: 'domain/*',
            // title: '网站列表',
            // icon: 'BankOutlined',
            element: React.lazy(async () => await import('@/pages/WebDomains')),
          },
          {
            routePath: 'monitor/*',
            // title: '网站列表',
            // icon: 'BankOutlined',
            element: React.lazy(async () => await import('@/pages/Monitors')),
          },
          {
            routePath: 'docker/*',
            // navPath: "docker",
            // title: '容器管理',
            icon: 'BankOutlined',
            // redirect: "docker/container",
            element: React.lazy(async () => await import('@/pages/Dcokers')),
          },
          {
            routePath: 'system/*',
            // navPath: "users",
            // title: '用户管理',
            // icon: 'BankOutlined',
            element: React.lazy(async () => await import('@/pages/Home')),
          }
        ]
      },

    ]
  }
];

export default layouts;
