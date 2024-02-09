// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
import { Card, Space, theme } from 'antd';
import React, { useEffect, useState } from 'react';
import { Col, Row } from 'antd';
import { Line } from '@ant-design/plots';

export const RequestQtyDiagram: React.FC = () => {
  const [data, setData] = useState([]);
  const asyncFetch = () => {
    fetch('/api/resource/report/all_report_data?cycleType=5')
      .then((response) => response.json())
      .then((json) => setData(json.data))
      .catch((error) => {
        console.log('fetch data failed', error);
      });
  };
  useEffect(() => {
    asyncFetch();
  }, []);

  const config = {
    data,
    padding: 'auto',
    xField: 'time',
    yField: 'scales',
    xAxis: {
      // type: 'timeCat',
      tickCount: 1,
    },
  };
  return (
    <>
      <Line {...config} />
    </>
  )
};
