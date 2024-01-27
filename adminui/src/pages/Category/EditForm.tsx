import { categoryGet } from '@/api/category';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { Button, Form, Input, Modal, Select, Space } from 'antd';
import React, { useEffect } from 'react';

export interface EditFormProps {
    open: boolean;
    categoryId: Number;
    onSubmit: (values: any) => void;
    onCancel: () => void;
}
export function CategoryEditForm(props: EditFormProps) {
    const [form] = Form.useForm();
    const { open, categoryId, onSubmit: onCreate, onCancel } = props;
    useEffect(() => {
        const fetchUrlRecord = async () => {
            var result = await categoryGet({ categoryId });
            if (result?.data) {
                form.setFieldsValue(result.data);
            }
        };
        if (open) {
            if (categoryId == null) {
                form.resetFields();
            }
            else {
                fetchUrlRecord();
            }
        }
    }, [open])
    return (
        <Modal
            open={open}
            title={categoryId === null ? "新建分类" : "编辑分类"}
            width={900}
            okText="保存"
            cancelText="取消"
            onCancel={onCancel}
            onOk={() => {
                form
                    .validateFields()
                    .then(values => {
                        onCreate(values);
                        //form.resetFields();
                    })
                    .catch(info => {
                        console.log('Validate Failed:', info);
                    });
            }}
        >
            <Form form={form} layout="vertical" initialValues={{ categoryId }} >
                <Form.Item name="categoryId" hidden={true} >
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item hidden name="ts">
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item name="name" label="Name" rules={[{ required: true, message: 'Name is required' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="normalizeName" label="NormalizeName" rules={[{ required: true, message: 'NormalizeName is required' }]}>
                    <Input />
                </Form.Item>
                <Form.Item name="slug" label="Slug">
                    <Input />
                </Form.Item>
                <Form.Item name="tags" label="Tags">
                    <Select mode="tags" tokenSeparators={[',']} />
                </Form.Item>
                <Form.Item label="元数据">
                    <Form.List name="metadic">
                        {(fields, { add, remove }) => (
                            <>
                                {fields.map(({ key, name, ...restField }) => (
                                    <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                                        <Form.Item
                                            {...restField}
                                            name={[name, 'key']}
                                            rules={[{ required: true, message: 'Missing key' }]}
                                        >
                                            <Input placeholder="Key" style={{}} />
                                        </Form.Item>
                                        <Form.Item
                                            {...restField}
                                            name={[name, 'value']}
                                            rules={[{ required: true, message: 'Missing value' }]}
                                        >
                                            <Input placeholder="value" style={{ width: "400px" }} />
                                        </Form.Item>
                                        <MinusCircleOutlined onClick={() => remove(name)} />
                                    </Space>
                                ))}
                                <Form.Item>
                                    <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                        Add field
                                    </Button>
                                </Form.Item>
                            </>
                        )}
                    </Form.List>
                </Form.Item>
                <Form.Item name="memo" label="备注" rules={[{ required: false, message: '备注' }]}>
                    <Input />
                </Form.Item>
            </Form>
        </Modal>
    );
};