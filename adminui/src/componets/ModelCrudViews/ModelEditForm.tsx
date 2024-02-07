import React, { useEffect, useState } from 'react';
import { Form, Input, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { AuditKeys } from './ModelCrudProps';
import { renderEditFormItem } from './renderEditFormItem';

export const ModelEditForm: React.FC<any> = (props) => {
  const { modelName, pkFieldName, fields } = props;
  const [tabItems, setTabItems] = useState<any[]>([]);


  const baseInfoFormItems = () => {
    var formItems = fields?.map((item: any) => renderEditFormItem(item.name))
    // debugger
    if (formItems && formItems?.length > 0) {
      return formItems;
    }

    return Object.keys(props.values).filter(item => AuditKeys.indexOf(item) < 0).map((item) => renderEditFormItem(item))
  }

  const buildTabItems = () => {
    const tabItems = [
      {
        label: '基本信息', key: 'tabBasicInfo', forceRender: true, children: baseInfoFormItems()
      }
    ];
    const tabExtraInfoItem = {
      label: '扩展信息', key: 'tabExtraInfo', forceRender: true, children: []
    };
    if (props.values[pkFieldName] || props.values["id"]) {
      tabItems.push(tabExtraInfoItem);
    }
    setTabItems(tabItems);
  }

  useEffect(() => {
    if (fields && fields.length > 0) {
      console.log("render tabItems")
      buildTabItems();
    } else if (Object.keys(props.values).length > 0) {
      console.log("render tabItems by object keys")
      buildTabItems();
    }
    else {
      console.log("fields is empty")
    }
  }, [fields, props.values])

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
      <Form.Item hidden name={pkFieldName}>
        <Input type={"hidden"} />
      </Form.Item>
      <Form.Item hidden name="ts">
        <Input type={"hidden"} />
      </Form.Item>
      <Tabs items={tabItems} />
    </Form>
  )
};
