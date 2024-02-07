// pages/Home/index.tsx
import React, { useState } from 'react';
import { Button, Space, notification } from 'antd';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '@ant-design/pro-layout';
import { TableOperationMenuItem } from '@/componets/TableOperationMenus';
import { renderTableItem } from '@/componets/ModelCrudViews/renderTableItem';
import { containerSearch } from '@/api/docker';
import TablePagedList from '@/componets/PagedTableList';
import TableList from '@/componets/TableList';
// import { TenantCreateModal } from './TenantCreateModal';

const ContainerList: React.FC = (props) => {
  const pkFieldName = "tenantId";
  const navigate = useNavigate();
  const [refreshTable, forceRefreshTable] = useState<number>(0);
  const [editModalFormVisiable, setEditModalFormVisiable] = useState(false);
  const renderTableOperationMenu: (record: any) => TableOperationMenuItem[] = (record: any) => {
    var tempItems: TableOperationMenuItem[] = [];
    tempItems.push({ commandKey: "detail", commandLabel: "查看" });
    tempItems.push({ commandKey: "edit", commandLabel: "编辑" });

    if (record.hasOwnProperty("isDeleted")) {
      if (!record.isDeleted) {
        tempItems.push({ commandKey: "delete", commandLabel: "删除" });
      }
      // else if (revokeDelete) {
      //   tempItems.push({ commandKey: "revokeDelete", commandLabel: "撤销删除" });
      // }
    }
    return tempItems;
  }
  const onTableMenuClick = async (commandKey: string, record: any) => {
    const pkValue = record[pkFieldName];
    switch (commandKey) {
      case "detail":
        navigate(`../detail/${pkValue}`);
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
  const initTableColumn = (columnsArray: any[]) => {
    let generalFields: string[] = ["name", "status", "state", "created", "ports", "network", "mounts"];
    generalFields?.forEach((fieldName) => {
      if (fieldName === pkFieldName)
        return;
      columnsArray.push(renderTableItem(fieldName))
    })
  }

  return (
    <PageHeader title={"Containers"} tags={
      <Space size="middle">
        <Button key="refresh" onClick={() => forceRefreshTable(Math.random() + 1)} >Refresh</Button>
        <Button key="create" onClick={() => setEditModalFormVisiable(true)} >Create</Button>
      </Space>
    }>
      <h2>WebsiteList</h2>
      <TableList searchAction={containerSearch} realodTableData={refreshTable} initTableColumn={initTableColumn} renderTableOperationMenu={renderTableOperationMenu} onTableMenuClick={onTableMenuClick} />
    </PageHeader>
  )
}

export default ContainerList;
