import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { TabBasicInfo } from './EditForms/TabBasicInfo';
import { TabHostInfo } from './EditForms/TabHostInfo';
import { TabDestInfo } from './EditForms/TabDestInfo';
import { TabSimpleResponseInfo } from './EditForms/TabSimpleResponseInfo';

export const EditForm: React.FC<any> = (props) => {
  const [tabItems, setTabItems] = useState<any[]>([]);

  useEffect(() => {
    setTabItems([TabBasicInfo, TabHostInfo, TabDestInfo, TabSimpleResponseInfo]);
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
