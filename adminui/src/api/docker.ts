import { FetchJson } from "@/requests";
import { IApiRspEnvelop } from "./apiTypes";

export const containerSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/docker/container/search', method: 'GET', data });
export const containerGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/docker/container/get', method: 'GET', params: data });

export const websiteSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/website/search', method: 'POST', data });
export const websiteGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/website/get', method: 'GET', params: data });
export const websiteUpsert = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/website/upsert', method: 'POST', data });
export const websiteDelete = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/website/delete', method: 'DELETE', data });

export const webDomainSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/web_domain/search', method: 'POST', data });
export const webDomainGet = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/web_domain/get', method: 'GET', params: data });
export const webDomainUpsert = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/web_domain/upsert', method: 'POST', data });
export const webDomainDelete = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/web_domain/delete', method: 'DELETE', data });

export const hostTrafficSearch = (data: any): Promise<IApiRspEnvelop<any>> => FetchJson({ url: '/yaginx/api/host_traffic/search', method: 'POST', data });
