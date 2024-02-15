import { notification } from "antd";
import qs from "qs";
import { Fetch } from "./Fetch";
import { PREFIX } from "./requestConsts";

export async function FetchJson({
  url, method, params, data, retryCount = 0, authHeader = undefined, ignoreGlobalError = false
}: {
  url?: string;
  method: string;
  params?: any;
  data?: any;
  retryCount?: number;
  authHeader?: Record<string, string>;
  ignoreGlobalError?: boolean;
}) {
  const opts: any = {
    method,
    headers: {
      "Content-Type": "application/json",
    },
  };

  // process params
  if (params) {
    var queryString = qs.stringify(params);
    url += "?" + queryString;
  }

  // process data
  switch (method) {
    case "POST":
    case "PUT":
      if (!data) {
        data = {};
      }
      opts.body = JSON.stringify(data);
      break;
    default:
      if (data) {
        var queryString = qs.stringify(data);
        url += "?" + queryString;
      }
      break;
  }

  let resp = await Fetch(`${PREFIX}${url}`, opts, retryCount, authHeader);
  const { code, msg, errMsg } = resp;
  if (!ignoreGlobalError) {
    if (code !== 200) {
      console.log(`Json API: ${code}:${errMsg}`);
      if (code === 401) {
        console.log(`Json API access denied`);
        notification.open({
          message: `未授权: ${code}`,
          description: msg,
          duration: 5,
        });
        return;
      }
      // 远程调用ApiStatusCode错误
      console.log({ code, errMsg });
      notification.open({
        message: `业务调用失败: ${code},详细错误可以查看控制台`,
        description: `${msg}`,
        duration: 5,
      });

      throw { code, reason: errMsg };
    }
  }
  return resp;
}
