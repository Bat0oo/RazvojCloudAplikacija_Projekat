// App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Register from './Components/Register';
import User from './Components/User';

function App() {
  return (
    <Router>
  <Routes>
    <Route exact path="/" element={<Register />} />
    <Route exact path="/user" element={<User />} />
  </Routes>
</Router>
  );
}

export default App;
