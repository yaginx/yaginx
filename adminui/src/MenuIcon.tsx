import { createFromIconfontCN, HomeOutlined, WarningOutlined } from '@ant-design/icons';
import * as icons from '@ant-design/icons';
import loadable from "@loadable/component";
import React from 'react';
import ErrorBoundary from 'antd/lib/alert/ErrorBoundary';
const IconFont = createFromIconfontCN({
    scriptUrl: [
        '//at.alicdn.com/t/font_2343771_edythimws3e.js', // icon-set icon-home icon-usercenter
        '//at.alicdn.com/t/font_2343771_phce2dke40s.js', // icon-shoppingcart, icon-python
    ],
});

const DynamicIcon = loadable((props: any) =>
    import(/* @vite-ignore */`@ant-design/icons/${props.name}.js`)
        .catch(err => {
            console.log(`icon name:${props.name} not found`);
            return import(`@ant-design/icons/WarningOutlined.js`)
        }), {
    cacheKey: (props: any) => props.name,
})
const DynamicIcon2 = (props: any) => {
    if (antIcon[props.name])
        return React.createElement(antIcon[props.name])
    else
        return <WarningOutlined style={{ color: '#f66' }} />;
}
const antIcon: { [key: string]: any } = icons;
const MenuIcon = (props: { name: string | undefined; }) => {
    if (props.name) {
        // return props.name.startsWith("icon-") ? <IconFont type={props.name} style={{ fontSize: '22px' }} /> : <DynamicIcon {...props} style={{ fontSize: '22px' }} />;
        return props.name.startsWith("icon-") ? <IconFont type={props.name} style={{ fontSize: '22px' }} /> : <DynamicIcon2 {...props} style={{ fontSize: '22px' }} />;
    }
    else {
        return <DynamicIcon name="WarningOutlined" style={{ fontSize: '22px', color: '#f66' }} />
    }
}
export default MenuIcon;