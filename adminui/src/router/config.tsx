import React from 'react';
import Redirect from '@/pages/Redirect';
export interface IRouteConfig {
  // 路由路径
  path: string;
  // 路由组件
  element?: any;
  // 302 跳转
  redirect?: string;
  isHide?: boolean;
  // 路由信息
  title: string;
  icon?: string;
  // 是否校验权限, false 为不校验, 不存在该属性或者为true 为校验, 子路由会继承父路由的 auth 属性
  auth?: boolean;
  // 菜单权限
  permission?: string;
  children?: IRouteConfig[];
}

const layouts: IRouteConfig[] = [
  {
    path: '/',
    title: '/',
    isHide: true,
    element: Redirect,
  },
  {
    path: '/system',
    element: React.lazy(async () => await import('@/layouts/BasicLayout')),
    title: '用户路由',
    redirect: '/system/access-denied',
    isHide: true,
    children: [
      {
        path: 'login',
        element: React.lazy(async () => await import('@/pages/User/Login')),
        title: '登录',
      },
      {
        path: 'register',
        element: React.lazy(async () => await import('@/pages/User/Register')),
        title: '注册',
      },
      {
        path: 'access-denied',
        element: React.lazy(async () => await import('@/pages/System/AccessDenied')),
        title: 'Access Denied',
      },
      {
        path: 'noFond',
        title: '页面不存在',
        isHide: true,
        element: React.lazy(async () => import('@/pages/System/NoFond')),
      },
    ],
  },{
    path: '/home',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: '仪表盘',
    redirect: '/home/dashboard',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'dashboard',
        element: React.lazy(async () => await import('@/pages/Home')),
        title: '仪表盘',
      }
    ]
  },
  {
    path: '/reverse-proxies',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: '反向代理',
    redirect: '/content/post/all',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'post/*',
        title: '静态规则配置',
        icon: 'HomeOutlined',
        isHide: true,
        element: React.lazy(async () => await import('@/pages/Post')),
      },
      {
        path: 'post/unpublished',
        title: '动态规则配置',
        icon: 'HomeOutlined'
      }
    ]
  },
  {
    path: '/dockers',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: 'Docker管理',
    redirect: '/setting/category',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'url_record/*',
        title: '容器列表',
        icon: 'HomeOutlined',
        redirect: 'url_record',
        element: React.lazy(async () => await import('@/pages/UrlRecord')),
      }
    ]
  },
  {
    path: '/certificates',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: '证书管理',
    redirect: '/setting/category',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'url_record/*',
        title: '容器列表',
        icon: 'HomeOutlined',
        redirect: 'url_record',
        element: React.lazy(async () => await import('@/pages/UrlRecord')),
      }
    ]
  },{
    path: '/clusters',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: '集群配置',
    redirect: '/setting/category',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'url_record/*',
        title: '容器列表',
        icon: 'HomeOutlined',
        redirect: 'url_record',
        element: React.lazy(async () => await import('@/pages/UrlRecord')),
      }
    ]
  },{
    path: '/monitors',
    element: React.lazy(async () => await import('@/layouts/UserLayout')),
    title: '负载监控',
    redirect: '/setting/category',
    icon: 'HomeOutlined',
    children: [
      {
        path: 'url_record/*',
        title: '容器列表',
        icon: 'HomeOutlined',
        redirect: 'url_record',
        element: React.lazy(async () => await import('@/pages/UrlRecord')),
      }
    ]
  }
];

export default layouts;
