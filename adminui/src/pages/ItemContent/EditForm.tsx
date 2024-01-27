import { itemContentGet } from '@/api/post';
import { CategorySelectInput } from '@/componets/CategorySelectInput';
import { Form, Input, Modal, Select, Space } from 'antd';
import React, { useEffect } from 'react';

export interface EditFormProps {
    open: boolean;
    contentId: Number | null;
    onSubmit: (values: any) => void;
    onCancel: () => void;
}
export function EditForm(props: EditFormProps) {
    const [form] = Form.useForm();
    const { open, contentId, onSubmit: onCreate, onCancel } = props;
    useEffect(() => {
        const fetchRecord = async () => {
            if (!contentId) {
                return;
            }
            var result = await itemContentGet({ contentId });
            if (result?.data) {
                form.setFieldsValue(result.data);
            }
        };

        if (open) {
            debugger
            if (contentId === null) {
                form.resetFields();
            }
            else {
                fetchRecord();
            }
        }
    }, [open])
    return (
        <Modal
            open={open}
            title={contentId === null ? "新建" : "编辑"}
            width={900}
            okText="保存"
            cancelText="取消"
            onCancel={onCancel}
            onOk={() => {
                form
                    .validateFields()
                    .then(values => {
                        onCreate(values);
                    })
                    .catch(info => {
                        console.log('Validate Failed:', info);
                    });
            }}
        >
            <Form form={form} layout="vertical" initialValues={{ contentId }} >
                <Form.Item name="contentId" hidden={true} >
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item hidden name="ts">
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item name="categoryId" label="类型" rules={[{ required: true, message: '类型' }]}>
                    <CategorySelectInput parentCategoryName={"ITEM_CONTENT"} />
                </Form.Item>
                <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="value" label="Value">
                    <Input.TextArea rows={8} />
                </Form.Item>
                <Form.Item name="sortOrder" label="sortOrder">
                    <Input />
                </Form.Item>
            </Form>
        </Modal>
    );
};