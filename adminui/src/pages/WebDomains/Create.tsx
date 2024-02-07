import React, { useEffect, useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { Form } from 'antd';
import { EditForm } from './EditForm';
import { webDomainUpsert, websiteUpsert } from '@/api/docker';

export const Create: React.FC = (props) => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [initialValues, setInitialValues] = useState<any>({});
  const onFinish = async (values: any) => {
    // console.log('Success:', values);
    // var rsp = await props.modelCreateSubmitApi({ ...values });
    // notification.info({
    //   message: `添加成功`,
    //   description: `添加成功`
    // });
    // let id = (pkFieldName !== undefined && rsp.data && rsp.data.hasOwnProperty(pkFieldName)) ? rsp.data[pkFieldName] : rsp.data;
    // navigate(`../edit/${id}`);
    var rsp = await webDomainUpsert({ ...values });
  };
  return (
    <PageHeader title={`Create New One`} onBack={() => navigate("../")}
      extra={<Space size="middle">
        <Button onClick={() => form.submit()}>Save</Button>
      </Space>}
    >
      <EditForm form={form} onFinish={onFinish} values={initialValues} {...props} />
    </PageHeader>
  );
};
