export class PaginationRequestParams {
    pageIndex: number = 1;
    pageSize: number = 10;
}
export interface PageResult<T>{
  paging:any;
  records:any;
}
export interface IApiRspEnvelop<T> {
  data: T;
  code: number;
  msg: string;
  errMsg: string;
  tid: string;
}
