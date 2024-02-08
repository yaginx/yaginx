import React, { useEffect, useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { Form } from 'antd';
import { EditForm } from './EditForm';
import { websiteUpsert } from '@/api/docker';
import { CreateForm } from './CreateForm';

export const Create: React.FC = (props) => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [initialValues, setInitialValues] = useState<any>({});
  const onFinish = async (values: any) => {
    var rsp = await websiteUpsert({ ...values });
    notification.info({
      message: `添加成功`,
      description: `添加成功`
    });
    let id = (rsp !== undefined && rsp.data && rsp.data.hasOwnProperty("id")) ? rsp.data["id"] : rsp.data;
    navigate(`../edit/${id}`);
  };
  return (
    <PageHeader title={`Create New One`} onBack={() => navigate("../")}
      extra={<Space size="middle">
        <Button onClick={() => form.submit()}>Save</Button>
      </Space>}
    >
      <CreateForm form={form} onFinish={onFinish} values={initialValues} {...props} />
    </PageHeader>
  );
};
