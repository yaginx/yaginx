import { categorySelectList, categoryTreeSelectList } from "@/api/category";
import { EnumNumberResponse, TreeListItem } from "@/api/enumApi";
import { Cascader } from "antd";
import React, { useEffect } from "react";
import { useState } from "react";
interface Option {
    value: string;
    label: string;
    children?: Option[];
    isLeaf?: boolean;
    loading?: boolean;
}
export function CategoryTreeSelectInput({ parentCategoryNamePath, searchText, searchDeep, selectedValueIds, ...props }: any) {
    const [data, setData] = useState<Array<TreeListItem>>([]);
    const [options, setOptions] = useState<Option[]>([]);

    const loadRoot = () => {
        console.log("LoadRoot")
        categoryTreeSelectList({}).then(rsp => {
            var newOptions: Option[] = [];
            rsp.data.map((d: TreeListItem) => {
                newOptions.push({ value: d.value.toString(), label: d.displayText, isLeaf: d.isLeaf });
            })
            setOptions(newOptions);
        });
    }

    const loadData = (selectedOptions: any = null) => {
        const targetOption = selectedOptions ? selectedOptions[selectedOptions.length - 1] : null;
        if (!targetOption) {
            return;
        }

        targetOption.loading = true;

        console.log(selectedOptions);
        console.log(targetOption);
        categoryTreeSelectList({ parentId: targetOption.value }).then(rsp => {
            var newOptions: Option[] = [];
            rsp.data.map((d: TreeListItem) => {
                newOptions.push({ value: d.value.toString(), label: d.displayText, isLeaf: d.isLeaf });
            })
            targetOption.children = newOptions;
            setOptions([...options]);
            targetOption.loading = false;
        });
    };

    useEffect(() => {
        loadRoot();
    }, [])

    // useEffect(() => {
    //     // debounceFetcher("");
    //     console.log("onChange")
    //     loadData();
    // }, [parentCategoryNamePath, searchDeep, props.value])
    // const onChange = (value: string[], selectedOptions: Option[]) => {
    //     console.log("onChange")
    //     console.log(value, selectedOptions);
    // };
    return <Cascader options={options} loadData={loadData} {...props} changeOnSelect={true} />;
}