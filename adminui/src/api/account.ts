import { FetchJson } from "@/utils/request";
export const login = (data: any): Promise<any> => FetchJson({ url: '/api/management/account/login', method: 'POST', data });
export const getAuthToken = (data: any): Promise<any> => FetchJson({ url: '/api/management/account/auth_token', method: 'GET', data });
export const getUserInfo = (data: any): Promise<any> => FetchJson({ url: '/api/management/account/user_info', method: 'GET', data });