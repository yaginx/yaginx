// pages/Home/index.tsx
import React, { useState } from 'react';
import { Outlet, Route, Routes } from 'react-router-dom';
import ItemContentList from './ItemContentList';

const PostIndex: React.FC = () => {
    return (
        <>
            <Routes>
                <Route index element={<ItemContentList />} />
                {/* <Route path="unpublishedList" element={<PostList publishStatus={false} />}></Route>
                <Route path="publishedList" element={<PostList publishStatus={true} />}></Route> */}
                {/* <Route path="create" element={<PostCreate />} />
                <Route path="edit/:postId" element={<PostEdit />} /> */}
            </Routes>
            <Outlet />
        </>
    )
}

export default PostIndex;