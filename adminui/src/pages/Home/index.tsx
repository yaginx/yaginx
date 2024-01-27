// pages/Home/index.tsx
import { bearStore } from '@/store/bearStore';
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
    return (
        <>
            <BearCounter />
            <Controls />
        </>
    )
};

export default Home;

