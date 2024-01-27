import React, { createRef, useState } from 'react';
import { Form, Input, Tabs, Radio, InputRef } from 'antd';
import { formItemDefaultLayout } from '@/componets/options';
import TextArea from 'antd/lib/input/TextArea';
import { RichContentEditor } from '@/componets/MarkdownEditor';
import { CategorySelectInput } from '@/componets/CategorySelectInput';
export const PostEditFormView: React.FC<any> = (props) => {
    const [editorType, setEditorType] = useState<number>(props.values?.contentFormat)
    const [slug, setSlug] = useState<string>(props.values?.slug)
    const slugRef = createRef<InputRef>();
    const tabSeoInfoItem = {
        label: 'SEO信息', key: 'seoInfo', forceRender: true, children:
            <>
                {/* <Form.Item name="postKind" label="类型" rules={[{ required: true, message: '类型' }]}>
                    <CategorySelectInput parentCategoryName={"POST_KIND"} />
                </Form.Item> */}
                <Form.Item name="primaryCategoryId" label="主分类" rules={[{ required: true, message: '主分类' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_CATEGORY"} />
                </Form.Item>
                <Form.Item name="relativeCategoryIds" label="关联分类" rules={[{ required: true, message: '关联分类' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_CATEGORY"} mode="multiple" />
                </Form.Item>
                <Form.Item name="keywordIds" label="关键字" rules={[{ required: true, message: '关键字' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_KEYWORD"} mode="multiple" />
                </Form.Item>
                <Form.Item name="authorId" label="作者" rules={[{ required: true, message: '作者' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_AUTHOR"} />
                </Form.Item>
            </>

    };
    const items = [
        {
            label: '页面信息', key: 'pageInfo', forceRender: true, children:
                <>
                    <Form.Item name="title" label="标题" rules={[{ required: true, message: '标题' }]}>
                        <Input onChange={(e) => { props.form.setFieldValue("slug", e.target.value.toLowerCase().replaceAll(" ", "-").replaceAll("[,:'`]", "")) }} />
                    </Form.Item>
                    <Form.Item name="slug" label="slug" rules={[{ required: true, message: '浏览器中的地址' }]}>
                        <Input ref={slugRef} />
                    </Form.Item>
                    <Form.Item name="preview" label="preview">
                        <TextArea rows={6} />
                    </Form.Item>
                    <Form.Item name="exposeImage" label="页面主图" >
                        <Input disabled />
                    </Form.Item>
                    <Form.Item name="contentFormat" initialValue={props.values?.contentFormat}>
                        <Radio.Group onChange={(v) => setEditorType(v.target.value)}>
                            <Radio.Button value={1}>HTML</Radio.Button>
                            <Radio.Button value={2}>Markdown</Radio.Button>
                        </Radio.Group>
                    </Form.Item>
                    <Form.Item name="content">
                        <RichContentEditor editorType={editorType} />
                    </Form.Item>
                </>
        }
    ];

    if (props.values?.postId) {
        items.push(tabSeoInfoItem);
    }
    return (
        <Form
            name="basic"
            layout="vertical"
            {...formItemDefaultLayout}
            {...props}
            autoComplete="off"
        >
            <Form.Item hidden name="postId">
                <Input type={"hidden"} />
            </Form.Item>
            <Form.Item hidden name="ts">
                <Input type={"hidden"} />
            </Form.Item>
            <Tabs items={items} />
        </Form>
    )
}