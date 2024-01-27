import { getUserInfo } from "@/api/post";
import create from "zustand";

interface AppStoreInfo {
    token: string, // token存储到sessionStoreage或者LocalStorage
    permissions: Array<string>,
    currentSiteId: string | null,
    loadPermissions: () => void,
    isAuthorized: () => boolean,
    login: (token: string) => void,
    logout: () => void,
    setCurrentSite: (siteId: string) => void
}

const tokenStoreKey = "token";
const siteIdStorKey = "siteId";
export const useAppStore = create<AppStoreInfo>((set, get) => (
    {
        token: localStorage.getItem(tokenStoreKey) ?? "",
        permissions: [],
        loadPermissions: async () => {
            console.log('loadPermissions');
            const response = await getUserInfo()
            set({ permissions: response?.data?.permissions ?? [] })
        },
        isAuthorized: () => {
            let { token: cachedToken } = get();
            return cachedToken !== null && cachedToken !== undefined && cachedToken.length > 0;
        },
        login: (token: string) => {
            localStorage.setItem(tokenStoreKey, token as string);
            set(() => ({ token }));
        },
        logout: () => {
            localStorage.removeItem(tokenStoreKey);
            set(() => ({}));
        },
        currentSiteId: localStorage.getItem(siteIdStorKey),
        setCurrentSite: (siteId: string) => {
            console.log('setCurrentSite', siteId.toString())
            set({ currentSiteId: siteId })
            localStorage.setItem(siteIdStorKey, siteId);
        }
    }
))