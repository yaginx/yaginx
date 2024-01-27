import { useEffect, useState } from "react";
import { Button, Table } from "antd";
import { PageHeader } from '@ant-design/pro-layout';
import { Link } from "react-router-dom";
import { LocalTime } from "@/componets/LocalTime";
import React from "react";
import { ArticleCreate, ArticlePageList } from "@/api/articleApi";
import { CreateForm } from "./CreateForm";

// export default function ArticleList() {
const ArticleList: React.FC = () => {
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 100, pageSizeOptions: ["100", "500", "1000"] });
    const [filterCondition, setFilterCondition] = useState<any>({});
    const [createFormModalVisiable, setCreateFormModalVisiable] = useState(false);
    // const [createFormModalVisiable, setCreateFormModalVisiable] = useState<any>({});

    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({
            title: '序号',
            dataIndex: 'rowIndex',
            key: 'rowIndex',
            align: 'center',
            width: 80,
            render: (text: any, record: any) => (<Link to={`edit/${record.pageId}`} className="ant-btn ant-btn-link">{text}</Link>)
        });
        columnsArray.push({ title: '正文标题', key: 'title', dataIndex: 'title', render: (text: any, record: any) => (<Link to={`edit/${record.pageId}`} title={record.metaSlug} className="ant-btn ant-btn-link">{record.title}</Link>) })
        // columnsArray.push({ title: 'URL', key: 'metaSlug', dataIndex: 'metaSlug' })
        columnsArray.push({ title: '作者', key: 'author', dataIndex: 'authorName' })
        columnsArray.push({ title: '发布状态', key: 'publish_status_text', dataIndex: 'publishStatusText' })
        // columnsArray.push({ title: '页签标题', key: 'metaTitle', dataIndex: 'metaTitle' })
        columnsArray.push({ title: '类别', key: 'category_id', dataIndex: 'categoryText' })
        columnsArray.push({ title: '页面类型', key: 'page_type', dataIndex: 'pageTypeText' })
        columnsArray.push({ title: '发布时间', key: 'publish_time', dataIndex: 'publishTime', render: (text: any, record: any) => <LocalTime timestamp={text} /> })
        columnsArray.push({ title: '更新时间', key: 'modify_time', dataIndex: 'modifyTime', render: (text: any, record: any) => <LocalTime timestamp={text} /> })
        return columnsArray;
    }
    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, []);
    const fetchTableData = async () => {
        var rsp = await ArticlePageList({ ...pagination, ...filterCondition })
        const { records, paging } = rsp.data;
        // if (records.length <= 0) {
        //     return;
        // }

        setPagination({
            ...pagination,
            ...filterCondition
        });

        let dataStoreArray: any = [];
        records.map((item: any, index: number) => { dataStoreArray.push({ ...item, key: item.pageId, rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
        setDataSource(dataStoreArray);
    }

    useEffect(() => {
        fetchTableData();
    }, [filterCondition])

    const onTableChange = (changedPagination: any, filters: any) => {
        setPagination({ ...changedPagination });
        setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
    }
    const onCreate = async (values: any) => {
        let rsp = await ArticleCreate(values);
        if (rsp.data.code === 200) {
            fetchTableData();
            setCreateFormModalVisiable(false);
        }
    };
    return (
        <PageHeader tags={[
            <Button key="create" onClick={() => setCreateFormModalVisiable(true)} >Create</Button>,
            <Button key="refresh" onClick={() => fetchTableData()} >Refresh</Button>]}>
            <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} size={"small"} />
            <CreateForm
                visible={createFormModalVisiable}
                onCreate={onCreate}
                onCancel={() => setCreateFormModalVisiable(false)}
            />
        </PageHeader>
    )
}

export default ArticleList;