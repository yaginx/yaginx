import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "./AuthProvider";

export const ProtectedRoute = ({ children }: any) => {
    const { user }: any = useAuth();
    if (!user) {
        // user is not authenticated
        return <Navigate to="/" />;
    }
    return children;
};