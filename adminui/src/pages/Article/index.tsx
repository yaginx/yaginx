import React from "react";
import { Outlet, Route, Routes } from "react-router-dom";
import ArticleList from "./List";
import PostCreate from "@/pages/Post/PostCreate";
import PostEdit from "@/pages/Post/PostEdit";
import { Edit } from "./Edit";
import { CreateForm } from "./CreateForm";

const PostIndex: React.FC = () => {
    return (
        <>
            <Routes>
                <Route index element={<ArticleList />} />
                <Route path={`edit/:pageId`} element={<Edit />} />
            </Routes>
            <Outlet />
        </>
    )
}
export default PostIndex;