// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
import { Card, Space, theme } from 'antd';
import React, { useEffect, useState } from 'react';
import { Col, Row } from 'antd';
import { Column, Line } from '@ant-design/plots';
interface TypeReportProps {
  cycleType: number,
  typeName: string,
}
export const TypeDataReport: React.FC<TypeReportProps> = (props) => {
  const [data, setData] = useState([]);
  // const { cycleType } = props;
  const [cycleType, setCycleType] = useState(props.cycleType);
  const asyncFetch = () => {
    fetch('/yaginx/api/resource/report/type_report_data?cycleType=' + cycleType + "&type=" + props.typeName)
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
    xField: 'time',
    yField: 'value',
    seriesField: 'type',
    legend: {
      position: 'top',
    },
    // xAxis: {
    //   type: 'time',
    // },
    // yAxis: {
    //   label: {
    //     // 数值格式化为千分位
    //     formatter: (v) => `${v}`.replace(/\d{1,3}(?=(\d{3})+$)/g, (s) => `${s},`),
    //   },
    // },
  };
  return (
    <>
      <Line {...config} />
    </>
  )
};
