// pages/Home/index.tsx
import React, { useState } from 'react';
import { PostListView } from '@/componets/posts/PostListView';
import { Outlet, Route, Routes } from 'react-router-dom';
import { PostCreateView } from '@/componets/posts/PostCreateView';
import { PostEditView } from '@/componets/posts/PostEditView';
import UserLayout from '@/layouts/UserLayout';

// const PostIndex: React.FC = () => {
//     const [postKind] = useState<number>(8);
//     return (
//         <>
//             <Routes>
//                 <Route path='/' element={<UserLayout />}>
//                     <Route index element={<PostListView postKind={postKind} />}></Route>
//                     <Route path="create" element={<PostCreateView postKind={postKind} />} />
//                     <Route path="edit/:postId" element={<PostEditView postKind={postKind} />} />
//                 </Route>
//             </Routes>
//             <Outlet />
//         </>
//     )
// }

// export default PostIndex;