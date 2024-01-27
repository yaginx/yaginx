import { Form, Input, Modal, Switch } from 'antd';
import React from 'react';
import { useState } from 'react';
import { CategoryTreeSelectInput } from '../../componets/CategoryTreeSelectInput';

export interface CreateFormProps {
    open: boolean;
    onSubmit: (values: any) => void;
    onCancel: () => void;
}

export function CreateForm({ open, onSubmit: onCreate, onCancel, }: CreateFormProps) {
    const [form] = Form.useForm();
    const [pageType, setPageType] = useState(1);

    return (
        <Modal
            open={open}
            title="新建分类"
            okText="Create"
            cancelText="Cancel"
            onCancel={onCancel}
            destroyOnClose={true}
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
                preserve={false}
                initialValues={{ pageType }}
            >
                {/* <Form.Item name="rootCategory" label="添加一级分类">
                    <Switch checkedChildren="一级分类" unCheckedChildren="非一级分类" checked={hideParentCategorySelector} onChange={isRootCategoryChanged} />
                </Form.Item> */}
                <Form.Item name="parentCategoryId" label="父级分类" rules={[{ required: true, message: '父级分类不能为空' }]} >
                    <CategoryTreeSelectInput />
                </Form.Item>
                <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="slug" label="Slug">
                    <Input />
                </Form.Item>
                <Form.Item name="memo" label="Memo">
                    <Input />
                </Form.Item>
            </Form>
        </Modal>
    );
};