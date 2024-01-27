import { useAuth } from '@/layouts/AuthProvider';
import React from 'react';
import { Navigate } from 'react-router-dom';
import { Button, Checkbox, Form, Input } from 'antd';
import { Col, Row } from 'antd';
import { login } from '@/api/account';

const Login: React.FC = () => {
  const { login: localLogin }: any = useAuth();
  const onFinish = async (values: any) => {
    console.log('Success:', values);
    var rsp = await login(values);
    if (rsp !== null && rsp.code === 200) {
      // 如果登录成功, localLogin会自己做跳转
      localLogin(rsp.data);
    }
  };

  const onFinishFailed = (errorInfo: any) => {
    console.log('Failed:', errorInfo);
  };
  return (
    <>
      <Row>
        <Col span={12} offset={6}>
          <Form
            name="basic"
            labelCol={{ span: 8 }}
            wrapperCol={{ span: 16 }}
            style={{ maxWidth: 600 }}
            initialValues={{ remember: true }}
            onFinish={onFinish}
            onFinishFailed={onFinishFailed}
            autoComplete="off"
          >
            <Form.Item
              label="Username"
              name="name"
              rules={[{ required: true, message: 'Please input your username!' }]}
            >
              <Input />
            </Form.Item>

            <Form.Item
              label="Password"
              name="password"
              rules={[{ required: true, message: 'Please input your password!' }]}
            >
              <Input.Password />
            </Form.Item>

            <Form.Item name="remember" valuePropName="checked" wrapperCol={{ offset: 8, span: 16 }}>
              <Checkbox>Remember me</Checkbox>
            </Form.Item>

            <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
              <Button type="primary" htmlType="submit">
                Submit
              </Button>
            </Form.Item>
          </Form>
        </Col>
      </Row>
    </>
  )
}

export default Login;