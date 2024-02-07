import { FetchJson } from "@/requests";
import { IApiRspEnvelop } from "./apiTypes";

export const schemaModelSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model/list', method: 'POST', data });
export const schemaModelGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model/get', method: 'POST', params: data });
export const schemaModelCreate = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model/create', method: 'POST', data });
export const schemaModelEdit = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model/edit', method: 'POST', data });
export const schemaModelDelete = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model/delete', method: 'DELETE',params: data });

export const schemaModelFieldSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_field/list', method: 'POST', data });
export const schemaModelFieldGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_field/get', method: 'POST', params: data });
export const schemaModelFieldCreate = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_field/create', method: 'POST', data });
export const schemaModelFieldEdit = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_field/edit', method: 'POST', data });
export const schemaModelFieldDelete = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_field/delete', method: 'DELETE',params: data });

// export const infraClobServerSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_item_mode/list', method: 'POST', data });
// export const infraClobServerGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_item_mode/get', method: 'POST', params: data });
// export const infraClobServerCreate = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_item_mode/create', method: 'POST', data });
// export const infraClobServerEdit = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/model_item_mode/edit', method: 'POST', data });

export const schemaModelFullGet = (data: ISchemaModelFullGetRequest): Promise<IApiRspEnvelop<ISchemaModelFullInfo>> => FetchJson({ url: '/webapi/central/api/v1/model_schemas/query', method: 'GET', params: data });

export interface ISchemaModelFullGetRequest {
  modelName: string
}

export interface ISchemaModelFullInfo {
  id: number;
  name: string;
  pkFieldName: string;
  displayFieldName: string;
  fields: ISchemaModelFieldInfo[];
  modes:string[];
}
export interface ISchemaModelFieldInfo {
  id: number;
  name: string;
  modes: any[];
}

