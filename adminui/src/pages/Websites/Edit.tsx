import React, { useEffect, useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate, useParams } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { Form } from 'antd';
import { EditForm } from './EditForm';
import { websiteGet, websiteUpsert } from '@/api/docker';

export const Edit: React.FC = (props) => {
  let { id } = useParams<any>();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [initialValues, setInitialValues] = useState<any>({});
  const onFinish = async (values: any) => {
    // console.log('Success:', values);
    // var rsp = await props.modelCreateSubmitApi({ ...values });

    // let id = (pkFieldName !== undefined && rsp.data && rsp.data.hasOwnProperty(pkFieldName)) ? rsp.data[pkFieldName] : rsp.data;

    var rsp = await websiteUpsert({ ...values });
    notification.info({
      message: `添加成功`,
      description: `添加成功`
    });
    let id = (rsp !== undefined && rsp.data && rsp.data.hasOwnProperty("id")) ? rsp.data["id"] : rsp.data;
    navigate(`../edit/${id}`);
  };

  const get = async () => {
    var result = await websiteGet({ id });
    var newValue = { ...initialValues, ...result.data };
    setInitialValues(newValue);
    form.setFieldsValue(newValue);
  };
  useEffect(() => {
    get();
  }, []);
  return (
    <PageHeader title={`Website: [#${initialValues["name"]}]`} onBack={() => navigate("../")}
      extra={<Space size="middle">
        <Button onClick={() => form.submit()}>Save</Button>
        <Button onClick={get}>Refresh</Button>
      </Space>}
    >
      <EditForm form={form} onFinish={onFinish} values={initialValues} {...props} />
    </PageHeader>
  );
};
