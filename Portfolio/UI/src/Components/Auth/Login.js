import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import '../../styles/AuthForm.css';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const response = await axios.post('http://localhost:64897/api/login', { email, password });

      if (response.status === 200) {
        localStorage.setItem('user', JSON.stringify({ email }));
        navigate('/portfolio');
      }
    } catch (error) {
      if (error.response && error.response.status === 401) {
        setError('Nevalidna email adresa ili lozinka.');
      } else {
        setError('Greška pri prijavi korisnika.');
      }
    }
  };

  return (
    <div className="Auth-form-container">
      <form className="Auth-form" onSubmit={handleSubmit}>
        <div className="Auth-form-content">
          <h3 className="Auth-form-title">Sign In</h3>
          <div className="form-group label-input-spacing">
            <label>Email address</label>
            <input
              type="email"
              className="form-control input-margin-left"
              placeholder="Enter email"
              value={email}
              onChange={handleEmailChange}
            />
          </div>
          <div className="form-group label-input-spacing">
            <label>Password</label>
            <input
              type="password"
              className="form-control password-input-margin-left"
              placeholder="Enter password"
              value={password}
              onChange={handlePasswordChange}
            />
          </div>
          <div className="d-grid gap-2">
            <button type="submit" className="btn btn-primary">
              Submit
            </button>
          </div>
          <br />
          <p className="register text-right mt-2">
            Don't have an account? <a href="/register">Register</a>
          </p>
        </div>
      </form>
    </div>
  );
};

export default Login;
