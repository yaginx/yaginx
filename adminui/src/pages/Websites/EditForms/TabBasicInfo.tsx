import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const TabBasicInfo = {
    label: '基本信息',
    key: 'tabBasicInfo',
    forceRender: true,
    children: <>
        <Row gutter={[8, 8]}>
            <Col span={8}>
                <Form.Item name="name" label="名称" rules={[{ required: true, message: '标题' }]}>
                    <Input />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="defaultHost" label="defaultHost">
                    <Input />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="defaultDestination" label="defaultDestination">
                    <Input />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="defaultDestinationHost" label="defaultDestinationHost">
                    <Input />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="webProxy" label="webProxy">
                    <Input />
                </Form.Item>
            </Col>
        </Row>
        <Row gutter={[8, 8]}>
            <Col span={8}>
                <Form.Item name="isWithOriginalHostHeader" label="是否携带源主机头(Host)" valuePropName="checked">
                    <Switch checkedChildren="携带源主机头" unCheckedChildren="不携带源主机头" />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="isAllowUnsafeSslCertificate" label="是否检查SSL证书有效性" valuePropName="checked">
                    <Switch checkedChildren="忽略SSL证书检查" unCheckedChildren="严格检查SSL证书有效性" />
                </Form.Item>
            </Col>
            <Col span={8}>
                <Form.Item name="isAutoRedirectHttp2Https" label="Http自动跳转到Https" valuePropName="checked">
                    <Switch checkedChildren="自动跳转" unCheckedChildren="不自动跳转" />
                </Form.Item>
            </Col>
        </Row>
    </>
}