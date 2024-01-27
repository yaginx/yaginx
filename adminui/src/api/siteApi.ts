import { FetchJson } from "@/utils/request";
export const SiteSelectList = (data: any = {}): Promise<any> => FetchJson({ url: '/api/management/site/select_list', method: 'POST', data });
export const getUserInfo = (data: any): Promise<any> => FetchJson({ url: '/api/management/account/user_info', method: 'GET', data });

// class SiteApi {
//   async SiteSelectList(params: any = null): Promise<Array<EnumNumberResponse>> {
//     var url = `/api/management/site/select_list`;
//     let request = {
//       url: url,
//       method: "POST",
//       data: {},
//     };
//     if (params) request.data = snakecaseKeys(params);
//     let rsp = await FetchJson(request);
//     var res = camelcaseKeys({ data: rsp.data }, { deep: true });
//     return res.data;
//   }
// }
// export const siteApi = new SiteApi();
