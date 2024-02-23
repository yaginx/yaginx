import React, { useEffect, useState } from 'react';
import { Button, Col, Form, Input, Row, Space, Switch, Tabs } from 'antd';
import { formItemDefaultLayout } from '@/componets/formItemDefaultLayout';
import { CloseOutlined, MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';

export const EditForm: React.FC<any> = (props) => {
  const [tabItems, setTabItems] = useState<any[]>([]);

  const buildTabItems = () => {
    const tabItems: any[] = [
      {
        label: '基本信息', key: 'tabBasicInfo', forceRender: true, children: <>
          <Row gutter={[8, 8]}>
            <Col span={8}>
              <Form.Item name="name" label="名称" rules={[{ required: true, message: '标题' }]}>
                <Input />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="defaultHost" label="defaultHost">
                <Input />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="defaultDestination" label="defaultDestination">
                <Input />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="defaultDestinationHost" label="defaultDestinationHost">
                <Input />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="webProxy" label="webProxy">
                <Input />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={[8, 8]}>
            <Col span={8}>
              <Form.Item name="isWithOriginalHostHeader" label="是否携带源主机头(Host)" valuePropName="checked">
                <Switch checkedChildren="携带源主机头" unCheckedChildren="不携带源主机头" />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="isAllowUnsafeSslCertificate" label="是否检查SSL证书有效性" valuePropName="checked">
                <Switch checkedChildren="忽略SSL证书检查" unCheckedChildren="严格检查SSL证书有效性" />
              </Form.Item>
            </Col> 
             <Col span={8}>
              <Form.Item name="isAutoRedirectHttp2Https" label="Http自动跳转到Https" valuePropName="checked">
                <Switch checkedChildren="自动跳转" unCheckedChildren="不自动跳转" />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={[8, 8]}>
            <Col span={24}>
              <Form.Item label="转发配置" key={"proxyTransforms"}>
                <Form.List name="proxyTransforms">
                  {(fields, { add, remove }) => (
                    <>
                      {fields.map(({ key, name, ...restField }) => (
                        <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                          <Form.Item
                            {...restField}
                            name={[name, 'key']}
                            rules={[{ required: true, message: 'Missing key' }]}
                          >
                            <Input placeholder="属性" style={{}} />
                          </Form.Item>
                          <Form.Item
                            {...restField}
                            name={[name, 'value']}
                            rules={[{ required: true, message: 'Missing value' }]}>
                            <Input placeholder="值" style={{ width: "400px" }} />
                          </Form.Item>
                          <MinusCircleOutlined onClick={() => remove(name)} />
                        </Space>
                      ))}
                      <Form.Item>
                        <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                          添加转发配置
                        </Button>
                      </Form.Item>
                    </>
                  )}
                </Form.List>
              </Form.Item>
            </Col>
            <Col span={24}>
              <Form.Item label="转发规则" key={"proxyRules"}>
                <Form.List name="proxyRules">
                  {(fields, { add, remove }) => (
                    <>
                      {fields.map(({ key, name, ...restField }) => (
                        <Space key={key} style={{ display: 'flex', marginBottom: 8 }} align="baseline">
                          <Form.Item
                            {...restField}
                            name={[name, 'pathPattern']}
                            rules={[{ required: true, message: 'Missing key' }]}
                          >
                            <Input placeholder="规则名称" style={{}} />
                          </Form.Item>
                          <Form.Item>
                            <Form.List name={[name, 'destinations']}>
                              {(subFields, subOpt) => (
                                <div style={{ display: 'flex', flexDirection: 'column', rowGap: 16 }}>
                                  {subFields.map((subField) => (
                                    <Space key={subField.key}>
                                      <Form.Item noStyle name={[subField.name, 'name']}>
                                        <Input placeholder="目标名称" />
                                      </Form.Item>
                                      <Form.Item noStyle name={[subField.name, 'address']}>
                                        <Input placeholder="目标地址" />
                                      </Form.Item>
                                      <CloseOutlined
                                        onClick={() => {
                                          subOpt.remove(subField.name);
                                        }}
                                      />
                                    </Space>
                                  ))}
                                  <Button type="dashed" onClick={() => subOpt.add()} block>
                                    + 添加目标地址
                                  </Button>
                                </div>
                              )}
                            </Form.List>
                          </Form.Item>
                          <MinusCircleOutlined onClick={() => remove(name)} />
                        </Space>
                      ))}
                      <Form.Item>
                        <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                          添加规则
                        </Button>
                      </Form.Item>
                    </>
                  )}
                </Form.List>
              </Form.Item>
            </Col>
          </Row>
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
