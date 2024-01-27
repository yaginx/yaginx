// pages/Home/index.tsx
import React, { useState } from 'react';
import { Navigate, Outlet, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import PostCreate from './PostCreate';
import PostEdit from './PostEdit';
import PostList from './PostList';
import RedirectPage from '../Redirect';

const PostIndex: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    console.log(location);
    return (
        <>
            <Routes>
                <Route index element={<Navigate to={"./all"} />} />
                <Route path="all" element={<PostList />}></Route>
                <Route path="unpublished" element={<PostList publishStatus={false} />}></Route>
                <Route path="published" element={<PostList publishStatus={true} />}></Route>
                <Route path="create" element={<PostCreate />} />
                <Route path="edit/:postId" element={<PostEdit />} />
            </Routes>
            <Outlet />
        </>
    )
}

export default PostIndex;