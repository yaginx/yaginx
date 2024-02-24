import React, { useEffect, useState } from 'react';
import { Button, Card, Col, Form, Input, InputNumber, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const TabSimpleResponseInfo = {
    label: '简单请求',
    key: 'tabSimpleResponse',
    forceRender: true,
    children: <>
        <Form.List name="simpleResponses">
            {(fields, { add, remove }) => (
                <div style={{ display: 'flex', rowGap: 16, flexDirection: 'column' }}>
                    {fields.map((field) => (
                        <Card
                            size="small"
                            title={`Item ${field.name + 1}`}
                            key={field.key}
                            extra={
                                <CloseOutlined
                                    onClick={() => {
                                        remove(field.name);
                                    }}
                                />
                            }
                        >
                            <Row gutter={[8, 8]}>
                                <Col span={6}>
                                    <Form.Item label="Url" name={[field.name, 'url']} rules={[{ required: true, message: 'Please input!' }]}>
                                        <Input prefix="/" />
                                    </Form.Item>
                                </Col>
                                <Col span={6}>
                                    <Form.Item label="ContentType" name={[field.name, 'contentType']} initialValue={"text/plain"} rules={[{ required: true, message: 'Please input!' }]}>
                                        <Input placeholder='text/plain' />
                                    </Form.Item>
                                </Col>
                                <Col span={3}>
                                    <Form.Item label="StatusCode" name={[field.name, 'statusCode']} initialValue={200} rules={[{ required: true, message: 'Please input!', type: 'number', min: 100, max: 999 }]} required>
                                        <InputNumber style={{ width: 200 }} />
                                    </Form.Item>
                                </Col>

                            </Row>
                            <Row gutter={[8, 8]}>
                                <Col span={24}>
                                    <Form.Item label="Content" name={[field.name, 'responseContent']} rules={[{ required: true, message: 'Please input!' }]}>
                                        <Input.TextArea rows={4} />
                                    </Form.Item>
                                    {/* Nest Form.List */}
                                    <Form.Item label="ExtraHeader">
                                        <Form.List name={[field.name, 'extraHeaders']}>
                                            {(subFields, subOpt) => (
                                                <div style={{ display: 'flex', flexDirection: 'column', rowGap: 8 }}>
                                                    {subFields.map((subField) => (
                                                        <Space key={subField.key}>
                                                            <Form.Item noStyle name={[subField.name, 'key']}>
                                                                <Input placeholder="Header Name" style={{ width: "300px" }} />
                                                            </Form.Item>
                                                            <Form.Item noStyle name={[subField.name, 'value']}>
                                                                <Input placeholder="Value" style={{ width: "600px" }} />
                                                            </Form.Item>
                                                            <CloseOutlined
                                                                onClick={() => {
                                                                    subOpt.remove(subField.name);
                                                                }}
                                                            />
                                                        </Space>
                                                    ))}
                                                    <Button type="dashed" onClick={() => subOpt.add()} block>
                                                        + Add Extra Header Item
                                                    </Button>
                                                </div>
                                            )}
                                        </Form.List>
                                    </Form.Item>
                                </Col>
                            </Row>
                        </Card>
                    ))}

                    <Button type="dashed" onClick={() => add()} block>
                        + Add Item
                    </Button>
                </div>
            )}
        </Form.List>
    </>
}