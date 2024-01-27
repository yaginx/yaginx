import moment from "moment";
import React, { useEffect, useRef } from "react";
import mermaid from 'mermaid';
export const LocalTime = ({ timestamp, format }: { timestamp: number | null, format?: string | null }) => {
    if (format == null) {
        format = 'YYYY-MM-DD HH:mm:ss';
    }
    if (timestamp !== null) {
        return <>{moment(timestamp * 1000).local().format(format)}</>;
    }
    else {
        return <></>
    }
};
const randomid = () => parseInt(String(Math.random() * 1e15), 10).toString(36);
const getCode = (arr: any = []) =>
  arr
    .map((dt: any) => {
      if (typeof dt === 'string') {
        return dt;
      }
      if (dt.props && dt.props.children) {
        return getCode(dt.props.children);
      }
      return false;
    })
    .filter(Boolean)
    .join('');
const Code = ({ inline, children = [], className, ...props }: any) => {
    const demoid = useRef(`dome${randomid()}`);
    const code = getCode(children);
    const demo = useRef(null);
    useEffect(() => {
        if (demo.current) {
            try {
                const str = mermaid.render(demoid.current, code, () => null, demo.current);
                // @ts-ignore
                demo.current.innerHTML = str;
            } catch (error) {
                // @ts-ignore
                demo.current.innerHTML = error;
            }
        }
    }, [code, demo]);

    if (
        typeof code === 'string' &&
        typeof className === 'string' &&
        /^language-mermaid/.test(className.toLocaleLowerCase())
    ) {
        return (
            <code ref={demo}>
                <code id={demoid.current} style={{ display: 'none' }} />
            </code>
        );
    }
    return <code className={String(className)}>{children}</code>;
};