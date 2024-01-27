import React from "react";
import MdEditor from 'md-editor-rt';
import 'md-editor-rt/lib/style.css';
import { TinymceEditor } from "./TinymceEditor";
import { imageUpload } from "@/api/post";
const onUploadImg = async (files: any, callback: any) => {
    const res = await Promise.all(
        files.map((file: any) => {
            return new Promise((reslove, reject) => {
                const form = new FormData();
                form.append('file', file);
                imageUpload(form)
                    .then((res) => {
                        if (res.code == 200) {
                            reslove(res.data[0].url)//上传成功，在成功函数里填入图片路径
                        } else {
                            reject("上传失败")
                        }
                    })
                    .catch((error) => reject(error));
            });
        })
    );
    callback(res);
};
export const MarkdownEditor = ({ value, onChange }: any) => {
    return (
        <>
            <MdEditor modelValue={value} onChange={onChange} onUploadImg={onUploadImg} />
        </>
    )
};
// export enum EditorType {
//     Html = 1,
//     Markdown = 2,
// }
export const RichContentEditor = (props: any) => {
    // console.log("RichContentEditor" + props.editorType)
    switch (props.editorType) {
        case 2:
            return (<MarkdownEditor  {...props} />);

        case 1:
        default:
            return (<TinymceEditor {...props} />);
    }
};