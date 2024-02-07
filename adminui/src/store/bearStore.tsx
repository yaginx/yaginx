// 创建store
import { create } from 'zustand'

interface BearStoreType {
  bears: number,
  increasePopulation: () => void,
  removeAllBears: () => void,
}

export const bearStore = create<BearStoreType>((set, get) => ({
  bears: 0,
  increasePopulation: () => {
    set(state => ({ bears: state.bears + 1 }));
  },
  removeAllBears: () => set({ bears: 0 })
}))
