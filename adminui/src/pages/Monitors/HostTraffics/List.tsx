import { TableOperationMenuItem } from "@/componets/TableOperationMenus"
import { PageHeader } from "@ant-design/pro-components"
import { Space, Button } from "antd"
import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { hostTrafficSearch, webDomainSearch, websiteSearch } from "@/api/docker"
import TableList from "@/componets/TableList"
import { renderTableItem } from "@/componets/ModelCrudViews/renderTableItem"
import { LocalTime } from "@/componets/LocalTime"

const List: React.FC = (props) => {
  const pkFieldName = "host";

  const navigate = useNavigate();
  const [refreshTable, forceRefreshTable] = useState<number>(0);

  const renderTableOperationMenu: (record: any) => TableOperationMenuItem[] = (record: any) => {
    var tempItems: TableOperationMenuItem[] = [];
    // tempItems.push({ commandKey: "detail", commandLabel: "查看" });
    tempItems.push({ commandKey: "edit", commandLabel: "编辑" });
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
      default:
        break;
    }
  }

  const initTableColumn = (columnsArray: any[]) => {
    columnsArray.push(renderTableItem("hostName", "主机头"));
    columnsArray.push({ title: "周期", key: "period", dataIndex: "period", render: (text: any, record: any) => <LocalTime timestamp={record["period"]} /> });
    columnsArray.push(renderTableItem("requestCounts", "请求次数"));
    columnsArray.push(renderTableItem("inboundBytes", "流入流量"));
    columnsArray.push(renderTableItem("outboundBytes", "流出流量"));
  }

  return (
    <PageHeader title={"主机流量"} tags={
      <Space size="middle">
        <Button key="refresh" onClick={() => forceRefreshTable(Math.random() + 1)} >Refresh</Button>
        {/* <Button key="create" onClick={() => navigate("../create")} >Create</Button> */}
      </Space>
    }>
      <TableList searchAction={hostTrafficSearch} realodTableData={refreshTable}
        initTableColumn={initTableColumn}
      // renderTableOperationMenu={renderTableOperationMenu}
      // onTableMenuClick={onTableMenuClick}
      />
    </PageHeader>
  )
}
export default List;
