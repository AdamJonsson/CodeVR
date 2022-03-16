import React from 'react';
import logo from './logo.svg';
import './App.css';
import { UnityBlocklyPage } from './Pages/UnityBlocklyPage/UnityBlocklyPage';
import { Route, Routes } from 'react-router-dom';
import { HomePage } from './Pages/HomePage/HomePage';
import { WebsiteBlocklyPage } from './Pages/WebsiteBlocklyPage/WebsiteBlocklyPage';

function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/Unity" element={<UnityBlocklyPage />} />
      <Route path="/Unity/Debug" element={<UnityBlocklyPage />} />
      <Route path="/Website" element={<WebsiteBlocklyPage />} />
    </Routes>
  );
}

export default App;
