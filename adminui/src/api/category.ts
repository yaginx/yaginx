import { Fetch, FetchJson } from "@/utils/request";
export const categorySelectList = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/select_list', method: 'GET', data });
export const categorySearchList = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/search', method: 'POST', data });
export const categoryUrlRecordFix = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/url_record_fix', method: 'POST', data });
export const categoryCreate = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/create', method: 'POST', data });
export const categoryModify = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/modify', method: 'POST', data });
export const categoryDelete = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/delete', method: 'DELETE', data });
export const categoryTreeSelectList = (data: any): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/tree_select_list', method: 'GET', data });
export const categoryGet = (data: {categoryId:Number|undefined}): Promise<any> => FetchJson({ url: '/yaginx/api/management/category/get', method: 'POST', data });