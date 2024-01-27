// pages/Home/index.tsx
import React, { useEffect, useState } from 'react';
import { Button, Dropdown, MenuProps, Space, Switch, Table, Tag } from 'antd';
import { postDelete, postPublish, postRevokeDelete, postSearch, postUrlRecordFix } from '@/api/post';
import { Link, useNavigate } from 'react-router-dom';
import { LocalTime } from '@/componets/LocalTime';
import { useAppStore } from '@/store/appStore';
import ColumnSearchProps from '../Category/ColumnSearchProps';
import { useCategory } from '@/hooks/useEnumStatus';
import { PageHeader } from '@ant-design/pro-layout';
interface PostOperationProps {
    record: any,
    onComplete: (record: any) => void;
}
const PostOperations: React.FC<PostOperationProps> = (props: PostOperationProps) => {
    const { record } = props;
    const [items, setItems] = useState<any>([]);
    const [mainAction, setMainAction] = useState<any>({});
    const onMenuClick = async (e: any) => {
        switch (e.key) {
            case "publish":
                await postPublish({ postId: record.postId, isPublished: true });
                break;
            case "unpublish":
                await postPublish({ postId: record.postId, isPublished: false });
                break;
            case "delete":
                await postDelete({ postId: record.postId });
                break;
            case "revokeDelete":
                await postRevokeDelete(record.postId);
                break;
            case "urlRecordFix":
                await postUrlRecordFix({ postId: Number(record.postId) });
                break;
            default:
                break;
        }
        if (props.onComplete)
            props.onComplete(record);
    };

    useEffect(() => {
        var tempItems = [];
        if (record.isPublished) {
            tempItems.push({ key: "unpublish", label: "取消发布" });
        }
        else {
            tempItems.push({ key: "publish", label: "发布" });
        }

        tempItems.push({ key: "urlRecordFix", label: "UrlRecordFix" });

        if (!record.isPublished) {
            if (record.isDeleted) {
                tempItems.push({ key: "revokeDelete", label: "撤销删除" });
            }
            else {
                tempItems.push({ key: "delete", label: "删除" });
            }
        }
        setMainAction({ ...tempItems[0] });
        setItems(tempItems.splice(1, tempItems.length - 1));
    }, [record])
    return (
        <Dropdown.Button menu={{ items, onClick: onMenuClick }} onClick={() => onMenuClick({ key: mainAction.key })}>{mainAction.label}</Dropdown.Button>
    )
}
interface PostListProps {
    publishStatus?: boolean;
}
const PostList: React.FC<PostListProps> = (props) => {
    const { publishStatus } = props;
    const navigate = useNavigate();
    const [refreshTable, forceRefreshTable] = useState<number>(0);
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500"] });
    const [filterCondition, setFilterCondition] = useState<any>({});//JSON.parse(localStorage.getItem("page-search-parms-PostList") ?? '{}')
    const { currentSiteId } = useAppStore();
    const { data: postKindList } = useCategory("POST_KIND");

    // const publishStatusFilters = [{ text: "Published", value: true }, { text: "Unpublished", value: false }]

    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });
        columnsArray.push({ title: 'PostKind', dataIndex: 'postKindName', key: 'postKind', width: 160, filterMultiple: false, filters: postKindList });
        columnsArray.push({
            title: 'title', key: 'title', dataIndex: 'title', ellipsis: true, ...ColumnSearchProps("title", "title"),
            render: (text: any, record: any) => (<Link key="edit" to={`../edit/${record.postId}`} title={text}>{text}</Link>)
        });

        // columnsArray.push({ title: 'URL', key: 'slug', dataIndex: 'slug', width: 380, ellipsis: true, ...ColumnSearchProps("slug", "slug") });
        columnsArray.push({
            title: 'CreateTime', key: 'createTime', dataIndex: 'createTime', width: 180,
            render: (text: any, record: any) => <><LocalTime timestamp={record.createTime} /></>
        })
        columnsArray.push({ title: '更新时间', key: 'modifyTime', width: 180, render: (text: any, record: any) => <LocalTime timestamp={record.modifyTime} /> })
        columnsArray.push({
            title: '发布状态', key: 'isPublished', dataIndex: 'isPublished', width: 240, filterMultiple: false, //filters: publishStatusFilters,
            render: (text: any, record: any) => <><LocalTime timestamp={record.publishTime} /></>
        })
        columnsArray.push({
            title: '操作', key: 'delete', dataIndex: 'postId', width: 150,
            render: (text: any, record: any) => <><PostOperations record={record} onComplete={onOperationCompleted} /></>
        })
        return columnsArray;
    }

    const fetchTableData = async () => {
        console.log("Use filtercondiction to query");
        console.log(filterCondition);
        console.log("publish status:", props.publishStatus);
        var rsp = await postSearch({ ...pagination, ...filterCondition, isPublished: props.publishStatus })
        const { records, paging } = rsp.data;

        // 从API返回的分页信息重新设置, 约定: 当客户端传到服务器端的分页信息不准确的时候，服务器端返回给客户端的分页信息会做调整
        setPagination({ ...pagination, current: paging.pageIndex, pageSize: paging?.pageSize, total: paging?.total });

        let dataStoreArray: any = [];
        records.map((item: any, index: number) => { dataStoreArray.push({ ...item, key: item.postId, rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
        setDataSource(dataStoreArray);
    }

    const onTableChange = (changedPagination: any, filters: any) => {
        setPagination({ ...changedPagination });
        setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
        console.log("set filterCondition");
        console.log(filters);
    }

    const onOperationCompleted = (record: any) => {
        // fetchTableData();
        forceRefreshTable(Math.random());
    }

    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, [postKindList]);

    useEffect(() => {
        fetchTableData();
    }, [filterCondition, currentSiteId, props.publishStatus, refreshTable])

    return (
        <PageHeader tags={
            <Space size="middle">
                <Button key="refresh" onClick={async () => await fetchTableData()} >Refresh</Button>
                {publishStatus === false ? <Button key="create" onClick={() => navigate('../create')} >Create</Button> : null}
            </Space>
        }>
            <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
        </PageHeader>
    )
}

export default PostList;


