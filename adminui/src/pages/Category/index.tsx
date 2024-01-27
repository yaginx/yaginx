import React from "react";
import { Routes, Route, Outlet } from "react-router-dom";
import List from "./List";

const Index: React.FC = () => {
    return (
        <>
            <Routes>
                <Route index element={<List />}></Route>
            </Routes>
            <Outlet />
        </>
    )
}

export default Index;