import { Layout, Progress, Space } from 'antd';
import React, { useEffect, useState } from 'react';
import { IAuthContext, useAuth } from './RootAuthProvider';

const { Footer } = Layout;
const AppFooter: React.FC = () => {

  const { authInfo }: IAuthContext = useAuth();
  const [tokenExpireIn, setTokenExpireIn] = useState<number>(authInfo.tokenExpireTime - Math.floor(Date.now() / 1000));
  const [percentIndicator, setPercentIndicator] = useState<number>(0);

  // 刷新定时器
  useEffect(() => {
    const interval = setInterval(() => {
      var value = authInfo.tokenExpireTime - Math.floor(Date.now() / 1000) + 1;
      // 展示倒计时
      setTokenExpireIn(value);
      // 展示百分比进度条
      setPercentIndicator(Number.parseInt((value / authInfo.maxValidSeconds * 100).toFixed(0)));
    }, 1000);
    return () => clearInterval(interval);
  }, [authInfo.token])

  return (
    <Footer className={"layout_footer"} style={{ userSelect: "none", padding: "0 0px", lineHeight: "35px" }}>
      <div className='footer_left'>
        {/* <div className='function_button_list'>
            <Space>
                {footerMenuItems.map(item => <div key={item.key} onClick={item.onClick}>{item.icon}</div>)}
            </Space>
        </div> */}
        <div className='progress_bar'>
          <Space>
            <span>状态:</span>
            <Progress strokeLinecap="butt" style={{ userSelect: "none" }} percent={percentIndicator} />
            <span>Session续期时间: {tokenExpireIn.toFixed(0)}</span>
          </Space>
        </div>
      </div>
      <div className='footer_middle'>
        <a href='https://woscm.com' target={"_blank"}>WoScm</a> <span> ©2023</span>
      </div>
      {/* <div className='footer_right'>
    </div> */}
    </Footer>
  )
};
export default AppFooter;



