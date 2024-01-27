// pages/Home/index.tsx
import React, { useState } from 'react';
import { Form, Input, Tabs, Radio } from 'antd';
import { formItemDefaultLayout } from '@/componets/options';
import TextArea from 'antd/lib/input/TextArea';
import { RichContentEditor } from '@/componets/MarkdownEditor';
import { CategorySelectInput } from '@/componets/CategorySelectInput';
const PostEditForm: React.FC<any> = (props) => {
    const [editorType, setEditorType] = useState<number>(props.values?.contentFormat)
    const [isPublished, setPublishStatus] = useState<boolean>(props.values?.isPublished)
    const tabSeoInfoItem = {
        label: '附加信息', key: 'extraInfo', forceRender: true, children:
            <>
                {/* <Form.Item name="primaryCategoryId" label="主分类" rules={[{ message: '主分类' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_CATEGORY"} />
                </Form.Item>
                <Form.Item name="relativeCategoryIds" label="关联分类" rules={[{ message: '关联分类' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_CATEGORY"} mode="multiple" />
                </Form.Item>
                <Form.Item name="keywordIds" label="关键字" rules={[{ message: '关键字' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_KEYWORD"} mode="multiple" />
                </Form.Item>
                <Form.Item name="authorId" label="作者" rules={[{ message: '作者' }]}>
                    <CategorySelectInput parentCategoryName={"BLOG_AUTHOR"} mode="tags" tokenSeparators={[',']} />
                </Form.Item> */}
                {/* <Form.Item name="isPublished" label="发布状态">
                    <Switch checkedChildren="已发布" unCheckedChildren="未发布" checked={isPublished} />
                </Form.Item> */}
                {/* <Form.Item name="publishTime" label="发布时间">
                    <DatePicker />
                </Form.Item> */}
            </>
    };
    const items = [
        {
            label: '页面信息', key: 'pageInfo', forceRender: true, children:
                <>
                    <Form.Item name="postKind" label="类型" rules={[{ required: true, message: '类型' }]}>
                        <CategorySelectInput parentCategoryName={"POST_KIND"} />
                    </Form.Item>
                    <Form.Item name="title" label="标题" rules={[{ required: true, message: '标题' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="slug" label="slug">
                        <Input />
                    </Form.Item>
                    <Form.Item name="keywords" label="keywords">
                        <CategorySelectInput parentCategoryName={"BLOG_KEYWORD"} mode="tags" tokenSeparators={[',']} />
                    </Form.Item>
                    <Form.Item name="preview" label="preview">
                        <TextArea rows={6} />
                    </Form.Item>
                    <Form.Item name="exposeImage" label="页面主图" >
                        <Input disabled />
                    </Form.Item>
                    <Form.Item name="contentFormat" label="ContentFormat" initialValue={props.values?.contentFormat}>
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
export default PostEditForm;