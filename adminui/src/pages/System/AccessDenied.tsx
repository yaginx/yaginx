import React from 'react';
import { Navigate } from 'react-router-dom';
import { Button, Result } from 'antd';
import { useAuth } from '@/layouts/RootAuthProvider';

const AccessDenied: React.FC = () => {
  const { authInfo }: any = useAuth();
  if (authInfo && authInfo.Token) {
    return <Navigate to="/" replace={true} />;
  }

  return (
    <Result
      status="403"
      title="403"
      subTitle="Sorry, you are not authorized to access this page."
      extra={<Button type="primary" href='/'>Back Home</Button>}
    />
  );
};
export default AccessDenied;
