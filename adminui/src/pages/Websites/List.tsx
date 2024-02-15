import { TableOperationMenuItem } from "@/componets/TableOperationMenus"
import { PageHeader } from "@ant-design/pro-components"
import { Space, Button } from "antd"
import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import TableList from "@/componets/TableList"
import { websiteDelete, websiteSearch } from "@/api/docker"

const List: React.FC = (props) => {
  const pkFieldName = "id";

  const navigate = useNavigate();
  const [refreshTable, forceRefreshTable] = useState<number>(0);
  const [editModalFormVisiable, setEditModalFormVisiable] = useState(false);

  const renderTableOperationMenu: (record: any) => TableOperationMenuItem[] = (record: any) => {
    var tempItems: TableOperationMenuItem[] = [];
    // tempItems.push({ commandKey: "detail", commandLabel: "查看" });
    tempItems.push({ commandKey: "edit", commandLabel: "编辑" });
    tempItems.push({ commandKey: "delete", commandLabel: "删除" });
    return tempItems;
  }
  const onTableMenuClick = (commandKey: string, record: any) => {
    const pkValue = record[pkFieldName];
    switch (commandKey) {
      // case "detail":
      //   navigate(`../detail/${pkValue}`);
      //   break;
      case "edit":
        navigate(`../edit/${pkValue}`);
        break;
      case "delete":
        websiteDelete({ id: pkValue });
        break;
      default:
        break;
    }
  }

  const initTableColumn = (columnsArray: any[]) => {
    columnsArray.push({ title: 'name', key: 'name', width: 150, dataIndex: "name" })
    // columnsArray.push({
    //   title: 'domain', key: 'domain', width: 150,
    //   render: (text: any, record: any) => record.hosts.map((element: any) => <>{element?.domain}</>)
    // })
    // columnsArray.push({
    //   title: 'rule', key: 'rule', width: 150,
    //   render: (text: any, record: any) => record.proxyRules.map((element: any) => <>{element?.pathPattern}</>)
    // })
  }

  return (
    <PageHeader title={"Websites"} tags={
      <Space size="middle">
        <Button key="refresh" onClick={() => forceRefreshTable(Math.random() + 1)} >Refresh</Button>
        <Button key="create" onClick={() => navigate("../create")} >Create</Button>
      </Space>
    }>
      <TableList searchAction={websiteSearch} realodTableData={refreshTable} initTableColumn={initTableColumn}
        renderTableOperationMenu={renderTableOperationMenu}
        onTableMenuClick={onTableMenuClick}
      />
    </PageHeader>
  )
}
export default List;
