import React, { useEffect, useState } from 'react';
import { Form, Input, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import TextArea from 'antd/es/input/TextArea';

export const EditForm: React.FC<any> = (props) => {
  const [tabItems, setTabItems] = useState<any[]>([]);

  const buildTabItems = () => {
    const tabItems: any[] = [
      {
        label: '基本信息', key: 'tabBasicInfo', forceRender: true, children: <>
          <Form.Item name="name" label="名称" rules={[{ required: true, message: '标题' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="isUseFreeCert" label="isUseFreeCert" valuePropName="checked">
            <Switch checkedChildren="开启" unCheckedChildren="关闭" />
          </Form.Item>
          <Form.Item name="isVerified" label="isVerified" valuePropName="checked">
            <Switch checkedChildren="开启" unCheckedChildren="关闭" />
          </Form.Item>
          {/* <Form.Item name="slug" label="slug">
            <Input />
          </Form.Item> */}
        </>
      }
    ];
    // const tabHostInfo = {
    //   label: '主机信息', key: 'tabHostInfo', forceRender: true, children: []
    // };
    // tabItems.push(tabHostInfo);

    // const tabProxyRuleInfo = {
    //   label: '代理信息', key: 'tabProxyRuleInfo', forceRender: true, children: []
    // };
    // tabItems.push(tabProxyRuleInfo);

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
