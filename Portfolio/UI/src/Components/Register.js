import React, { useState } from 'react';
import axios from 'axios';

const Register = () => {
    const [user, setUser] = useState({
      FirstName: '',
      LastName: '',
      Email: '',
      Password: '',
      Address: '',
      City: '',
      Country: '',
      PhoneNumber: '',
      ProfilePicture: ''
    });
  
    const handleChange = (e) => {
      setUser({ ...user, [e.target.name]: e.target.value });
    };
  
    const handleSubmit = async (e) => {
      e.preventDefault();
  
      try {
        const response = await axios.post('http://localhost:64897/api/register', user);
        console.log(response.data);
        setUser({
          FirstName: '',
          LastName: '',
          Email: '',
          Password: ''
        });
      } catch (error) {
        console.error('Greška prilikom registracije:', error);
      }
    };
  
    return (
      <div>
        <h2>Registracija</h2>
        <form onSubmit={handleSubmit}>
          <div>
            <label>Ime:</label>
            <input type="text" name="FirstName" value={user.FirstName} onChange={handleChange} />
          </div>
          <div>
            <label>Prezime:</label>
            <input type="text" name="LastName" value={user.LastName} onChange={handleChange} />
          </div>
          <div>
            <label>Email:</label>
            <input type="email" name="Email" value={user.Email} onChange={handleChange} />
          </div>
          <div>
            <label>Lozinka:</label>
            <input type="password" name="Password" value={user.Password} onChange={handleChange} />
          </div>
          <button type="submit">Registruj se</button>
        </form>
      </div>
    );
  };
  
export default Register;