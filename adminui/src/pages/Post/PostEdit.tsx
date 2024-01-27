// pages/Home/index.tsx
import React, { useEffect, useState } from 'react';
import { postGet, postGetForContentInfoModify, postModifyContentInfo } from '@/api/post';
import { useNavigate, useParams } from 'react-router-dom';
import { Form, Button, Space, notification } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import PostEditForm from './PostEditForm';

const PostEdit: React.FC = () => {
    let { postId } = useParams<any>();
    const navigate = useNavigate();
    const [form] = Form.useForm();
    const [initialValues, setInitialValues] = useState<any>({ postId });

    const onFinish = async (values: any) => {
        console.log('Success:', values);
        await postModifyContentInfo({ ...values })
        notification.info({
            message: `更新成功`,
            description: `更新成功`
        });
        await fetchArticle();
    };

    const fetchArticle = async () => {
        console.log("fetchArticle")
        var result = await postGetForContentInfoModify({ postId });
        var newValue = { ...initialValues, ...result.data }
        setInitialValues(newValue);
        form.setFieldsValue(newValue);
    }
    useEffect(() => {
        fetchArticle();
    }, [])

    return (
        <PageHeader title={`POST: [#${initialValues.postId}]-${initialValues.title}`} onBack={() => navigate("../")}
            extra={
                <Space size="middle">
                    <Button onClick={() => form.submit()}>Save</Button>
                    <Button onClick={fetchArticle}>Refresh</Button>
                </Space>}
        >
            <PostEditForm form={form} onFinish={onFinish} values={initialValues} />
        </PageHeader>
    )
}

export default PostEdit;