import React, { useState } from 'react';
import Login from './Auth/Login';

const LandingPage = () => {
  const [showLogin, setShowLogin] = useState(true);

  const toggleForm = () => {
    setShowLogin(!showLogin);
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center mt-5">
        <div className="col-md-6">
          <Login />
        </div>
      </div>
    </div>
  );
};

export default LandingPage;
