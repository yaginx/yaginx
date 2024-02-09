// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
import { Card, Space, theme } from 'antd';
import React, { useEffect, useState } from 'react';
import { Col, Row } from 'antd';
import { Line } from '@ant-design/plots';
import { RequestQtyDiagram } from './RequestQty';

// 组件绑定
function BearCounter() {
  const bears = bearStore((state) => state.bears)
  return <h1>{bears} around here ...</h1>
}

function Controls() {
  const increasePopulation = bearStore((state) => state.increasePopulation)
  return <button onClick={increasePopulation}>one up</button>
}

const Home: React.FC = () => {
  const [data, setData] = useState([]);
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const asyncFetch = () => {
    fetch('https://gw.alipayobjects.com/os/bmw-prod/55424a73-7cb8-4f79-b60d-3ab627ac5698.json')
      .then((response) => response.json())
      .then((json) => setData(json))
      .catch((error) => {
        console.log('fetch data failed', error);
      });
  };
  useEffect(() => {
    asyncFetch();
  }, []);

  const config = {
    data,
    xField: 'year',
    yField: 'value',
    seriesField: 'category',
    yAxis: {
      label: {
        // 数值格式化为千分位
        formatter: (v: any) => `${v}`.replace(/\d{1,3}(?=(\d{3})+$)/g, (s) => `${s},`),
      },
    },
    color: ['#1979C9', '#D62A0D', '#FAA219'],
  };
  const tabList = [
    {
      key: 'tab1',
      tab: 'tab1',
    },
    {
      key: 'tab2',
      tab: 'tab2',
    },
  ];
  const [activeTabKey1, setActiveTabKey1] = useState<string>('tab1');
  const onTab1Change = (key: string) => {
    setActiveTabKey1(key);
  };
  const contentList: Record<string, React.ReactNode> = {
    tab1: <>
      <Row>
        <Col span={16}>
          <p>content1</p>
          <Line {...config} />
        </Col>
        <Col span={8}>
        </Col>
      </Row>

    </>,
    tab2: <>
      <p>content2</p>
      <Line {...config} />
    </>,
  };
  return (
    <>
      <Row gutter={[16, 16]} style={{ marginBottom: "24px" }}>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>}
            actions={[
              <>日销售额￥12,423</>
            ]}>
            Card content
          </Card>
        </Col>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>}  >
            Card content
          </Card>
        </Col>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>} >
            Card content
          </Card>
        </Col>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>} >
            Card content
          </Card>
        </Col>
      </Row>
      <Row style={{ marginBottom: "24px" }}>
        <Col span={24}>
          <Card title="请求量图" extra={<a href="#">More</a>} >
            <RequestQtyDiagram />
          </Card>
        </Col>
      </Row>
      <Row style={{ marginBottom: "24px" }}>
        <Col span={24}>
          <Card
            // style={{ width: '100%' }}
            // title="Card title"
            // extra={<a href="#">More</a>}
            tabList={tabList}
            activeTabKey={activeTabKey1}
            onTabChange={onTab1Change}
          >
            {contentList[activeTabKey1]}
          </Card>
        </Col>
      </Row>
    </>
  )
};

export default Home;

