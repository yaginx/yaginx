// vite-plugin-imp、vite-plugin-style-import
// 两款按需加载都存在部分问题，目前先按照全局引入
// 引入 less 文件，使vite的配置可以替换主题
import 'antd/dist/reset.css';
import './global.less';
import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
