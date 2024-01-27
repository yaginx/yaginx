import { notification } from "antd";
import qs from "qs";

const GM_FETCH_TIMEOUT = 30000;
const GM_FETCH_RETRY_DELAY = 1000;
async function FetchInternal(
  input: RequestInfo,
  init?: RequestInit,
  retryCount: number = 0
): Promise<any> {
  return new Promise((resolve, reject) => {
    let timer = undefined;
    const onResponse = (response: Response) => {
      if (!response.ok) {
        reject(new Error(response.statusText));
      }
      return response.json().then(resolve).catch(reject);
    };

    const onError = (error: any) => {
      retryCount--;
      if (retryCount) {
        setTimeout(fetchRequest, GM_FETCH_RETRY_DELAY);
      } else {
        reject(error);
      }
    };

    const rescueError = (error: any) => {
      console.warn(error);
      throw error;
    };

    function fetchRequest() {
      return fetch(input, init)
        .then(onResponse)
        .catch(onError)
        .catch(rescueError);
    }

    fetchRequest();

    if (GM_FETCH_TIMEOUT) {
      const err = new Error("request timeout");
      if (timer) {
        clearTimeout(timer);
      }
      timer = setTimeout(reject, GM_FETCH_TIMEOUT, err);
    }
  });
}

export async function Fetch(
  input: RequestInfo,
  init?: RequestInit,
  retryCount: number = 0
) {
  const additionalHeaders = new Headers();
  let token = getUserToken();
  if (token) {
    additionalHeaders.append('Authorization', `Bearer ${token}`);
  }

  let siteId = getSiteId();
  if (siteId) {
    additionalHeaders.append('X-SITE-ID', siteId);
  }
  
  const updatedHeaders = new Headers(init?.headers);
  for (const [name, value] of additionalHeaders.entries()) {
    updatedHeaders.append(name, value);
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
      const isErrorCode = `${error}` === `${code}`;
      return { code, msg: null, response: null };
    }
    throw err;
  }
}

const getUserToken = () => {
  const userToken = (window.localStorage.getItem("token") as string) || "";
  return userToken;
};

const getSiteId = () => {
  const siteId = (window.localStorage.getItem("siteId") as string) || "";
  return siteId;
};

const PREFIX: string = "";
export async function FetchJson({
  url,
  method,
  params,
  data,
}: {
  url?: string;
  method: string;
  params?: any;
  data?: any;
  token?: string;
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

  let resp = await Fetch(`${PREFIX}${url}`, opts);
  const { code, msg, errMsg } = resp;

  if (code !== 200) {
    console.log(`Json API: ${code}:${errMsg}`);
    if (code === 401) {
      console.log(`Json API access denied`);
      notification.open({
        message: `未授权: ${code}`,
        description: msg,
        duration: 10,
      });
      return;
    }
    // 远程调用ApiStatusCode错误
    console.log({ code, errMsg });
    notification.open({
      message: `业务调用失败: ${code},详细错误可以查看控制台`,
      description: `${msg}`,
      duration: 10,
    });

    throw { code, reason: errMsg };
  }

  return resp;
}
