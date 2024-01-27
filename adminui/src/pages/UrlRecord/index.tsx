import React from "react";
import { Link, Outlet, Route, Routes } from "react-router-dom";
import { Space } from "antd";
import UrlRecordList from "./List";

export default function UrlRecordIndex() {
    return (
        // <div>
        //     <Routes>
        //         {/* <Route path={`create`} element={<BuildParametersCreate />} />
        //         <Route path={`edit/:name`} element={<BuildParametersEdit />} /> */}
        //         <Route path="" element={<UrlRecordList />} />
        //     </Routes>
        // </div>
        <>
            <Routes>
                <Route index element={<UrlRecordList />} />
            </Routes>
            <Outlet />
        </>
    );
}
