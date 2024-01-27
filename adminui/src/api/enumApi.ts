import { FetchJson } from "@/utils/request";
export const getEnumNumberValue = (type: string): Promise<any> => FetchJson({ url: `/api/management/metadata/enum/number_value/${type}`, method: 'GET' });

export enum EnumMetadataType {
  PageType,
  UrlRecordCategory,
  PublishStatus,
  UrlRecordHandleType
}

export type EnumNumberResponse = {
  displayText: string;
  value: number;
  children: [] | null;
};
export type EnumStringResponse = {
  displayText: string;
  value: string;
  children: [] | null;
};
export type TreeListItem = {
  displayText: string;
  value: string;
  isLeaf: boolean;
};

export function FormatEnumResponse(data: Array<EnumStringResponse>) {
  let result: { text: string; value: string | string }[] = [];
  data.forEach((d) => {
    result.push({
      text: d.displayText,
      value: d.value,
    });
  });
  return result;
}