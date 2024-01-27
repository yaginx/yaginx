import { urlRecordGet } from '@/api/urlRecordApi';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { Button, Form, Input, Modal, Space } from 'antd';
import React from 'react';
import { useEffect } from 'react';
import { EnumSelectInput } from '../Article/SearchInput';

export interface CollectionCreateFormProps {
    urlRecordId: any;
    visible: boolean;
    onCreate: (values: any) => void;
    onCancel: () => void;
}

export function UrlRecordModalEditForm(props: CollectionCreateFormProps) {
    const [form] = Form.useForm();
    const { urlRecordId, visible, onCreate, onCancel } = props;
    useEffect(() => {
        const fetchUrlRecord = async () => {
            var result = await urlRecordGet({ urlRecordId });
            if (result?.data) {
                form.setFieldsValue(result.data);
            }
        };
        if (visible) {
            if (urlRecordId == null) {
                form.resetFields();
            }
            else {
                fetchUrlRecord();
            }
        }
    }, [visible])
    return (
        <Modal
            open={visible}
            title={urlRecordId === null ? "新建UrlRecord" : "编辑UrlRecord"}
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
            <Form form={form} layout="vertical" initialValues={{ urlRecordId }} >
                <Form.Item name="urlRecordId" hidden={true} >
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item name="category" label="类别" rules={[{ required: true, message: '类别不能为空' }]}>
                    <EnumSelectInput enumType="UrlRecordCategory" />
                </Form.Item>
                <Form.Item name="handleType" label="处理方式" rules={[{ required: true, message: '处理方式不能为空' }]}>
                    <EnumSelectInput enumType="UrlRecordHandleType" />
                </Form.Item>
                <Form.Item name="slug" label="Slug" rules={[{ required: true, message: 'Slug不能为空' }]}>
                    <Input />
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