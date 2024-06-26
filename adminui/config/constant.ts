// 基本前缀
export const BASE_URL = {
  development: '/api',
  production: '/',
  beta: '/beta',
  release: '/release',
};
// 开发端口
export const PORT = 1314;
// 基本路径
export const VITE_BASE_PATH = '/yaginx/adminui/';
// 请求接口地址
export const VITE_PROXY_HTTP = 'http://192.168.8.33:8080/'; //'https://sh01.vm.niusys.net/';
// 应用名称
export const VITE_APP_TITLE = 'YAGINX';
// 开启 mock
export const VITE_APP_MOCK: boolean = false;
// 开启包依赖分析 可视化
export const VITE_APP_ANALYZE: boolean = false;
// 开启Gzip压缩
export const VITE_APP_COMPRESS_GZIP: boolean = true;
// 开启Gzip压缩，删除原文件
export const VITE_APP_COMPRESS_GZIP_DELETE_FILE: boolean = false;
// 去除 console
export const VITE_DROP_CONSOLE: boolean = true;
// 开启兼容
export const VITE_APP_LEGACY: boolean = true;
