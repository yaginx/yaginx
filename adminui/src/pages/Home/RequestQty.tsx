// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
import { Card, Space, theme } from 'antd';
import React, { useEffect, useState } from 'react';
import { Col, Row } from 'antd';
import { Line } from '@ant-design/plots';
interface RequestQtyDiagramProps {
  cycleType: number
}
export const RequestQtyDiagram: React.FC<RequestQtyDiagramProps> = (props) => {
  const [data, setData] = useState([]);
  // const { cycleType } = props;
  const [cycleType, setCycleType] = useState(props.cycleType);
  const asyncFetch = () => {
    fetch('/api/resource/report/all_report_data?cycleType=' + cycleType)
      .then((response) => response.json())
      .then((json) => setData(json.data))
      .catch((error) => {
        console.log('fetch data failed', error);
      });
  };
  useEffect(() => {
    asyncFetch();
  }, [cycleType]);

  useEffect(() => {
    setCycleType(props.cycleType);
  }, [props]);

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
