
import { categorySelectList } from '@/api/category';
import { EnumNumberResponse } from '@/api/enumApi';
import { Select, Spin, Tree } from 'antd';
import { debounce } from 'lodash';
import React from 'react';
import { useEffect, useState } from 'react';

const { Option } = Select;

interface Option {
    key: number;
    value: number | string;
    label: string;
}
export function CategorySelectInput({ parentCategoryName, searchText, selectedValueIds, ...props }: any) {
    const fetchRef = React.useRef(0);
    const debounceTimeout = 800;
    const [fetching, setFetching] = React.useState(false);
    const [options, setOptions] = useState<Option[]>([{ label: "请选择", value: -1, key: -1 }]);
    const { mode } = props;

    const debounceFetcher = React.useMemo(() => {
        const loadOptions = (searchText: string) => {
            fetchRef.current += 1;
            const fetchId = fetchRef.current;
            setFetching(true);
            let selectedValueIds = props.value ? (Array.isArray(props.value) ? props.value : [props.value]) : [];
            selectedValueIds = selectedValueIds.join(",");
            categorySelectList({ parentCategoryName, searchText, selectedValueIds, }).then(rsp => {
                setFetching(false);
                if (fetchId !== fetchRef.current) {
                    return;
                }

                var newOptions: Option[] = [];
                if (rsp.data) {
                    rsp.data.map((d: EnumNumberResponse) => {
                        if (mode === "tags") {
                            newOptions.push({ key: d.value, value: d.displayText, label: d.displayText });
                        }
                        else {
                            newOptions.push({ key: d.value, value: d.value, label: d.displayText });
                        }
                    })
                }
                setOptions(newOptions);
            });
        };

        return debounce(loadOptions, debounceTimeout);
    }, [debounceTimeout, parentCategoryName]);

    useEffect(() => {
        debounceFetcher("");
    }, [parentCategoryName])

    return (
        <Select showSearch allowClear={true} onSearch={debounceFetcher} autoClearSearchValue={false}
            defaultActiveFirstOption={false} showArrow={true} filterOption={false}
            notFoundContent={fetching ? <Spin size="small" /> : null} {...props} options={options} />
    );
}