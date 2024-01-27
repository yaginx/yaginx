import { useEffect, useState } from "react";
import { Button, Table } from "antd";
import { PageHeader } from '@ant-design/pro-layout';
import { Link } from "react-router-dom";
import React from "react";
import { LocalTime } from "@/componets/LocalTime";
import { urlRecordCreate, urlRecordDelete, urlRecordPageList, urlRecordUpdate } from "@/api/urlRecordApi";
import { UrlRecordModalEditForm } from "./UrlRecordModalEditForm";
import ColumnSearchProps from "../Category/ColumnSearchProps";
import { EnumMetadataType } from "@/api/enumApi";
import { useEnumStatus } from "@/hooks/useEnumStatus";

export default function UrlRecordList() {
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500", "1000"] });
    const [filterCondition, setFilterCondition] = useState<any>({});
    const [editModalFormVisiable, setEditModalFormVisiable] = useState(false);
    const [editUrlRecordId, setEditUrlRecordId] = useState<string | null>(null);
    const { data: urlRecordCategories } = useEnumStatus(EnumMetadataType.UrlRecordCategory);
    const { data: urlRecordHandleTypes } = useEnumStatus(EnumMetadataType.UrlRecordHandleType);
    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });
        // columnsArray.push({ title: 'name', key: 'name', dataIndex: 'name', render: (text: any, record: any) => (<Button key="create" onClick={() => handleEditRecord(record.urlRecordId)} >Edit</Button>) })
        columnsArray.push({ title: 'categoryText', key: 'category', dataIndex: 'categoryText', filterSearch: true, filterMultiple: false, filters: urlRecordCategories })
        columnsArray.push({ title: 'slug', key: 'slug', dataIndex: 'slug', ...ColumnSearchProps("slug", "slug"), render: (text: any, record: any) => (<Link key="edit" title={record.memo ?? "无备注"} to={"#"} onClick={() => handleEditRecord(record.urlRecordId)} >{text}</Link>) })
        columnsArray.push({ title: 'HandleTypeText', key: 'handleType', dataIndex: 'handleTypeText', filterSearch: true, filterMultiple: false, filters: urlRecordHandleTypes })
        columnsArray.push({ title: 'metadic', key: 'metadic', render: (text: any, record: any) => (<span>{record.metadic ? record.metadic.length : 0}</span>) })
        columnsArray.push({ title: 'createTime', key: 'createTime', dataIndex: 'createTime', render: (text: any, record: any) => <LocalTime timestamp={record.createTime} /> })
        columnsArray.push({ title: 'modifyTime', key: 'modifyTime', dataIndex: 'modifyTime', render: (text: any, record: any) => <LocalTime timestamp={record.modifyTime} /> })
        columnsArray.push({ title: '操作', key: 'delete', dataIndex: 'cateogryId', width: 80, render: (text: any, record: any) => (<Button key="delete" danger onClick={() => handleUrlRecordDelete(record.urlRecordId)} >删除</Button>) })
        return columnsArray;
    }
    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, [urlRecordCategories, urlRecordHandleTypes]);
    const fetchTableData = async () => {
        var rsp = await urlRecordPageList({ ...pagination, ...filterCondition })
        const { records, paging } = rsp.data;
        // if (records.length <= 0) {
        //     return;
        // }

        // 从API返回的分页信息重新设置, 约定: 当客户端传到服务器端的分页信息不准确的时候，服务器端返回给客户端的分页信息会做调整
        setPagination({ ...pagination, current: paging.pageIndex, pageSize: paging.pageSize, total: paging.total, });

        let dataStoreArray: any = [];
        records.map((item: any, index: number) => { dataStoreArray.push({ ...item, key: item.urlRecordId, rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
        setDataSource(dataStoreArray);
    }

    useEffect(() => {
        fetchTableData();
    }, [filterCondition])

    const onTableChange = (changedPagination: any, filters: any) => {
        setPagination({ ...changedPagination });
        setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
    }
    const handleCreateUrlRecord = () => {
        setEditUrlRecordId(null);
        setEditModalFormVisiable(true);
    }
    const handleEditRecord = (urlRecordId: string) => {
        console.log("edit:" + urlRecordId);
        setEditUrlRecordId(urlRecordId);
        setEditModalFormVisiable(true);
    }
    const onEditModalFormSubmit = async (values: any) => {
        console.log('Received values of form: ', values);
        if (values.urlRecordId === null) {
            await urlRecordCreate(values);
        }
        else {
            await urlRecordUpdate(values);
        }
        setEditUrlRecordId(null);
        setEditModalFormVisiable(false);
        fetchTableData();
    };
    const handleUrlRecordDelete = async (urlRecordId: number) => {
        await urlRecordDelete({ urlRecordId });
        await fetchTableData();
    }
    return (
        <PageHeader tags={[
            <Button key="create" onClick={() => handleCreateUrlRecord()} >Create</Button>,
            <Button key="refresh" onClick={() => fetchTableData()} >Refresh</Button>
        ]}>
            <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} size="middle" />
            <UrlRecordModalEditForm
                urlRecordId={editUrlRecordId}
                visible={editModalFormVisiable}
                onCreate={onEditModalFormSubmit}
                onCancel={() => { setEditModalFormVisiable(false); }}
            />
        </PageHeader>
    )
}