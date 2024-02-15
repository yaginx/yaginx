import { TableOperationMenuItem } from "@/componets/TableOperationMenus"
import { PageHeader } from "@ant-design/pro-components"
import { Space, Button } from "antd"
import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import { webDomainDelete, webDomainSearch, websiteSearch } from "@/api/docker"
import TableList from "@/componets/TableList"
import { renderTableItem } from "@/componets/ModelCrudViews/renderTableItem"

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
        webDomainDelete({ id: pkValue });
        break;
      default:
        break;
    }
  }

  const initTableColumn = (columnsArray: any[]) => {
    columnsArray.push(renderTableItem("name", "域名"));
    columnsArray.push(renderTableItem("isUseFreeCert", "使用免费证书"));
    columnsArray.push(renderTableItem("isHaveCert", "证书是否存在"));
    columnsArray.push(renderTableItem("isVerified", "是否已验证"));
  }

  return (
    <PageHeader title={"域名"} tags={
      <Space size="middle">
        <Button key="refresh" onClick={() => forceRefreshTable(Math.random() + 1)} >Refresh</Button>
        <Button key="create" onClick={() => navigate("../create")} >Create</Button>
      </Space>
    }>
      <TableList searchAction={webDomainSearch} realodTableData={refreshTable}
        initTableColumn={initTableColumn}
        renderTableOperationMenu={renderTableOperationMenu}
        onTableMenuClick={onTableMenuClick} />
    </PageHeader>
  )
}
export default List;
