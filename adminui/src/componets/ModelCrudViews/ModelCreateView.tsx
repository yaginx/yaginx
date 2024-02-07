import React, { useEffect, useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { Form } from 'antd';
import { ModelEditForm } from './ModelEditForm';
import { IModelCreateViewProps } from './ModelCrudProps';

export const ModelCreateView: React.FC<IModelCreateViewProps> = (props) => {
  const { modelName, pkFieldName, fields } = props;
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [initialValues, setInitialValues] = useState<any>({});
  const onFinish = async (values: any) => {
    console.log('Success:', values);
    var rsp = await props.modelCreateSubmitApi({ ...values });
    notification.info({
      message: `添加成功`,
      description: `添加成功`
    });
    let id = (pkFieldName !== undefined && rsp.data && rsp.data.hasOwnProperty(pkFieldName)) ? rsp.data[pkFieldName] : rsp.data;
    navigate(`../edit/${id}`);
  };
  return (
    <PageHeader title={`${modelName}: Create New One`} onBack={() => navigate("../")}
      extra={<Space size="middle">
        <Button onClick={() => form.submit()}>Save</Button>
      </Space>}
    >
      <ModelEditForm form={form} onFinish={onFinish} values={initialValues} {...props} />
    </PageHeader>
  );
};
