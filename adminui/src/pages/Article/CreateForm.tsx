import { CategoryKind } from '@/PageCategoryKind';
import { Form, Input, Modal } from 'antd';
import React from 'react';
import { useState } from 'react';
import { CategorySelectInput, EnumSelectInput } from './SearchInput';

export interface CreateFormProps {
    visible: boolean;
    onCreate: (values: any) => void;
    onCancel: () => void;
}

export function CreateForm({ visible, onCreate, onCancel, }: CreateFormProps) {
    const [form] = Form.useForm();
    const [pageType, setPageType] = useState(1);
    return (
        <Modal
            open={visible}
            title="新建文章"
            okText="Create"
            cancelText="Cancel"
            onCancel={onCancel}
            onOk={() => {
                form.validateFields()
                    .then(values => {
                        onCreate(values);
                    })
                    .catch(info => {
                        console.log('Validate Failed:', info);
                    });
            }}>
            <Form
                form={form}
                layout="vertical"
                name="form_in_modal"
                initialValues={{ pageType }}
            >
                <Form.Item name="pageType" label="页面类型" rules={[{ required: true, message: '页面类型不能为空' }]}>
                    <EnumSelectInput enumType="PageType" onChange={(value: any) => setPageType(value)} />
                </Form.Item>
                <Form.Item name="categoryId" label="内容分类" rules={[{ required: true, message: '内容分类不能为空' }]}>
                    <CategorySelectInput categoryKind={CategoryKind.PageCategory} typeLevel1={pageType} />
                </Form.Item>
                <Form.Item name="metaSlug" label="URL" rules={[{ required: true, message: '浏览器中的地址不能为空' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="title" label="Title" rules={[{ required: true, message: '页面内容标题不能为空' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="metaTitle" label="MetaTitle" rules={[{ required: true, message: '页面Header中的Title不能为空' }]}>
                    <Input />
                </Form.Item>
            </Form>
        </Modal>
    );
};