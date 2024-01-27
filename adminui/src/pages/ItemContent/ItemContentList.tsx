// pages/Home/index.tsx
import React, { useEffect, useState } from 'react';
import { Button, Dropdown, Popconfirm, Space, Table } from 'antd';
import { itemContentCreate, itemContentDelete, itemContentModify, itemContentSearch, postDelete, postPublish, postRevokeDelete, postUrlRecordFix } from '@/api/post';
import { Link, useNavigate } from 'react-router-dom';
import { useAppStore } from '@/store/appStore';
import ColumnSearchProps from '../Category/ColumnSearchProps';
import { PageHeader } from '@ant-design/pro-layout';
import { EditForm } from './EditForm';

const ItemContentList: React.FC = (props: any) => {
    const { publishStatus } = props;
    const navigate = useNavigate();
    const [refreshTable, forceRefreshTable] = useState<Number>(0);
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500"] });
    const [filterCondition, setFilterCondition] = useState<any>({});
    const { currentSiteId } = useAppStore();

    // 新建与编辑的窗口控制属性
    const [createFormModalVisiable, setCreateFormModalVisiable] = useState(false);
    const [editModalFormVisiable, setEditModalFormVisiable] = useState(false);
    const [editContentId, setEditContentId] = useState<Number | null>(null);

    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });
        columnsArray.push({ title: 'Category', dataIndex: 'categoryName', key: 'categoryName', align: 'center', width: 300 });
        columnsArray.push({
            title: 'name', key: 'name', dataIndex: 'name', ...ColumnSearchProps("name", "名称"),
            render: (text: any, record: any) => (
                <Link key="edit" title={record.slug ?? "无备注"} to={"#"} onClick={() => showEditForm(record.contentId)} >{record.name}</Link>
            )
        })
        columnsArray.push({ title: 'SortOrder', dataIndex: 'sortOrder', key: 'sortOrder', align: 'center', width: 80 });
        columnsArray.push({
            title: '操作', key: 'delete', dataIndex: 'postId', width: 150,
            render: (text: any, record: any) => <><RowOperations record={record} onComplete={onOperationCompleted} /></>
        })
        return columnsArray;
    }
    const onOperationCompleted = (record: any) => {
        // fetchTableData();
        forceRefreshTable(Math.random());
    }
    const fetchTableData = async () => {
        var rsp = await itemContentSearch({ ...pagination, ...filterCondition })
        const { records, paging } = rsp.data;

        // 从API返回的分页信息重新设置, 约定: 当客户端传到服务器端的分页信息不准确的时候，服务器端返回给客户端的分页信息会做调整
        setPagination({ ...pagination, current: paging.pageIndex, pageSize: paging?.pageSize, total: paging?.total });

        let dataStoreArray: any = [];
        records.map((item: any, index: number) => { dataStoreArray.push({ ...item, key: item.contentId, rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
        setDataSource(dataStoreArray);
    }

    const onTableChange = (changedPagination: any, filters: any) => {
        setPagination({ ...changedPagination });
        setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
    }

    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, []);

    useEffect(() => {
        fetchTableData();
    }, [filterCondition, currentSiteId, publishStatus, refreshTable])

    const onEditSubmit = async (values: any) => {
        let rsp = values.contentId ? await itemContentModify(values) : await itemContentCreate(values);
        if (rsp.code === 200) {
            fetchTableData();
            setEditModalFormVisiable(false);
        }
    };

    const showEditForm = (contentId: Number | null = null) => {
        setEditContentId(contentId);
        setEditModalFormVisiable(true);
    }

    return (
        <PageHeader tags={
            <Space size="middle">
                <Button key="refresh" onClick={async () => await fetchTableData()} >Refresh</Button>
                <Button key="create" onClick={() => showEditForm()} >Create</Button>
            </Space>
        }>
            <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
            <EditForm
                open={editModalFormVisiable}
                onSubmit={onEditSubmit}
                contentId={editContentId}
                onCancel={() => { setEditModalFormVisiable(false); }}
            />
        </PageHeader>
    )
}

export default ItemContentList;

interface RowOperationProps {
    record: any,
    onComplete: (record: any) => void;
}
const RowOperations: React.FC<RowOperationProps> = (props: RowOperationProps) => {
    const { record } = props;
    const [items, setItems] = useState<any>([]);
    const [mainAction, setMainAction] = useState<any>({});
    const onMenuClick = async (e: any) => {
        switch (e.key) {
            case "delete":
                await itemContentDelete({ contentId: record.contentId });
                break;
            default:
                break;
        }
        if (props.onComplete)
            props.onComplete(record);
    };

    useEffect(() => {
        var tempItems = [];
        tempItems.push({ key: "delete", label: "删除" });
        tempItems.push({ key: "delete2", label: "删除" });
        tempItems.push({ key: "delete3", label: "删除" });
        setMainAction({ ...tempItems[0] });
        setItems(tempItems.splice(1, tempItems.length - 1));
    }, [record])
    return (
        // <Popconfirm
        //     title="Delete the task"
        //     description="Are you sure to delete this task?"
        //     okText="Yes"
        //     cancelText="No"
        // >
            
        // </Popconfirm>
        <Dropdown.Button menu={{ items, onClick: onMenuClick }} onClick={() => onMenuClick({ key: mainAction.key })}>{mainAction.label}</Dropdown.Button>

    )
}


