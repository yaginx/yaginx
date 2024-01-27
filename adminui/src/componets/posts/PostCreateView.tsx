// pages/Home/index.tsx
import React, { useState } from 'react';
import { postCreate } from '@/api/post';
import { useNavigate } from 'react-router-dom';
import { Form, Button, Space, notification } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { PostEditFormView } from './PostEditFormView';

export const PostCreateView: React.FC<any> = (props: any) => {
    const navigate = useNavigate();
    const [form] = Form.useForm();
    const [initialValues, setInitialValues] = useState<any>({ contentFormat: 1 });
    const { postKind } = props;

    const onFinish = async (values: any) => {
        var rsp = await postCreate({ ...values, postKind })
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
            <PostEditFormView form={form} onFinish={onFinish} values={initialValues} />
        </PageHeader>
    )
}