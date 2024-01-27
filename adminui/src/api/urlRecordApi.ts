import { FetchJson } from "@/utils/request";
export const urlRecordPageList = (data: any): Promise<any> => FetchJson({ url: '/api/management/url_record/page_list', method: 'POST', data });
export const urlRecordGet = (data: any): Promise<any> => FetchJson({ url: '/api/management/url_record/get', method: 'POST', data });
export const urlRecordCreate = (data: any): Promise<any> => FetchJson({ url: '/api/management/url_record/create', method: 'POST', data });
export const urlRecordUpdate = (data: any): Promise<any> => FetchJson({ url: '/api/management/url_record/update', method: 'POST', data });
export const urlRecordDelete = (data: any): Promise<any> => FetchJson({ url: '/api/management/url_record/delete', method: 'POST', data });
// class UrlRecordApi {
//   async UrlRecordPageList(params: any): Promise<any> {
//     var url = `/api/management/url_record/page_list`;
//     let rsp = await FetchJson({
//       url: url,
//       method: "POST",
//       data: snakecaseKeys(params),
//     });

//     var res = camelcaseKeys({ data: rsp.data }, { deep: true });
//     return { ...res.data };
//   }

//   async UrlRecordGet(params: any): Promise<any> {
//     var url = `/api/management/url_record/get`;
//     let rsp = await FetchJson({
//       url: url,
//       method: "POST",
//       data: snakecaseKeys(params),
//     });

//     var res = camelcaseKeys({ data: rsp.data }, { deep: true });
//     return { ...res.data };
//   }

//   async UrlRecordUpdate(params: any): Promise<any> {
//     var url = `/api/management/url_record/update`;
//     let rsp = await FetchJson({
//       url: url,
//       method: "POST",
//       data: snakecaseKeys(params),
//     });

//     var res = camelcaseKeys({ data: rsp.data }, { deep: true });
//     return { ...res.data };
//   }
// }
// export const urlRecordApi = new UrlRecordApi();
