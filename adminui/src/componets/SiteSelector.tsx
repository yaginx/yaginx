// import "./SiteSelector.less";

import { useEffect, useState } from "react";
import { Select } from 'antd';
import React from "react";
import { EnumNumberResponse, EnumStringResponse } from "@/api/enumApi";
import { useAppStore } from "@/store/appStore";
import { SiteSelectList } from "@/api/siteApi";
interface Option {
    key: string;
    value: string;
    label: string;
}
const { Option } = Select;
export default function SiteSelector({ ...props }) {
    const { currentSiteId, setCurrentSite } = useAppStore();
    const [options, setOptions] = useState<Option[]>([{ label: "请选择", value: "", key: "" }]);

    useEffect(() => {
        SiteSelectList().then(rsp => {
            if (rsp.data) {
                var newOptions: Option[] = [];
                rsp.data.map((d: EnumStringResponse) => {
                    newOptions.push({ key: d.value, value: d.value, label: d.displayText });
                })
                setOptions(newOptions);
                if (currentSiteId === null && newOptions.length > 0) {
                    var defaultSiteId = newOptions[0].value;
                    setCurrentSite(defaultSiteId);
                }
            }
        });
    }, [])
    const handleChange = (value: string) => {
        setCurrentSite(value);
    };

    return (
        <>
            <Select allowClear={false} defaultValue={currentSiteId} defaultActiveFirstOption={true} showArrow={true} filterOption={false}
                notFoundContent={null} value={currentSiteId} onChange={handleChange} {...props} options={options} />
        </>
    );
}

