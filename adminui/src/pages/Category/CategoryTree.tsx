import { useEffect } from "react";
import { Tree } from "antd";
import React from "react";
import { categoryTreeSelectList } from "@/api/category";
import { TreeListItem } from "@/api/enumApi";
import { useState } from "react";
import { DataNode, EventDataNode } from "antd/lib/tree";

// It's just a simple demo. You can use tree map to optimize update perf.
const updateTreeData = (list: DataNode[], key: React.Key, children: DataNode[]): DataNode[] =>
    list.map(node => {
        if (node.key === key) {
            return {
                ...node,
                children,
            };
        }
        if (node.children) {
            return {
                ...node,
                children: updateTreeData(node.children, key, children),
            };
        }
        return node;
    });

interface CategoryTreeProps {
    onNodeClick: (categoryId: string | number, isSelected: boolean) => void
}
//自定义的下来搜索
const CategoryTree = (props: CategoryTreeProps) => {
    const [treeData, setTreeData] = useState<DataNode[]>([]);

    const loadRoot = () => {
        categoryTreeSelectList({}).then(rsp => {
            var tempDataList: DataNode[] = [];
            rsp.data.map((d: TreeListItem) => {
                tempDataList.push({ key: d.value.toString(), title: d.displayText, isLeaf: d.isLeaf });
            })
            setTreeData(tempDataList);
        });
    }
    const onLoadData = async ({ key, children }: any) => {
        if (children) {
            return;
        }
        var rsp = await categoryTreeSelectList({ parentId: key });
        var newOptions: DataNode[] = [];
        rsp.data.map((d: TreeListItem) => {
            newOptions.push({ key: d.value.toString(), title: d.displayText, isLeaf: d.isLeaf });
        })
        setTreeData(origin =>
            updateTreeData(origin, key, newOptions),
        );
    };

    const onClick = (e: React.MouseEvent<HTMLSpanElement>, node: EventDataNode<DataNode>) => {
        console.log(node);
        props.onNodeClick(node.key, !node.selected);
    }

    useEffect(() => {
        loadRoot();
    }, [])

    return (
        <Tree loadData={onLoadData} treeData={treeData} onClick={onClick} />
    );
}
export default CategoryTree;
