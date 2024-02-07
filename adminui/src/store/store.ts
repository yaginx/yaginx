import { create } from "zustand";
import { PersistOptions, StorageValue, persist } from "zustand/middleware";
export type Updater<T> = (updater: (value: T) => void) => void;
export function deepClone<T>(obj: T) {
  return JSON.parse(JSON.stringify(obj));
}

type SecondParam<T> = T extends (
  _f: infer _F,
  _s: infer S,
  ...args: infer _U
) => any
  ? S
  : never;

type MakeUpdater<T> = {
  lastUpdateTime: number;
  markUpdate: () => void;
  update: Updater<T>;
};

type SetStoreState<T> = (
  partial: T | Partial<T> | ((state: T) => T | Partial<T>),
  replace?: boolean | undefined,
) => void;

type MakeReload = {
  reload: () => void;
}

export function createShaddowStore<T, M>(
  defaultState: T,
  methods: (set: SetStoreState<T & MakeReload>, get: () => T & MakeReload) => M,
  persistOptions: SecondParam<typeof persist<T & M>>) {
  let result = create<T & M & MakeReload>((set, get) => ({
    ...defaultState,
    ...methods(set as any, get),
    reload() {
      let value = window.localStorage.getItem(persistOptions.name);
      if (!value)
        return;
      let storageValue: StorageValue<T> = JSON.parse(value ?? '{}');
      set(deepClone(storageValue?.state ?? { ...get() }));
    }
  }));
  result.getState().reload();
  return result;
}

export function createMemoryStore<T,M>(
  defaultState:T,
  methods: (
    set: SetStoreState<T & MakeUpdater<T>>,
    get: () => T & MakeUpdater<T>,
  ) => M
)
{
  return create<T & M & MakeUpdater<T>>()(
    (set, get) => {
      return {
        ...defaultState,
        ...methods(set as any, get),

        lastUpdateTime: 0,
        markUpdate() {
          set({ lastUpdateTime: Date.now() } as Partial<
            T & M & MakeUpdater<T>
          >);
        },
        update(updater) {
          const state = deepClone(get());
          updater(state);
          get().markUpdate();
          set(state);
        },
      };
    }
  );
}

export function createPersistStore<T, M>(
  defaultState: T,
  methods: (
    set: SetStoreState<T & MakeUpdater<T>>,
    get: () => T & MakeUpdater<T>,
  ) => M,
  persistOptions: SecondParam<typeof persist<T & M & MakeUpdater<T>>>,
) {
  return create<T & M & MakeUpdater<T>>()(
    persist((set, get) => {
      return {
        ...defaultState,
        ...methods(set as any, get),

        lastUpdateTime: 0,
        markUpdate() {
          set({ lastUpdateTime: Date.now() } as Partial<
            T & M & MakeUpdater<T>
          >);
        },
        update(updater) {
          const state = deepClone(get());
          updater(state);
          get().markUpdate();
          set(state);
        },
      };
    }, persistOptions),
  );
}
