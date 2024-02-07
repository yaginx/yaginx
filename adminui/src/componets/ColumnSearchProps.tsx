import { useEffect, useRef } from "react";
import { Space, Button, Input, Tooltip, Alert, Popover } from "antd";
import { SearchOutlined } from "@ant-design/icons";
import { FilterDropdownProps } from "antd/lib/table/interface";
import React from "react";

//自定义的下来搜索
const ColumnSearchProps = (dataIndex: string, title: string, searchText: string = "", searchedColumn?: string) => {
  interface DropdownContentProps extends FilterDropdownProps {
    title: string
  }
  function DropdownContent(props: DropdownContentProps) {
    const { selectedKeys, setSelectedKeys, confirm, clearFilters, title } = props;
    const searchInput = useRef<any>(null);
    useEffect(() => {
      searchInput?.current?.select();
    }, [searchInput]);
    return (
      <div style={{ padding: 8 }}>
        <Input
          ref={searchInput}
          placeholder={`请输入${title}`}
          value={selectedKeys[0]}
          onChange={(e) => setSelectedKeys(e.target.value ? [e.target.value] : [])}
          onPressEnter={() => confirm({ closeDropdown: true })}
          style={{ marginBottom: 8, display: "block" }}
        />
        <Space>
          <Button type="primary" onClick={() => confirm({ closeDropdown: true })} icon={<SearchOutlined />} size="small" style={{ width: 90 }}>搜索</Button>
          <Button onClick={() => { clearFilters?.(); confirm(); }} size="small" style={{ width: 90 }} disabled={selectedKeys !== undefined && selectedKeys.length < 1}>重置</Button>
        </Space>
      </div>
    );
  };

  return ({
    filterDropdown: (props: FilterDropdownProps) => (<DropdownContent {...props} title={title} />),
    filterIcon: (filtered: any) => (
      <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
    ),
    render: (text: any) => {
      if (text?.length > 30 ?? false) {
        return <Popover content={text} trigger="click" >
          <Button>show content</Button>
        </Popover >
      }
      else {
        return (text)
      }
      // return searchedColumn === dataIndex ? (
      //   // <Highlighter
      //   //     highlightStyle={{ backgroundColor: '#22c069', padding: 100 }}
      //   //     searchWords={[searchText]}
      //   //     autoEscape
      //   //     textToHighlight={text ? text.toString() : ''}
      //   // />
      //   text
      // ) : (text)
    }
  });
}
export default ColumnSearchProps;
