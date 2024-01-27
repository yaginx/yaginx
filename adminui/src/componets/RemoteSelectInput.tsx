
import { categorySelectList } from '@/api/category';
import { EnumNumberResponse, getEnumNumberValue } from '@/api/enumApi';
import { Select, Spin } from 'antd';
import { debounce } from 'lodash';
import React from 'react';
import { useEffect, useState } from 'react';

const { Option } = Select;
interface Option {
    key: number;
    value: number;
    label: string;
}
export function RemoteSelectInput({ ...props }: any) {
    const [data, setData] = useState<any>({ data: [] });
    const [options, setOptions] = useState<Option[]>([{ label: "请选择", value: -1, key: -1 }]);
    const handleSearch = (value: any) => {
        if (value) {
            getEnumNumberValue('PageType')
                .then(rsp => {
                    if (!rsp.data) {
                        return;
                    }
                    var newOptions: Option[] = [];
                    rsp.data.map((d: EnumNumberResponse) => {
                        newOptions.push({ key: d.value, value: d.value, label: d.displayText });
                    })
                    setOptions(newOptions);
                });
        } else {
            setData({ data: [] });
        }
    };

    return (
        <Select showSearch allowClear={true}
            defaultActiveFirstOption={true}
            showArrow={false}
            filterOption={false}
            onSearch={handleSearch}
            notFoundContent={null} {...props} options={options} />
    );
}