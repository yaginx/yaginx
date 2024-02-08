import React, { useEffect, useState } from 'react';
import { Form, Input, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import TextArea from 'antd/es/input/TextArea';

export const CreateForm: React.FC<any> = (props) => {
  const [tabItems, setTabItems] = useState<any[]>([]);

  const buildTabItems = () => {
    const tabItems: any[] = [
      {
        label: '基本信息', key: 'tabBasicInfo', forceRender: true, children: <>
          <Form.Item name="name" label="名称" rules={[{ required: true, message: '标题' }]}>
            <Input />
          </Form.Item>
          {/* <Form.Item name="slug" label="slug">
            <Input />
          </Form.Item> */}
        </>
      }
    ];
    const tabExtraInfoItem = {
      label: '扩展信息', key: 'tabExtraInfo', forceRender: true, children: []
    };
    // if (props.values[pkFieldName] || props.values["id"]) {
    //   tabItems.push(tabExtraInfoItem);
    // }
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
      {/* <Form.Item hidden name={pkFieldName}>
        <Input type={"hidden"} />
      </Form.Item> */}
      <Form.Item hidden name="ts">
        <Input type={"hidden"} />
      </Form.Item>
      <Tabs items={tabItems} />
    </Form>
  )
};
