import { FetchJson } from "@/utils/request";

export const ArticlePageList = (params: any): Promise<any> => FetchJson({ url: '/api/cms/article/page_list', method: 'POST', data: params });
export const ArticleGet = (params: any): Promise<any> => FetchJson({ url: '/api/cms/article/get', method: 'POST', data: params });
export const ArticleCreate = (params: any): Promise<any> => FetchJson({ url: '/api/cms/article/create', method: 'POST', data: params });
export const ArticleModify = (params: any): Promise<any> => FetchJson({ url: '/api/cms/article/modify', method: 'POST', data: params });
export const ArcticleDelete = (params: any): Promise<any> => FetchJson({ url: '/api/cms/article/delete', method: 'POST', data: params });
