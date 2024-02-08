import { IApiRspEnvelop } from "@/api/apiTypes"
import { TableOperationMenuItem, TableOperations } from "@/componets/TableOperationMenus"
import { Table } from "antd"
import React, { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"

export interface ITableListProps {
  renderTableOperationMenu?: (record: any) => TableOperationMenuItem[];
  onTableMenuClick?: (commandKey: string, record: any) => void;
  initTableColumn: (columnsArray: any[]) => void;
  realodTableData: number;
  searchAction: (data: any) => Promise<IApiRspEnvelop<any>>
}
const TableList: React.FC<ITableListProps> = (props: ITableListProps) => {
  const pkFieldName = "id";
  const { realodTableData: externalRefreshTable } = props;
  const navigate = useNavigate();
  const [internalRefreshTable, forceRefreshTable] = useState<number>(0);
  const [columns, setColumns] = useState<any>([]);
  const [dataSource, setDataSource] = useState<any>([]);
  const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500"] });
  const [filterCondition, setFilterCondition] = useState<any>({});

  useEffect(() => {
    fetchTableData();
  }, [externalRefreshTable, internalRefreshTable, filterCondition]);

  // 设置table column
  useEffect(() => { setColumns(initTableColumn()); }, []);

  const initTableColumn = () => {
    let columnsArray: any[] = [];
    columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });

    props.initTableColumn(columnsArray);

    if (props.renderTableOperationMenu && props.onTableMenuClick)
      columnsArray.push({
        title: '操作', key: 'menuOperations', dataIndex: pkFieldName, width: 150,
        render: (text: any, record: any) => <><TableOperations record={record} menuOptions={props.renderTableOperationMenu(record)} onMenuClick={props.onTableMenuClick} onComplete={() => forceRefreshTable(Math.random() + 1)} /></>
      })
    return columnsArray;
  }

  const fetchTableData = async () => {
    var rsp = await props.searchAction({ ...pagination, ...filterCondition })
    const records = rsp.data;
    let dataStoreArray: any = [];
    records.forEach((item: any, index: number) => { dataStoreArray.push({ ...item, key: item[pkFieldName], rowIndex: index + 1 }); });
    setDataSource(dataStoreArray);
  }

  const onTableChange = (changedPagination: any, filters: any) => {
    setPagination({ ...changedPagination });
    setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
  }

  return (
    <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
  )
}
export default TableList;
