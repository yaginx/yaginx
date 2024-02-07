import React, { useEffect, useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate, useParams } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { Form } from 'antd';
import { ModelEditForm } from './ModelEditForm';
import { IModelEditViewProps } from './ModelCrudProps';

export const ModelEditView: React.FC<IModelEditViewProps> = (props) => {
  const { modelName, pkFieldName, displayFieldName: displayKeyName, fields } = props;
  let { id } = useParams<any>();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [initialValues, setInitialValues] = useState<any>({ id });

  const onFinish = async (values: any) => {
    console.log('Success:', values);
    await props.modelEditSubmitApi({ ...values });
    notification.info({
      message: `更新成功`,
      description: `更新成功`
    });
  };

  const get = async () => {
    var result = await props.modelGetApi({ id });
    var newValue = { ...initialValues, ...result.data };
    setInitialValues(newValue);
    form.setFieldsValue(newValue);
  };
  useEffect(() => {
    get();
  }, []);

  return (
    <PageHeader title={`${modelName}: [#${initialValues[pkFieldName]}]-${initialValues[displayKeyName]}`} onBack={() => navigate("../")}
      extra={<Space size="middle">
        <Button onClick={() => form.submit()}>Save</Button>
        <Button onClick={get}>Refresh</Button>
      </Space>}
    >
      <ModelEditForm form={form} onFinish={onFinish} values={initialValues} {...props} />
    </PageHeader>
  );
};
