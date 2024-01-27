import { categorySelectList } from '@/api/category';
import { EnumNumberResponse, getEnumNumberValue } from '@/api/enumApi';
import { Select, Spin } from 'antd';
import { debounce } from 'lodash';
import React from 'react';
import { useEffect, useState } from 'react';

const { Option } = Select;

export function RemoteSelectInput({ ...props }: any) {
    const [data, setData] = useState<[]>([]);
    const handleSearch = (value: any) => {
        if (value) {
            getEnumNumberValue('PageType')
                .then(result => setData(result.data));
        } else {
            setData([]);
        }
    };
    const options = data.map((d: EnumNumberResponse) => <Option key={d.value} value={d.value}>{d.displayText}</Option>);
    return (
        <Select showSearch allowClear={true}
            defaultActiveFirstOption={true}
            showArrow={false}
            filterOption={false}
            onSearch={handleSearch}
            notFoundContent={null} {...props}>
            {options}
        </Select>
    );
}

export function EnumSelectInput({ enumType, ...props }: any) {
    const [data, setData] = useState<[]>([]);

    useEffect(() => {
        getEnumNumberValue(enumType).then(result => {
            if (result)
                setData(result.data)
        });
    }, [enumType])

    const options = data.map((d: EnumNumberResponse) => <Option key={d.value} value={d.value}>{d.displayText}</Option>);
    return (
        <Select allowClear={false} defaultActiveFirstOption={false} showArrow={true} filterOption={false} notFoundContent={null} {...props} >
            {options}
        </Select>
    );
}

export function CategorySelectInput({ categoryKind, typeLevel1, typeLevel2, ...props }: any) {
    const [data, setData] = useState<[]>([]);
    const fetchRef = React.useRef(0);
    const debounceTimeout = 800;
    const [fetching, setFetching] = React.useState(false);

    const debounceFetcher = React.useMemo(() => {
        const loadOptions = (searchText: string) => {
            fetchRef.current += 1;
            const fetchId = fetchRef.current;
            setFetching(true);
            console.log(`CategoryValues:${props.value}`);
            let selectedValueIds = props.value ? (Array.isArray(props.value) ? props.value : [props.value]) : [];
            selectedValueIds = selectedValueIds.join(",");
            console.log(`SelectedValues:${selectedValueIds}`);
            categorySelectList({ categoryKind, searchText, selectedValueIds, typeLevel1, typeLevel2 }).then(result => {
                if (fetchId !== fetchRef.current) {
                    // for fetch callback order
                    return;
                }
                setData(result?.data ? result.data : [])
                setFetching(false);
            });
        };

        return debounce(loadOptions, debounceTimeout);
    }, [debounceTimeout, categoryKind, typeLevel1, typeLevel2, props.value]);

    useEffect(() => {
        debounceFetcher("");
    }, [categoryKind, typeLevel1, typeLevel2, props.value])

    const options = data.map((d: EnumNumberResponse) => <Option key={d.value} value={d.value}>{d.displayText}</Option>);
    return (
        <Select showSearch allowClear={false} onSearch={debounceFetcher}
            defaultActiveFirstOption={true} showArrow={false} filterOption={false}
            notFoundContent={fetching ? <Spin size="small" /> : null} {...props} >
            {options}
        </Select>
    );
}