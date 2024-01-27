import { useEffect, useRef, useState } from "react";
import { Form, Input, Button, Space, notification, Tabs } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { useNavigate, useParams } from "react-router-dom";
import TextArea from "antd/lib/input/TextArea";
import { CategorySelectInput, EnumSelectInput } from "./SearchInput";
import JoditEditor from "jodit-react";
import { useLocation } from "react-router";
import React from "react";
import { CategoryKind } from "@/PageCategoryKind";
import { ArticleGet, ArticleModify } from "@/api/articleApi";

const { TabPane } = Tabs;
export function Edit() {
    let { pageId } = useParams<any>();
    const [viewModel, setViewModel] = useState<any>({ pageId });
    const [pageType, setPageType] = useState();
    const [form] = Form.useForm();
    const navigate = useNavigate();
    const location = useLocation();
    const editorConfig = {
        readonly: false // all options from https://xdsoft.net/jodit/doc/
    }
    const onFinish = async (values: any) => {
        console.log('Success:', values);
        await ArticleModify({ ...values })
        notification.info({
            message: `更新成功`,
            description: `更新成功`
        });
        await fetchArticle();
    };

    const onFinishFailed = (errorInfo: any) => {
        console.log('Failed:', errorInfo);
    };
    const fetchArticle = async () => {
        var result = await ArticleGet({ pageId });
        // result = camelcaseKeys(result);
        var newValue = { ...viewModel, ...result.data }
        setViewModel(newValue);
        setPageType(newValue.pageType);
        form.setFieldsValue(newValue);
    }
    useEffect(() => {
        fetchArticle();
    }, [])
    const formItemDefaultLayout = {
        labelCol: { span: 3 },
        wrapperCol: { span: 24 },
    }
    const options = {
        selectOnLineNumbers: true
    };
    const handleChangeSlug = () => {

    }
    const handlePreview = () => {

    }
    const editor = useRef(null)
    const [content, setContent] = useState('')
    return (
        <PageHeader title={`${viewModel.title}`} subTitle={`${viewModel.categoryText}`} onBack={() => navigate("../")} extra={
            <Space size="middle">
                <Button onClick={() => form.submit()}>Save</Button>
                <a className={"ant-btn"} href={`/${viewModel.metaSlug}?useCache=false`} target="_blank">预览</a>
                <Button onClick={() => handleChangeSlug()}>修改URL</Button>
                <Button danger >Delete</Button>
            </Space>}>
            <Form
                name="basic"
                layout="vertical"
                form={form}
                {...formItemDefaultLayout}
                initialValues={viewModel}
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
                autoComplete="off"
            >
                <Form.Item hidden name="pageId">
                    <Input type={"hidden"} />
                </Form.Item>
                <Form.Item hidden name="ts">
                    <Input type={"hidden"} />
                </Form.Item>
                <Tabs defaultActiveKey="1" >
                    <TabPane tab="页面信息" key="pageInfo" forceRender={true}>
                        <Form.Item name="pageType" label="类型" rules={[{ required: true, message: '类型' }]}>
                            <EnumSelectInput disabled enumType="PageType" onChange={(value: any) => setPageType(value)} />
                        </Form.Item>

                        <Form.Item name="categoryId" label="分类" rules={[{ required: true, message: '所属分类' }]}>
                            <CategorySelectInput categoryKind={CategoryKind.PageCategory} typeLevel1={pageType} />
                        </Form.Item>
                        <Form.Item name="tagIds" label="标签" rules={[{ required: true, message: '所属标签' }]}>
                            <CategorySelectInput categoryKind={CategoryKind.PageTag} typeLevel1={pageType} mode="multiple" />
                        </Form.Item>
                        <Form.Item name="title" label="标题" rules={[{ required: true, message: '标题' }]}>
                            <Input />
                        </Form.Item>
                        <Form.Item name="authorId" label="作者" rules={[{ required: true, message: '作者' }]}>
                            <CategorySelectInput categoryKind={CategoryKind.PageAuthor} />
                        </Form.Item>
                        <Form.Item name="smallImage" label="列表页缩略图">
                            <Input />
                        </Form.Item>
                        <Form.Item name="publishStatus" label="publishStatus" rules={[{ required: true, message: 'publishStatus' }]}>
                            <EnumSelectInput enumType="PublishStatus" />
                        </Form.Item>
                    </TabPane>
                    <TabPane tab="SEO元数据" key="seo_metadata" forceRender={true}>
                        <Form.Item name="metaTitle" label="metaTitle" rules={[{ required: true, message: '页面Header中的Title' }]}>
                            <Input />
                        </Form.Item>
                        <Form.Item name="metaSlug" label="metaSlug" rules={[{ required: true, message: '浏览器中的地址' }]}>
                            <Input disabled />
                        </Form.Item>
                        <Form.Item name="metaKeywordIds" label="metaKeywords">
                            <CategorySelectInput categoryKind={CategoryKind.PageTag} mode="multiple" maxTagCount={5} />
                        </Form.Item>
                        <Form.Item name="metaDescription" label="Description">
                            <TextArea rows={6} />
                        </Form.Item>
                    </TabPane>
                    <TabPane tab="内容" key="content" forceRender={true}>
                        <Form.Item name="content" label="Content">
                            <JoditEditor
                                ref={editor}
                                value={content}
                                config={editorConfig}
                                //tabIndex={1} // tabIndex of textarea
                                onBlur={newContent => setContent(newContent)} // preferred to use only this option to update the content for performance reasons
                                onChange={newContent => { }}
                            />
                        </Form.Item>
                    </TabPane>
                </Tabs>

            </Form>
        </PageHeader >
    )

}

function camelcaseKeys(result: any): any {
    throw new Error("Function not implemented.");
}
