import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const TabHostInfo = {
    label: '主机信息',
    key: 'tabHostInfo',
    forceRender: true,
    children: <>
        <Row gutter={[8, 8]}>
            <Col span={24}>
                <Form.List
                    name="hosts"
                // rules={[
                //   {
                //     validator: async (_, names) => {
                //       if (!names || names.length < 2) {
                //         return Promise.reject(new Error('At least 2 passengers'));
                //       }
                //     },
                //   },
                // ]}
                >
                    {(fields, { add, remove }, { errors }) => (
                        <>
                            {fields.map((field, index) => (
                                <Form.Item
                                    // {...(index === 0 ? formItemLayout : formItemLayoutWithOutLabel)}
                                    label={index === 0 ? '主机头' : ''}
                                    required={false}
                                    key={field.key}
                                >
                                    <Form.Item
                                        {...field}
                                        validateTrigger={['onChange', 'onBlur']}
                                        rules={[
                                            {
                                                required: true,
                                                whitespace: true,
                                                message: "Please input host or delete this field.",
                                            },
                                        ]}
                                        noStyle
                                    >
                                        <Input placeholder="Host" style={{ width: '60%' }} />
                                    </Form.Item>
                                    {fields.length >= 0 ? (
                                        <MinusCircleOutlined
                                            className="dynamic-delete-button"
                                            onClick={() => remove(field.name)}
                                        />
                                    ) : null}
                                </Form.Item>
                            ))}
                            <Form.Item>
                                <Space direction="horizontal" >
                                    <Button
                                        type="dashed"
                                        onClick={() => add()}
                                        style={{}}
                                        icon={<PlusOutlined />}
                                    >
                                        添加主机头
                                    </Button>
                                    <Button
                                        type="dashed"
                                        onClick={() => {
                                            add('The head item', 0);
                                        }}
                                        style={{}}
                                        icon={<PlusOutlined />}
                                    >
                                        在顶部添加
                                    </Button>
                                </Space>
                                <Form.ErrorList errors={errors} />
                            </Form.Item>
                        </>
                    )}
                </Form.List>
            </Col>
        </Row>
    </>
}