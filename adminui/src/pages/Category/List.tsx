import { useEffect, useState } from "react";
import { Button, Col, Row, Table, Tree } from "antd";
import { PageHeader } from '@ant-design/pro-layout';
import { Link } from "react-router-dom";
import ColumnSearchProps from "./ColumnSearchProps";
// import { useStore } from "reto";
// import { AppStore } from "@/stores/AppStore";
import { CreateForm } from "./CreateForm";
import React from "react";
import { categoryCreate, categoryDelete, categoryModify, categorySearchList } from "@/api/category";
import { LocalTime } from "@/componets/LocalTime";
import { useAppStore } from "@/store/appStore";
import { CategoryEditForm } from "./EditForm";
import CategoryTree from "./CategoryTree";
import { DataNode } from "antd/lib/tree";
import { FilterValue } from "antd/lib/table/interface";
export default function UrlRecordList() {
    const [columns, setColumns] = useState<any>([]);
    const [dataSource, setDataSource] = useState<any>([]);
    const [pagination, setPagination] = useState<any>({ current: 1, pageSize: 50, pageSizeOptions: ["50", "100", "500", "1000"] });
    const [filterCondition, setFilterCondition] = useState<Record<string, any>>({});
    const [editModalFormVisiable, setEditModalFormVisiable] = useState(false);
    const [editCategoryId, setEditCategoryId] = useState<any>();
    const { currentSiteId } = useAppStore();
    const [createFormModalVisiable, setCreateFormModalVisiable] = useState(false);

    const initTableColumn = () => {
        let columnsArray: any = [];
        columnsArray.push({ title: '序号', dataIndex: 'rowIndex', key: 'rowIndex', align: 'center', width: 80 });
        columnsArray.push({
            title: 'name', key: 'name', dataIndex: 'name', ...ColumnSearchProps("name", "名称"),
            render: (text: any, record: any) => (
                <Link key="edit" title={record.slug ?? "无备注"} to={"#"} onClick={() => handleEditRecord(record)} >{record.name}</Link>
            )
        })
        columnsArray.push({ title: 'normalizeName', key: 'normalizeName', dataIndex: 'normalizeName', ...ColumnSearchProps("normalizeName", "normalizeName") })
        columnsArray.push({ title: 'slug', key: 'slug', dataIndex: 'slug', ...ColumnSearchProps("slug", "Slug") })
        // columnsArray.push({ title: 'kind', key: 'kind', dataIndex: 'kindText', filters: categoryKindEnumList, filterMultiple: false })
        columnsArray.push({ title: 'id', key: 'id', dataIndex: 'categoryId' })
        columnsArray.push({ title: 'Tags', key: 'strTags', dataIndex: 'strTags' })
        // columnsArray.push({ title: 'site', key: 'site', dataIndex: 'siteName' })
        // columnsArray.push({ title: 'action', key: 'action', dataIndex: 'categoryId', render: (text: any, record: any) => (<Button key="urlRecordFix" onClick={() => handleUrlRecordFix(record.categoryId)} >URL记录修复</Button>) })
        // columnsArray.push({ title: 'metadic', key: 'metadic', render: (text: any, record: any) => (<span>{record.metadic ? record.metadic.length : 0}</span>) })
        // columnsArray.push({ title: 'createTime', key: 'createTime', dataIndex: 'createTime', render: (text: any, record: any) => <LocalTime timestamp={record.createTime} /> })
        // columnsArray.push({ title: 'modifyTime', key: 'modifyTime', dataIndex: 'modifyTime', render: (text: any, record: any) => <LocalTime timestamp={record.modifyTime} /> })
        columnsArray.push({
            title: '操作', key: 'delete', dataIndex: 'postId', width: 80,
            render: (text: any, record: any) => (<Button key="delete" danger onClick={() => handlePostDelete(record.categoryId)} >删除</Button>)
        })
        return columnsArray;
    }
    // 设置table column
    useEffect(() => {
        setColumns(initTableColumn());
    }, []);
    const handlePostDelete = async (categoryId: number) => {
        await categoryDelete({ categoryId });
        await fetchTableData();
    }
    const fetchTableData = async () => {
        var rsp = await categorySearchList({ ...pagination, ...filterCondition, searchText: filterCondition.name })
        const { records, paging } = rsp.data;
        // if (records.length <= 0) {
        //     return;
        // }

        // 从API返回的分页信息重新设置, 约定: 当客户端传到服务器端的分页信息不准确的时候，服务器端返回给客户端的分页信息会做调整
        setPagination({ ...pagination, current: paging.pageIndex, pageSize: paging.pageSize, total: paging.total, });

        let dataStoreArray: any = [];
        records.map((item: any, index: number) => { dataStoreArray.push({ ...item, strTags: item.tags.join(","), key: item.categoryId, rowIndex: (paging.pageIndex - 1) * paging.pageSize + index + 1 }); });
        setDataSource(dataStoreArray);
    }

    useEffect(() => {
        fetchTableData();
    }, [filterCondition, currentSiteId])

    const onTableChange = (changedPagination: any, filters: Record<string, FilterValue | null>) => {
        setPagination({ ...changedPagination });
        setFilterCondition({ ...filters, pageSize: changedPagination.pageSize, pageIndex: changedPagination.current })
    }

    const handleEditRecord = (record: any) => {
        setEditCategoryId(record.categoryId);
        setEditModalFormVisiable(true);
    }
    const onCreateSubmit = async (values: any) => {
        // 获取最后一个值
        values.parentCategoryId = values.parentCategoryId[values.parentCategoryId.length - 1];
        let rsp = await categoryCreate(values);
        if (rsp.code === 200) {
            fetchTableData();
            setCreateFormModalVisiable(false);
        }
    };
    const onEditSubmit = async (values: any) => {
        let rsp = await categoryModify(values);
        if (rsp.code === 200) {
            fetchTableData();
            setEditModalFormVisiable(false);
        }
    };
    const onLeftTreeNodeClick = (selectedCategoryId: number | string, isSelected: boolean) => {
        // filterCondition
        if (isSelected) {
            setFilterCondition({ ...filterCondition, parentCategoryId: selectedCategoryId })
        }
        else {
            var newFilterCondition = { ...filterCondition };
            delete newFilterCondition.parentCategoryId;
            setFilterCondition({ ...newFilterCondition });
        }
    };
    return (
        <PageHeader tags={[
            <Button key="create" onClick={() => setCreateFormModalVisiable(true)} >Create</Button>,
            <Button key="refresh" onClick={() => fetchTableData()} >Refresh</Button>
        ]}>
            <Row>
                <Col span={4}>
                    <CategoryTree onNodeClick={onLeftTreeNodeClick} />
                </Col>
                <Col span={20}>
                    <Table dataSource={dataSource} columns={columns} pagination={pagination} onChange={onTableChange} />
                </Col>
            </Row>

            <CreateForm
                open={createFormModalVisiable}
                onSubmit={onCreateSubmit}
                onCancel={() => setCreateFormModalVisiable(false)}
            />
            <CategoryEditForm
                open={editModalFormVisiable}
                onSubmit={onEditSubmit}
                categoryId={editCategoryId}
                onCancel={() => { setEditModalFormVisiable(false); }}
            />
        </PageHeader>
    )
}