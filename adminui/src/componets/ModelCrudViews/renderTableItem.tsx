import React from 'react';
import ColumnSearchProps from '@/componets/ColumnSearchProps';
import { LocalTime } from '../LocalTime';
import { Checkbox } from 'antd';


export const renderTableItem = (fieldName: string, title: string = fieldName) => {
  switch (fieldName) {
    case "id":
      // 主键上面强制输出,碰到之后不做处理
      break;
    case "metaDic":
      return { title: title, key: fieldName, dataIndex: fieldName, render: (text: any, record: any) => <span>{record.metaDic?.length ?? 0}</span> };
      break;
    case "createTime":
    case "updateTime":
    case "deleteTime":
      return { title: title, key: fieldName, dataIndex: fieldName, render: (text: any, record: any) => <LocalTime timestamp={record[fieldName]} /> };
    default:
      return {
        title: title, dataIndex: fieldName, key: fieldName, align: 'center', ...ColumnSearchProps(fieldName, fieldName),
        render: (text: any, record: any) => {
          switch (typeof record[fieldName]) {
            case "boolean":
              return <Checkbox checked={record[fieldName]} disabled />;
            case "string":
              return record[fieldName];
            default:
              return record[fieldName].toString();
          }
        }
      };
  }
};
