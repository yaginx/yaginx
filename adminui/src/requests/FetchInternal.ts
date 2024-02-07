import { FETCH_RETRY_DELAY, FETCH_TIMEOUT } from "./requestConsts";

export async function FetchInternal(
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
        setTimeout(fetchRequest, FETCH_RETRY_DELAY);
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

    if (FETCH_TIMEOUT) {
      const err = new Error("request timeout");
      if (timer) {
        clearTimeout(timer);
      }
      timer = setTimeout(reject, FETCH_TIMEOUT, err);
    }
  });
}
