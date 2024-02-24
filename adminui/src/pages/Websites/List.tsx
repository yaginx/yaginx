import { TableOperationMenuItem } from "@/componets/TableOperationMenus"
import { PageHeader } from "@ant-design/pro-components"
import { Space, Button, UploadProps, message, Upload } from "antd"
import React, { useState } from "react"
import { useNavigate } from "react-router-dom"
import TableList from "@/componets/TableList"
import { websiteDelete, websiteSearch } from "@/api/docker"
import { renderTableItem } from "@/componets/ModelCrudViews/renderTableItem"
import { DownloadOutlined, UploadOutlined } from "@ant-design/icons"

const List: React.FC = (props) => {
  const pkFieldName = "id";

  const navigate = useNavigate();
  const [refreshTable, forceRefreshTable] = useState<number>(0);

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
    columnsArray.push({ title: 'name', key: 'name', width: 150, dataIndex: "name" });
    columnsArray.push({ title: 'defaultHost', key: 'defaultHost', width: 150, dataIndex: "defaultHost" })
    columnsArray.push({ title: 'defaultDestination', key: 'defaultDestination', dataIndex: "defaultDestination" })
    columnsArray.push(renderTableItem("isHaveSslCert", "证书是否存在"));
    // columnsArray.push({
    //   title: 'domain', key: 'domain', width: 150,
    //   render: (text: any, record: any) => record.hosts.map((element: any) => <>{element?.domain}</>)
    // })
    // columnsArray.push({
    //   title: 'rule', key: 'rule', width: 150,
    //   render: (text: any, record: any) => record.proxyRules.map((element: any) => <>{element?.pathPattern}</>)
    // })
  }
  const downloadConfig = async () => {
    const newTab = window.open('/api/website/config/backup', '_blank');
    newTab?.focus();
    message.success(`网站配置备份成功`);
  };
  const uploadProps: UploadProps = {
    name: 'file',
    action: '/api/website/config/restore',
    // headers: {
    //   authorization: 'authorization-text',
    // },
    showUploadList: false,
    onChange(info) {
      if (info.file.status !== 'uploading') {
        console.log(info.file, info.fileList);
      }
      if (info.file.status === 'done') {
        forceRefreshTable(Math.random() + 1)
        message.success(`${info.file.name} file uploaded successfully`);
      } else if (info.file.status === 'error') {
        message.error(`${info.file.name} file upload failed.`);
      }
    },
  };
  return (
    <PageHeader title={"站点管理"} tags={
      <Space size="middle">
        <Button key="refresh" onClick={() => forceRefreshTable(Math.random() + 1)} >Refresh</Button>
        <Button key="create" onClick={() => navigate("../create")} >Create</Button>
      </Space>
    } extra={
      <Space size="middle">
        <Button icon={<DownloadOutlined />} onClick={downloadConfig}>备份配置</Button>
        <Upload {...uploadProps}>
          <Button icon={<UploadOutlined />}>恢复配置</Button>
        </Upload>
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
