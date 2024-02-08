import React, { useEffect, useState } from 'react';
import { Button, Form, Input, Space, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import TextArea from 'antd/es/input/TextArea';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const EditForm: React.FC<any> = (props) => {
  const [tabItems, setTabItems] = useState<any[]>([]);

  const buildTabItems = () => {
    const tabItems: any[] = [
      {
        label: '基本信息', key: 'tabBasicInfo', forceRender: true, children: <>
          <Form.Item name="name" label="名称" rules={[{ required: true, message: '标题' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="defaultHost" label="defaultHost" rules={[{ required: true, message: 'defaultHost' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="defaultDestination" label="defaultDestination" rules={[{ required: true, message: 'defaultDestination' }]}>
            <Input />
          </Form.Item>
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
                        <Input placeholder="规则" style={{}} />
                      </Form.Item>
                      <Form.Item
                        {...restField}
                        name={[name, 'address']}
                        rules={[{ required: true, message: 'Missing value' }]}
                      >
                        <Input placeholder="转发地址" style={{ width: "400px" }} />
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
        </>
      }
    ];
    const tabHostInfo = {
      label: '主机信息', key: 'tabHostInfo', forceRender: true, children: []
    };
    tabItems.push(tabHostInfo);

    const tabProxyRuleInfo = {
      label: '代理信息', key: 'tabProxyRuleInfo', forceRender: true, children: []
    };
    tabItems.push(tabProxyRuleInfo);

    setTabItems(tabItems);
  }

  useEffect(() => {
    buildTabItems();
  }, [props.values])

  return (
    <Form
      name="basic"
      layout="vertical"
      {...formItemDefaultLayout}
      // {...props}
      form={props.form}
      onFinish={props.onFinish}
      initialValues={props.values}
      autoComplete="off"
    >
      <Form.Item hidden name={"id"}>
        <Input type={"hidden"} />
      </Form.Item>
      <Form.Item hidden name="ts">
        <Input type={"hidden"} />
      </Form.Item>
      <Tabs items={tabItems} />
    </Form>
  )
};
