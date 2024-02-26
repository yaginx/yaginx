import { login } from '@/api/account';
import { useAuth } from '@/layouts/RootAuthProvider';
import {
  AlipayOutlined,
  LockOutlined,
  MobileOutlined,
  TaobaoOutlined,
  UserOutlined,
  WeiboOutlined,
} from '@ant-design/icons';
import {
  LoginFormPage,
  ProConfigProvider,
  ProFormCaptcha,
  ProFormCheckbox,
  ProFormText,
} from '@ant-design/pro-components';
import { Button, Divider, Space, Tabs, message, theme } from 'antd';
import type { CSSProperties } from 'react';
import React from 'react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

type LoginType = 'phone' | 'account';

const iconStyles: CSSProperties = {
  color: 'rgba(0, 0, 0, 0.2)',
  fontSize: '18px',
  verticalAlign: 'middle',
  cursor: 'pointer',
};

const Page = () => {
  const [loginType, setLoginType] = useState<LoginType>('account');
  const { token } = theme.useToken();

  const { login: localLogin }: any = useAuth();
  const navigate = useNavigate();
  const onSubmit = async (values: any) => {
    console.log('Success:', values);
    var rsp = await login(values);
    if (rsp !== null && rsp.code === 200) {
      // 如果登录成功, localLogin会自己做跳转
      localLogin(rsp.data);
      navigate("/", { replace: true });
    }
  };

  const onSubmitFailed = (errorInfo: any) => {
    console.log('Failed:', errorInfo);
  };


  const onFocus = () => {
    console.log("变化");
  }

  return (
    <div
      style={{
        backgroundColor: 'white',
        height: '100vh',
      }}
    >
      <LoginFormPage
        backgroundImageUrl="https://cn.bing.com/th?id=OHR.DevetashkaCave_ROW3161324044_1920x1080.webp&qlt=50"
        // logo="https://logos-download.com/wp-content/uploads/2016/03/Starbucks_Logo_1992.png"
        // backgroundVideoUrl="https://gw.alipayobjects.com/v/huamei_gcee1x/afts/video/jXRBRK_VAwoAAAAAAAAAAAAAK4eUAQBr"
        title="Yaginx"
        // containerStyle={{
        //   backgroundColor: 'rgba(0, 0, 0,0.65)',
        //   backdropFilter: 'blur(4px)',
        // }}
        onFinish={onSubmit}
        onFinishFailed={onSubmitFailed}
        onFocus={onFocus}
        subTitle="WebSite => Gateway, Certificate, Docker, Monitor, Waf"
      >
        <Tabs
          centered
          activeKey={loginType}
          onChange={(activeKey) => setLoginType(activeKey as LoginType)}
        >
          <Tabs.TabPane key={'account'} tab={'账号密码登录'} />
          {/* <Tabs.TabPane key={'phone'} tab={'手机号登录'} /> */}
        </Tabs>
        {loginType === 'account' && (
          <>
            <ProFormText
              name="email"
              fieldProps={{
                size: 'large',
                prefix: (
                  <UserOutlined
                    style={{
                      color: token.colorText,
                    }}
                    className={'prefixIcon'}
                  />
                ),
              }}
              placeholder={'Email: admin@yaginx.com'}
              rules={[
                {
                  required: true,
                  message: '请输入邮件地址!',
                },
              ]}
            />
            <ProFormText.Password
              name="password"
              fieldProps={{
                size: 'large',
                prefix: (
                  <LockOutlined
                    style={{
                      color: token.colorText,
                    }}
                    className={'prefixIcon'}
                  />
                ),
              }}
              placeholder={'密码: admin'}
              rules={[
                {
                  required: true,
                  message: '请输入密码！',
                },
              ]}
            />
          </>
        )}       
        <div
          style={{
            marginBlockEnd: 24,
          }}
        >
          <ProFormCheckbox noStyle name="autoLogin">
            自动登录
          </ProFormCheckbox>
          <a
            style={{
              float: 'right',
            }}
          >
            忘记密码
          </a>
        </div>
      </LoginFormPage>
    </div>
  );
};

export default () => {
  return (
    <ProConfigProvider >
      <Page />
    </ProConfigProvider>
  );
};
