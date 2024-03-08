// pages/Home/index.tsx
import { Card } from 'antd';
import React, { useState } from 'react';
import { Col, Row } from 'antd';
import { RequestQtyDiagram } from './RequestQty';
import { ParentModel } from './ParentModel';

const Home: React.FC = () => {
  const tabRequestQtyDiagramList = [
    {
      key: 'minutly',
      tab: '分钟图',
    },
    {
      key: 'hourly',
      tab: '小时图',
    },
    {
      key: 'daily',
      tab: '日线图',
    }
  ];
  const [requsetQtyDiagramCycleType, setRequestQtyDiagramCycleType] = useState(5);
  const onTabRequestQtyTabChange = (key: string) => {
    switch (key) {
      case "minutly":
        setRequestQtyDiagramCycleType(5);
        break;
      case "hourly":
        setRequestQtyDiagramCycleType(10);
        break;
      case "daily":
        setRequestQtyDiagramCycleType(20);
        break;
      default:
        break;
    }
  };

  return (
    <>
      <Row gutter={[16, 16]} style={{ marginBottom: "24px" }}>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>}
            actions={[<>Card content</>]}>
            Card content
          </Card>
        </Col>
        <Col span={6}>
          <Card title="Default size card" extra={<a href="#">More</a>}  >
            <ParentModel></ParentModel>
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
      {/* <Row style={{ marginBottom: "24px" }}>
        <Col span={24}>
          <Card title="请求量图" extra={<a href="#">More</a>} >
            <RequestQtyDiagram cycleType={5} />
          </Card>
        </Col>
      </Row> */}
      <Row style={{ marginBottom: "24px" }}>
        <Col span={24}>
          <Card
            title="请求量分时图"
            tabList={tabRequestQtyDiagramList}
            defaultActiveTabKey='minutly'
            onTabChange={onTabRequestQtyTabChange}>
            <RequestQtyDiagram cycleType={requsetQtyDiagramCycleType} />
          </Card>
        </Col>
      </Row>
    </>
  )
};

export default Home;

