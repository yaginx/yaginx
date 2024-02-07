// pages/Home/index.tsx
import React, { useEffect, useState } from 'react';
import { Navigate, Outlet, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import { TableListView } from '@/componets/ModelCrudViews/TableListView';
import { ModelCreateView } from '@/componets/ModelCrudViews/ModelCreateView';
import { ModelEditView } from '@/componets/ModelCrudViews/ModelEditView';
import { schemaModelFullGet } from '@/api/modelSchemas';
import { BasicCrudIndexProps, ISchemaModelInfo } from '@/componets/ModelCrudViews/ModelCrudProps';

export const BasicCrudIndex: React.FC<BasicCrudIndexProps> = (props) => {
  const { modelName } = props;
  const [schemaModelInfo, setSchemaModelInfo] = useState<ISchemaModelInfo>({ modelName: modelName, pkFieldName: "", displayFieldName: "" });
  const fetchSchemaModel = async () => {
    var rsp = await schemaModelFullGet({ ...schemaModelInfo });
    if (rsp.code != 200) {
      return;
    }
    setSchemaModelInfo({ ...rsp.data, modelName: rsp.data.name });
  }
  useEffect(() => {
    fetchSchemaModel();
  }, [])
  return (
    <>
      <Routes>
        <Route index element={<Navigate to={"./list"} />} />
        <Route path="list" element={<TableListView tableDataFetchApi={props.search} tableDataDeleteApi={props.delete} revokeDelete={props.revokeDelete} {...schemaModelInfo} />} />
        <Route path="create" element={<ModelCreateView modelCreateSubmitApi={props.create} {...schemaModelInfo} />} />
        <Route path="edit/:id" element={<ModelEditView modelGetApi={props.get} modelEditSubmitApi={props.edit}  {...schemaModelInfo} />} />
      </Routes>
      <Outlet />
    </>
  )
}
