import { Fetch, FetchJson } from "@/utils/request";

export const postGet = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/get', method: 'POST', data });
export const postGetForContentInfoModify = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/get/content_info_for_modify', method: 'POST', data });
export const postDelete = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/delete', method: 'DELETE', data });
export const postRevokeDelete = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/revoke_delete', method: 'GET', data });
export const postPublish = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/publish', method: 'POST', data });
export const postSearch = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/search', method: 'POST', data });
export const postCreate = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/create', method: 'POST', data });
export const postModifyContentInfo = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/modify/content_info', method: 'POST', data });
export const imageUpload = (form: FormData): Promise<any> => {
    form.append("domain", "globalmandarin.com");
    form.append("subpath", "blog/image");
    return Fetch('/api/management/post/img/upload', { method: "POST", body: form });
}
export const postUrlRecordFix = (data: any): Promise<any> => FetchJson({ url: '/api/management/post/url_record_fix', method: 'POST', data });
export const getUserInfo = (): Promise<any> => FetchJson({ url: '/api/management/account/user_info', method: 'GET' });

// ItemContent Apis
export const itemContentSearch = (data: any): Promise<any> => FetchJson({ url: '/api/management/item_content/search', method: 'POST', data });
export const itemContentCreate = (data: any): Promise<any> => FetchJson({ url: '/api/management/item_content/create', method: 'POST', data });
export const itemContentModify = (data: any): Promise<any> => FetchJson({ url: '/api/management/item_content/modify', method: 'POST', data });
export const itemContentDelete = (data: any): Promise<any> => FetchJson({ url: '/api/management/item_content/delete', method: 'DELETE', data });
export const itemContentGet = (data: any): Promise<any> => FetchJson({ url: '/api/management/item_content/get', method: 'POST', data });


