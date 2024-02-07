import { categorySelectList } from "@/api/category";
import { EnumMetadataType, FormatEnumResponse, getEnumNumberValue } from "@/api/enumApi";
import { useEffect, useState, useRef } from "react";

export const useEnumStatus = (statusEnumType: EnumMetadataType) => {
    const cache = useRef<any>({});
    const [status, setStatus] = useState("idle");
    const [data, setData] = useState<{ text: string; value: number | string }[]>([]);
    const enumTypeString = statusEnumType.toString();

    const fetchData = async () => {
        if (cache.current[enumTypeString]) {
            const data = cache.current[enumTypeString];
            setData(data);
            setStatus("fetched");
        } else {
            setStatus("fetching");
            const rsp = await getEnumNumberValue(
                EnumMetadataType[statusEnumType]
            );
            const data = FormatEnumResponse(rsp.data);
            cache.current[enumTypeString] = data; // set response in cache;
            setData(data);
            setStatus("fetched");
        }
    };

    useEffect(() => {
        fetchData();
    }, [statusEnumType]);
    return { status, data };
};

export const useCategory = (categoryName: string) => {
    // const cache = useRef<any>({});
    const [status, setStatus] = useState("idle");
    const [data, setData] = useState<{ text: string; value: number | string }[]>([]);

    const fetchData = async () => {
        // if (cache.current[categoryName]) {
        //     const data = cache.current[categoryName];
        //     setData(data);
        //     setStatus("fetched");
        // } else {

        // }
        setStatus("fetching");
        const rsp = await categorySelectList({ parentCategoryName: categoryName });
        const data = FormatEnumResponse(rsp.data);
        // cache.current[categoryName] = data; // set response in cache;
        setData(data);
        setStatus("fetched");
    };

    useEffect(() => {
        fetchData();
    }, [categoryName]);
    return { status, data };
};
