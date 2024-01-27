import { useState } from "react";

export const useLocalStorage = (keyName: string, defaultValue: any) => {
  const [storedValue, setStoredValue] = useState(() => {
    try {
      const value = window.localStorage.getItem(keyName);
      if (value) {
        try {
          return JSON.parse(value);
        }
        catch (err) {
          return value;
        }
      } else {
        if (typeof defaultValue === 'string') {
          window.localStorage.setItem(keyName, defaultValue);
          return defaultValue;
        } else {
          window.localStorage.setItem(keyName, JSON.stringify(defaultValue));
          return defaultValue;
        }
      }
    } catch (err) {
      return defaultValue;
    }
  });

  const setValue = (newValue: any) => {
    try {
      if (typeof newValue === 'string') {
        window.localStorage.setItem(keyName, newValue);
      } else {
        window.localStorage.setItem(keyName, JSON.stringify(newValue));
      }
    } catch (err) {}
    setStoredValue(newValue);
  };

  return [storedValue, setValue];
};
