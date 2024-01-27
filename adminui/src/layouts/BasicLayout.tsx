import React from "react";
import { Outlet, } from "react-router-dom";
import { Layout } from "antd";
import { Footer, Header } from "antd/es/layout/layout";
import { VITE_APP_TITLE } from '@c/constant';

const { Content } = Layout;

const BasicLayout: React.FC = () => {
    return (
        <Layout>
            <Header className={"layout_header"} style={{ userSelect: "none", height: "45px", lineHeight: "45px" }}>
                <div className={"layout_header_left"}>
                    <div className="logo">{VITE_APP_TITLE}</div>
                </div>
            </Header >
            <Layout>
                <Content style={{ padding: 12, margin: 0 }}>
                    <Outlet />
                </Content>
            </Layout>
            <Footer className={"layout_footer"} style={{ userSelect: "none", padding: "0 10px", lineHeight: "35px" }}>
                <div className='footer_left'>
                </div>
                <div className='footer_middle'>
                    <a href='https://feinian.net' target={"_blank"}>Feinian Studio</a> <span> ©2022 based on </span> <a href='https://ant-design.antgroup.com/components/overview-cn/' target={"_blank"}>Ant Design</a>
                </div>
            </Footer>
        </Layout>
    );
};

export default BasicLayout;