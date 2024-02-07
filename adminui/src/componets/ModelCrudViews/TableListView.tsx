import React, { useEffect, useState } from 'react';
import { Button, Space, Switch, Table, Tag, notification } from 'antd';
import { Link, useNavigate } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import ColumnSearchProps from '@/componets/ColumnSearchProps';
import { ISchemaModelInfo, ITableListViewProps } from './ModelCrudProps';
import { TableOperationMenuItem, TableOperations } from '../TableOperationMenus';
import { renderTableItem } from './renderTableItem';
import { v4 as uuidv4 } from 'uuid';

export const TableListView: React.FC<ITableListViewProps & ISchemaModelInfo> = (props) => {
  const { tableDataFetchApi, tableDataDeleteApi, revokeDelete, pkFieldName, modelName, fields } = props;
  const navigate = useNavigate();
  const [refreshTable, forceRefreshTable] = useState<number>(0);
  const [columns, setColumns] = useState<any>([]);
  const [dataSource, setDataSource] = useState<any>([]);
  const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500"] });
  const [filterCondition, setFilterCondition] = useState<any>({});
  const [isIncludeDeleted, setIsIncludeDeleted] = useState<boolean>(false);
  const renderTableOperationMenu: (record: any) => TableOperationMenuItem[] = (record: any) => {
    var tempItems: TableOperationMenuItem[] = [];
    tempItems.push({ commandKey: "edit", commandLabel: "编辑" });

    if (tableDataDeleteApi) {
      if (record.hasOwnProperty("isDeleted")) {
        if (!record.isDeleted) {
          tempItems.push({ commandKey: "delete", commandLabel: "删除" });
        }
        else if (revokeDelete) {
          tempItems.push({ commandKey: "revokeDelete", commandLabel: "撤销删除" });
        }
      }
    }
    return tempItems;
  }

  const initTableColumn = () => {
    let columnsArray: any = [];
    columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });

    let fieldName = pkFieldName ? pkFieldName : "id";
    columnsArray.push({
      title: fieldName, key: fieldName, dataIndex: fieldName, ellipsis: true, ...ColumnSearchProps("title", "title"),
      render: (text: any, record: any) => (<Link key="edit" to={`../edit/${record[fieldName]}`} title={text}>{text}</Link>)
    });

    fields?.forEach((item) => {
      let { name: fieldName } = item;
      if (fieldName === pkFieldName)
        return;
      columnsArray.push(renderTableItem(fieldName))
    })
    if (isIncludeDeleted) {
      columnsArray.push({
        title: "isDeleted", dataIndex: "isDeleted", key: "isDeleted", align: 'center', ...ColumnSearchProps("isDeleted", "isDeleted"),
        render: (text: any, record: any) => record.isDeleted === true ? <Tag color="#cd201f">Deleted</Tag> : <></>
      });
    }
    columnsArray.push({
      title: '操作', key: 'menuOperations', dataIndex: pkFieldName, width: 150,
      render: (text: any, record: any) => <><TableOperations record={record} menuOptions={renderTableOperationMenu(record)} onMenuClick={onTableMenuClick} onComplete={() => forceRefreshTable(Math.random() + 1)} /></>
    })
    return columnsArray;
  };

  const fetchTableData = async () => {
    var rsp = await tableDataFetchApi({ ...pagination, ...filterCondition, isDeleted: isIncludeDeleted, });
    const { records, paging } = rsp.data;

    // 从API返回的分页信息重新设置, 约定: 当客户端传到服务器端的分页信息不准确的时候，服务器端返回给客户端的分页信息会做调整
    setPagination({ ...pagination, current: paging.pageIndex, pageSize: paging?.pageSize, total: paging?.total });

    let dataStoreArray: any = [];
    records.map((item: any, index: number) => { dataStoreArray.push({ ...item, key: uuidv4(), rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
    setDataSource(dataStoreArray);
  };

  const onTableChange = (changedPagination: any, filters: any) => {
    setPagination({ ...changedPagination });
    setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current });
  };

  const onTableMenuClick = async (commandKey: string, record: any) => {
    const pkValue = record[pkFieldName];
    switch (commandKey) {
      case "delete":
        if (tableDataDeleteApi) {
          await tableDataDeleteApi?.call(this, { id: pkValue });
          forceRefreshTable(Math.random());
        }
        break;
      case "revokeDelete":
        if (revokeDelete) {
          await revokeDelete({ id: pkValue })
        }
        break;
      case "edit":
        navigate(`../edit/${pkValue}`);
        break;
      default:
        notification.info({
          message: `错误提示`,
          description: `没有处理的操作, commandKey:${commandKey}`
        });
        break;
    }
  }

  // 设置table column
  useEffect(() => {
    if (fields && fields?.length > 0)
      setColumns(initTableColumn());
  }, [fields, pkFieldName, isIncludeDeleted]);

  useEffect(() => {
    fetchTableData();
  }, [refreshTable, isIncludeDeleted]);

  return (
    <PageHeader title={`${modelName}`} tags={<Space size="middle">
      <Button key="refresh" onClick={async () => await fetchTableData()}>Refresh</Button>
      <Button key="create" onClick={() => navigate('../create')}>Create</Button>
      <Switch checkedChildren="已删除" unCheckedChildren="已删除" defaultChecked={isIncludeDeleted} onChange={(result, event) => setIsIncludeDeleted(result)} />
    </Space>}>
      <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
    </PageHeader>
  );
};
