import { notification } from "antd";
import { FetchInternal } from "./FetchInternal";
import { getHeaders } from "./getHeaders";


export async function Fetch(
  input: RequestInfo,
  init?: RequestInit,
  retryCount: number = 0,
  authHeader?: Record<string, string>
) {

  const authorizationHeaders = authHeader ?? getHeaders();
  const updatedHeaders = new Headers(init?.headers);
  for (const key in authorizationHeaders) {
    const value = authorizationHeaders[key];
    updatedHeaders.append(key, value);
  }
  // for (const [name, value] of additionalHeaders.entries()) {
  //   updatedHeaders.append(name, value);
  // }

  const additionalHeaders: Record<string, string> = {
    // "Content-Type": "application/json",
    "x-requested-with": "XMLHttpRequest",
  };
  for (const key in additionalHeaders) {
    const value = additionalHeaders[key];
    updatedHeaders.append(key, value);
  }

  const updatedRequestOptions = {
    ...init,
    headers: updatedHeaders
  };

  try {
    return await FetchInternal(input, updatedRequestOptions, retryCount);
  } catch (err: any) {
    // 远程调用HttpStatusCode错误
    console.log(err);
    notification.open({
      message: "Remote Http Error ",
      description: err.message,
      duration: 15,
    });
    if (err && err.message === "request timeout") {
      const code = 408;
      const error = "request timeout";
      return { code, msg: null, response: null };
    }
    throw err;
  }
}
