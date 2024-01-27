// pages/Home/index.tsx
import React, { useState } from 'react';
import { postCreate } from '@/api/post';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Space, notification } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import PostEditForm from './PostEditForm';

export const PostCreate: React.FC = () => {
    const navigate = useNavigate();
    const [form] = Form.useForm();
    const [initialValues, setInitialValues] = useState<any>({ contentFormat: 1 });

    const onFinish = async (values: any) => {
        console.log('Success:', values);
        var rsp = await postCreate({ ...values })
        notification.info({
            message: `添加成功`,
            description: `添加成功`
        });
        let postId = rsp.data;
        navigate(`../edit/${postId}`);
    };
    return (
        <PageHeader title={`POST: Create New One`} onBack={() => navigate("../")}
            extra={
                <Space size="middle">
                    <Button onClick={() => form.submit()}>Save</Button>
                </Space>}
        >
            <PostEditForm form={form} onFinish={onFinish} values={initialValues} />
        </PageHeader>
    )
}

export default PostCreate;
