import React, { useEffect, useState } from 'react';
import { Button, Space, Switch, Table, Tag } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { postDelete, postPublish, postRevokeDelete, postSearch } from '@/api/post';
import { Link, useNavigate } from 'react-router-dom';
import { LocalTime } from '@/componets/LocalTime';
import { useAppStore } from '@/store/appStore';
interface publisDeleteProps {
    record: any,
    handlePostDelete: (postId: number) => void;
    handlePostRevokeDelete: (postId: number) => void;
}
const PostListDeleteOptions: React.FC<publisDeleteProps> = (props: publisDeleteProps) => {
    const { record, handlePostRevokeDelete, handlePostDelete } = props;
    if (!record.isPublished) {
        return (
            record.isDeleted === 1
                ? <Button key="delete" danger onClick={() => handlePostRevokeDelete(record.postId)} >撤销删除</Button>
                : <Button key="delete" danger onClick={() => handlePostDelete(record.postId)} >删除</Button>
        )
    }
    else {
        return <></>
    }
}

interface publishOptionProps {
    record: any,
    handlePublish: (data: any) => void;
}
const PostListPublishOptions: React.FC<publishOptionProps> = (props: publishOptionProps) => {
    const { record, handlePublish } = props;
    return (
        record.isPublished === 1
            ? <Button key="publish" danger onClick={() => handlePublish({ postId: record.postId, isPublished: false })}>取消发布</Button>
            : <Button key="publish" danger onClick={() => handlePublish({ postId: record.postId, isPublished: true })}>发布</Button>
    )
}
interface PostListViewProps {
    postKind: number
}

export const PostListView: React.FC<PostListViewProps> = (props: PostListViewProps) => {
    const navigate = useNavigate();
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500"] });
    const [filterCondition, setFilterCondition] = useState<any>({});
    const { currentSiteId } = useAppStore();
    const { postKind } = props;
    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, []);
    useEffect(() => {
        fetchTableData();
    }, [filterCondition, currentSiteId])

    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });
        columnsArray.push({
            title: 'title', key: 'title', dataIndex: 'title', ellipsis: true,
            render: (text: any, record: any) => (<Link key="edit" to={`edit/${record.postId}`} title={text}>{text}</Link>)
        })

        columnsArray.push({
            title: '地址', key: 'slug', dataIndex: 'slug', width: 380, ellipsis: true,
            render: (text: any, record: any) => (<>{text}</>)
        })
        columnsArray.push({
            title: '发布时间', key: 'isPublished', dataIndex: 'isPublished', align: 'center', width: 240,
            render: (text: any, record: any) =>
                <>
                    <LocalTime timestamp={record.publishTime} />
                </>
        })
        columnsArray.push({ title: '添加时间', key: 'createTime', align: 'center', width: 180, render: (text: any, record: any) => <LocalTime timestamp={record.createTime} /> })
        // columnsArray.push({
        //     title: '删除', key: 'isDeleted', dataIndex: 'isDeleted', width: 80,
        //     render: (text: any, record: any) => (<>{text === 1 ? "是" : "否"}</>)
        // })
        columnsArray.push({
            title: '操作', key: 'delete', dataIndex: 'postId', align: 'center', width: 160,
            render: (text: any, record: any) =>
                <>
                    <Space>
                        <PostListPublishOptions record={record} handlePublish={handlePostPublish} />
                        <PostListDeleteOptions record={record} handlePostDelete={handlePostDelete} handlePostRevokeDelete={handlePostRevokeDelete} />
                    </Space>
                </>
        })
        return columnsArray;
    }

    const handlePostDelete = async (postId: number) => {
        await postDelete({ postId });
        await fetchTableData();
    }
    const handlePostRevokeDelete = async (postId: number) => {
        await postRevokeDelete({ postId });
        await fetchTableData();
    }

    const handlePostPublish = async (data: any) => {
        await postPublish(data);
        await fetchTableData();
    }
    const fetchTableData = async () => {
        var rsp = await postSearch({ postKind, ...pagination, ...filterCondition })
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
    }

    return (
        <PageHeader tags={
            <Space size="middle">
                <Button key="refresh" onClick={() => fetchTableData()} >Refresh</Button>
                <Button key="create" onClick={() => navigate('create')} >Create</Button>
            </Space>
        }>
            <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
        </PageHeader>
    )
}