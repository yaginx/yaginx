import React, { useEffect, useState } from 'react';
import { Button, Dropdown } from 'antd';
import { useNavigate } from 'react-router-dom';

export interface TableOperationMenuItem {
  commandKey: string;
  commandLabel: string;
}
export interface TableOperationProps {
  record: any;
  menuOptions: TableOperationMenuItem[];
  onMenuClick: (commandKey: string, record: any) => void;
  onComplete: (record: any) => void;
}
export const TableOperations: React.FC<TableOperationProps> = (props: TableOperationProps) => {
  const { record, menuOptions, onMenuClick, onComplete } = props;
  const [items, setItems] = useState<Array<any>>([]);
  const [mainAction, setMainAction] = useState<any>({});
  const onMenuClickInternal = (e: any) => {
    if (onMenuClick)
      onMenuClick(e.key, record);
    if (onComplete)
      onComplete(record);
  };

  useEffect(() => {
    let tempItems: any[] = [];
    menuOptions.forEach(item => {
      tempItems.push({ key: item.commandKey, label: item.commandLabel });
    });
    setMainAction({ ...tempItems[0] });
    setItems(tempItems.splice(1, tempItems.length - 1));
  }, [record])
  return items.length > 0
    ? <Dropdown.Button menu={{ items, onClick: onMenuClickInternal }} onClick={() => onMenuClickInternal({ key: mainAction.key })}>{mainAction.label}</Dropdown.Button>
    : <Button onClick={() => onMenuClickInternal({ key: mainAction.key })}>{mainAction.label}</Button>
}
