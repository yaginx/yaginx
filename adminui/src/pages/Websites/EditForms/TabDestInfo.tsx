import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const TabDestInfo = {
    label: 'DestInfo',
    key: 'tabDestInfo',
    forceRender: true,
    children: <>
        <Row gutter={[8, 8]}>
            <Col span={24}>
                <Form.Item label="转发配置" key={"proxyTransforms"}>
                    <Form.List name="proxyTransforms">
                        {(fields, { add, remove }) => (
                            <>
                                {fields.map(({ key, name, ...restField }) => (
                                    <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                                        <Form.Item
                                            {...restField}
                                            name={[name, 'key']}
                                            rules={[{ required: true, message: 'Missing key' }]}
                                        >
                                            <Input placeholder="属性" style={{}} />
                                        </Form.Item>
                                        <Form.Item
                                            {...restField}
                                            name={[name, 'value']}
                                            rules={[{ required: true, message: 'Missing value' }]}>
                                            <Input placeholder="值" style={{ width: "400px" }} />
                                        </Form.Item>
                                        <MinusCircleOutlined onClick={() => remove(name)} />
                                    </Space>
                                ))}
                                <Form.Item>
                                    <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                        添加转发配置
                                    </Button>
                                </Form.Item>
                            </>
                        )}
                    </Form.List>
                </Form.Item>
            </Col>
            <Col span={24}>
                <Form.Item label="转发规则" key={"proxyRules"}>
                    <Form.List name="proxyRules">
                        {(fields, { add, remove }) => (
                            <>
                                {fields.map(({ key, name, ...restField }) => (
                                    <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                                        <Form.Item
                                            {...restField}
                                            name={[name, 'pathPattern']}
                                            rules={[{ required: true, message: 'Missing key' }]}
                                        >
                                            <Input placeholder="规则名称" style={{}} />
                                        </Form.Item>
                                        <Form.Item>
                                            <Form.List name={[name, 'destinations']}>
                                                {(subFields, subOpt) => (
                                                    <div style={{ display: 'flex', flexDirection: 'column', rowGap: 16 }}>
                                                        {subFields.map((subField) => (
                                                            <Space key={subField.key}>
                                                                <Form.Item noStyle name={[subField.name, 'name']}>
                                                                    <Input placeholder="目标名称" />
                                                                </Form.Item>
                                                                <Form.Item noStyle name={[subField.name, 'address']}>
                                                                    <Input placeholder="目标地址" />
                                                                </Form.Item>
                                                                <CloseOutlined
                                                                    onClick={() => {
                                                                        subOpt.remove(subField.name);
                                                                    }}
                                                                />
                                                            </Space>
                                                        ))}
                                                        <Button type="dashed" onClick={() => subOpt.add()} block>
                                                            + 添加目标地址
                                                        </Button>
                                                    </div>
                                                )}
                                            </Form.List>
                                        </Form.Item>
                                        <MinusCircleOutlined onClick={() => remove(name)} />
                                    </Space>
                                ))}
                                <Form.Item>
                                    <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                        添加规则
                                    </Button>
                                </Form.Item>
                            </>
                        )}
                    </Form.List>
                </Form.Item>
            </Col>
        </Row>
    </>
}