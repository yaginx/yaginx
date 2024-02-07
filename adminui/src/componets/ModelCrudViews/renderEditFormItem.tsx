import React from 'react';
import { Button, Col, Form, Input, Space } from 'antd';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';


export const renderEditFormItem = (fieldName: string) => {
  switch (fieldName) {
    case "metaDic":
      return (
        <Form.Item label="元数据" key={"metaDic"}>
          <Form.List name="metaDic">
            {(fields, { add, remove }) => (
              <>
                {fields.map(({ key, name, ...restField }) => (
                  <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                    <Form.Item
                      {...restField}
                      name={[name, 'key']}
                      rules={[{ required: true, message: 'Missing key' }]}
                    >
                      <Input placeholder="Key" style={{}} />
                    </Form.Item>
                    <Form.Item
                      {...restField}
                      name={[name, 'value']}
                      rules={[{ required: true, message: 'Missing value' }]}
                    >
                      <Input placeholder="value" style={{ width: "400px" }} />
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
      );
    default:
      return (
        <Col key={"key" + fieldName} span={8}>
          <Form.Item label={fieldName} name={fieldName}><Input /></Form.Item>
        </Col>
      );
  }
};
