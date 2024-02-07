import { gwGetHeaders } from "./getHeaders";
import { createPersistStore } from "./store";
export enum StoreKey {
  AuthInfo = "auth_info"
}

let authCheckState = 0; // 0 not fetch, 1 fetching, 2 done
export interface IAuthInfo {
  userId: number,
  name: string,
  tokenExpireTime: number,
  expiresIn: number,
  token: string,
  maxValidSeconds: number,
}

export const nowts = () => Math.floor(Date.now() / 1000);
export const DEFAULT_AUTH_INFO: IAuthInfo = {
  tokenExpireTime: Math.floor(Date.now() / 1000),
  expiresIn: 0,
  token: "",
  userId: 0,
  name: "",
  maxValidSeconds: 0,
}

const DEFAULT_ACCESS_STATE = {
  isCheckAuthLoopEnable: false,
  isLoginSuccess: false,
  tokenExpireTime: Math.floor(Date.now() / 1000),
  token: "",
  userId: 0,
  name: "",
  maxValidSeconds: 0,
};

export const useAccessStore = createPersistStore(
  { ...DEFAULT_ACCESS_STATE },

  (set, get) => ({
    emptyToken() {
      set(() => ({
        token: "",
        tokenExpireTime: 0,
        userId: 0,
        name: "",
        isLoginSuccess: false,
        isCheckAuthLoopEnable: false
      }));
    },

    updateToken(authInfo: IAuthInfo) {
      const { token, expiresIn, userId, name } = authInfo;
      if (token.length <= 0) return;
      set(() => ({
        token: token?.trim(),
        tokenExpireTime: (Math.floor(Date.now() / 1000) + Number.parseInt(expiresIn.toString()) / 6 * 5), // 提前到5/6的时间开始刷新Token
        maxValidSeconds: Number.parseInt(expiresIn.toString()) / 6 * 5,
        userId: userId,
        name: name,
        isLoginSuccess: true,
        isCheckAuthLoopEnable: true
      }));
    },

    isAuthorized() {
      this.checkAuthInfo();
      return get().isLoginSuccess;
    },

    authInfo(): IAuthInfo {
      const obj = get();
      return {
        token: obj.token,
        tokenExpireTime: obj.tokenExpireTime,
        expiresIn: Math.floor(Date.now() / 1000) - obj.tokenExpireTime,
        userId: obj.userId,
        name: obj.name,
        maxValidSeconds: obj.maxValidSeconds,
      }
    },

    checkAuthInfo() {
      if (get().isCheckAuthLoopEnable === false) return false;
      const currentTimeInSeconds = Math.floor(Date.now() / 1000);
      if (authCheckState < 1 && get().tokenExpireTime <= currentTimeInSeconds) {
        // console.debug("token will be expired, start refresh token");
        authCheckState = 1;
      }

      if (authCheckState !== 1) return;
      authCheckState = 2;
      console.debug("start call refresh token from server");
      fetch("/api/account/refresh_token", {
        method: "get",
        body: null,
        headers: {
          ...gwGetHeaders(),
        },
      }).then((res) => {
        console.debug("got refresh token result from server", res);
        if (res.status !== 200) {
          this.emptyToken();
          return;
        }
        else {
          res.json().then((rsp): any => {
            if (rsp.code && rsp.code === 200) {
              this.updateToken({ ...DEFAULT_AUTH_INFO, ...rsp.data });
            }
            else {
              this.emptyToken();
            }
          });
        }
      }).catch(error => {
        console.error("check autoifno error," + error.message);
      }).finally(() => {
        authCheckState = 0;
      });
    },
  }),
  {
    name: StoreKey.AuthInfo,
    version: 1,
  },
);
