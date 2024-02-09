import React from "react";
import { useState } from "react";

export const ParentModel: React.FC = () => {
  const [sum, setSum] = useState(0);

  const addSum = () => {
    setSum(sum + 1);
  }

  return (
    <>
      <div>当前数量为：{sum}</div>
      <div className="container" onClick={addSum}>点击加1</div>
      <ChildModal sum={sum} />
    </>
  );
}
export const ChildModal: React.FC<any> = (props: any) => {
  return (
    <>
      <div>子组件的数量:{props.sum}</div>
    </>
  );
}
