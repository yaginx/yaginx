import React from 'react';
import { Navigate } from 'react-router-dom';

const RedirectPage: React.FC = () => {
  return <Navigate to="/home/" />;
};
export default RedirectPage;
