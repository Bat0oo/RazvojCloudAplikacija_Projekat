// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Register from './Components/Auth/Register';
import Login from './Components/Auth/Login';
import User from './Components/User';
import PortfolioPage from './Components/PortfolioPage';
import LandingPage from './Components/LandingPage';
import Profile from './Components/Profile';
import Alarm from './Components/Alarm';

function App() {
  return (
    <Router>
  <Routes>
    <Route exact path="/" element={<LandingPage />} />
    <Route exact path="/login" element={<Login />} />
    <Route exact path="/register" element={<Register />} />
    <Route exact path="/user" element={<User />} />
    <Route exact path="/portfolio" element={<PortfolioPage />} />
    <Route exact path="/profile" element={<Profile />} />
    <Route exact path="/alarm" element={<Alarm />} />
  </Routes>
</Router>
  );
}

export default App;
