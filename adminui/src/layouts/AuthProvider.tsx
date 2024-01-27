import React, { createContext, useContext, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useLocalStorage } from "./useLocalStorage";

// https://blog.logrocket.com/complete-guide-authentication-with-react-router-v6/
const AuthContext = createContext({});

export const AuthProvider = ({ children }: any) => {
    const [authInfo, setAuthInfo] = useLocalStorage("auth", null);
    const [token, setToken] = useLocalStorage("token", null);
    const navigate = useNavigate();
    
    // call this function when you want to authenticate the user
    const login = async (data: any) => {
        const { token, ...authModel } = data;
        setAuthInfo(authModel);
        if (token) {
            setToken(token);
        }
        navigate("/");
    };

    // call this function to sign out logged in user
    const logout = () => {
        setAuthInfo(null);
        setToken(null);
        navigate("/", { replace: true });
    };

    const value = useMemo(
        () => ({
            authInfo,
            token,
            login,
            logout
        }),
        [token, authInfo]
    );
    return (<AuthContext.Provider value={value}>{children}</AuthContext.Provider>);
};

export const useAuth = () => {
    return useContext(AuthContext);
};