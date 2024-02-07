import React from 'react';
import ColumnSearchProps from '@/componets/ColumnSearchProps';
import { LocalTime } from '../LocalTime';


export const renderTableItem = (fieldName: string) => {
  switch (fieldName) {
    case "id":
      // 主键上面强制输出,碰到之后不做处理
      break;
    case "metaDic":
      return { title: fieldName, key: fieldName, dataIndex: fieldName, render: (text: any, record: any) => <span>{record.metaDic?.length ?? 0}</span> };
      break;
    case "createTime":
    case "updateTime":
    case "deleteTime":
      return { title: fieldName, key: fieldName, dataIndex: fieldName, render: (text: any, record: any) => <LocalTime timestamp={record[fieldName]} /> };
    default:
      return { title: fieldName, dataIndex: fieldName, key: fieldName, align: 'center',  ...ColumnSearchProps(fieldName, fieldName) };
  }
};
