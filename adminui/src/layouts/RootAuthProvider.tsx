import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { DEFAULT_AUTH_INFO, IAuthInfo, useAccessStore } from "@/store/accessStore";
import workerScript from "@/woker";
export interface IAuthContext {
  authInfo: IAuthInfo,
  isAuthorized: boolean,
  login: (data: IAuthInfo) => void,
  logout: () => void
}
const DEFAULT_AUTH_CONTEXT: IAuthContext = {
  authInfo: DEFAULT_AUTH_INFO,
  isAuthorized: false,
  login: (data: IAuthInfo) => { },
  logout: () => { }
}
// https://blog.logrocket.com/complete-guide-authentication-with-react-router-v6/
const AuthContext = createContext<IAuthContext>(DEFAULT_AUTH_CONTEXT);

// 负责提供Auth信息
export const RootAuthProvider = ({ children }: any) => {
  const accessStore = useAccessStore();
  const [token, setToken] = useState<string>(accessStore.token);
  const [authInfo, setAuthInfo] = useState<IAuthInfo>(accessStore.authInfo());
  const [isAuthorized, setIsAuthorized] = useState<boolean>(accessStore.isAuthorized())
  const navigate = useNavigate();

  useEffect(() => {
    let worker = new Worker(workerScript);
    worker.onmessage = (m: any) => {
      accessStore.checkAuthInfo();
    };

    return () => {
      // 记得在组件销毁时清除worker
      worker.terminate();
    }
  }, [])

  useEffect(() => {
    //console.debug("AuthProvider Detected Token Changed, " + authInfo.tokenExpireTime);// 测试正常
    setAuthInfo(accessStore.authInfo());
    setToken(accessStore.authInfo().token);
    setIsAuthorized(accessStore.isAuthorized());
  }, [accessStore.authInfo().token])

  const login = async (data: IAuthInfo) => {
    accessStore.updateToken(data);
  };

  const logout = () => {
    accessStore.emptyToken();
    navigate("/login", { replace: true });
  };

  const value: IAuthContext = useMemo(() => ({ authInfo, isAuthorized, login, logout }), [token]);
  return (<AuthContext.Provider value={value}>{children}</AuthContext.Provider>);
};
export const useAuth = () => useContext(AuthContext);
