// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
import { theme } from 'antd';
import React from 'react';
// 组件绑定
function BearCounter() {
  const bears = bearStore((state) => state.bears)
  return <h1>{bears} around here ...</h1>
}

function Controls() {
  const increasePopulation = bearStore((state) => state.increasePopulation)
  return <button onClick={increasePopulation}>one up</button>
}

const Home: React.FC = () => {

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  return (
    <>
      <BearCounter />
      <Controls />
      <div
        style={{
          padding: 24,
          textAlign: 'center',
          background: colorBgContainer,
          borderRadius: borderRadiusLG,
        }}
      >
        <p>long content</p>
        {
          // indicates very long content
          Array.from({ length: 100 }, (_, index) => (
            <React.Fragment key={index}>
              {index % 20 === 0 && index ? 'more' : '...'}
              <br />
            </React.Fragment>
          ))
        }
      </div>
    </>
  )
};

export default Home;

