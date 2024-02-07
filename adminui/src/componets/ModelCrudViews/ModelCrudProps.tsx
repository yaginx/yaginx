import { ISchemaModelFieldInfo, ISchemaModelFullInfo } from "@/api/modelSchemas";

// pages/Home/index.tsx
export const AuditKeys: string[] = ["ts", "createdBy", "createTime", "updatedBy", "updateTime", "isDeleted", "deletedBy", "deleteTime"];
export interface ISchemaModelInfo {
  sysId?: string;
  modelName: string;
  pkFieldName: string;
  displayFieldName: string;
  fields?: ISchemaModelFieldInfo[];
}

export interface ITableListViewProps extends ISchemaModelInfo {
  tableDataFetchApi: (data: any) => Promise<any>;
  tableDataDeleteApi?: (data: any) => Promise<any>;
  revokeDelete?: (data: any) => Promise<any>;
}
export interface IModelCreateViewProps extends ISchemaModelInfo {
  modelCreateSubmitApi: (data: any) => Promise<any>;
}
export interface IModelEditFormProps extends ISchemaModelInfo {
  values: any;
  keyPropertyName: string;
}
export interface IModelEditViewProps extends ISchemaModelInfo {
  modelEditSubmitApi: (data: any) => Promise<any>;
  modelGetApi: (data: any) => Promise<any>;
}
export interface BasicCrudIndexProps {
  modelName: string;
  search: (data: any) => Promise<any>;
  delete?: (data: any) => Promise<any>;
  create: (data: any) => Promise<any>;
  edit: (data: any) => Promise<any>;
  get: (data: any) => Promise<any>;
  revokeDelete?: (data: any) => Promise<any>;
}
