import React, { useState, useEffect } from 'react';
import axios from 'axios';

const Profile = () => {
  const [user, setUser] = useState(null);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    address: '',
    city: '',
    country: '',
    phoneNumber: '',
    password: '',
    profilePicture: ''
  });

  useEffect(() => {
    const userEmail = localStorage.getItem('user');
    if (userEmail) {
      axios.get(`http://localhost:64897/api/getUser/${userEmail}`)
        .then(response => {

            console.log('firstName:', response.data.firstName);
            console.log('lastName:', response.data.lastName);
            console.log('address:', response.data.address);

          setUser(response.data);
          setFormData({
            firstName: response.data.firstName,
            lastName: response.data.lastName,
            address: response.data.address,
            city: response.data.city,
            country: response.data.country,
            phoneNumber: response.data.phoneNumber,
            password: response.data.password,
            profilePicture: response.data.profilePicture
          });
        })

        .catch(error => {
          console.error('Error fetching user data:', error);
        });
    }
  }, []);

  const handleChange = e => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      const response = await axios.put('http://localhost:64897/api/editProfile', formData);
      console.log(response.data);
    } catch (error) {
      console.error('Error updating user:', error);
    }
  };

  return (
    <div>
      <h2>Profil korisnika</h2>
      <form onSubmit={handleSubmit}>
        <label>
          Ime:
          <input type="text" name="firstName" value={formData.firstName} onChange={handleChange} />
        </label>
        <label>
          Prezime:
          <input type="text" name="lastName" value={formData.lastName} onChange={handleChange} />
        </label>
        <label>
          Adresa:
          <input type="text" name="address" value={formData.address} onChange={handleChange} />
        </label>
        <label>
          Grad:
          <input type="text" name="city" value={formData.city} onChange={handleChange} />
        </label>
        <label>
          Država:
          <input type="text" name="country" value={formData.country} onChange={handleChange} />
        </label>
        <label>
          Broj telefona:
          <input type="text" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} />
        </label>
        <label>
          Lozinka:
          <input type="password" name="password" value={formData.password} onChange={handleChange} />
        </label>
        <label>
          Slika profila:
          <input type="file" name="profilePicture" value={formData.profilePicture} onChange={handleChange} />
        </label>
        <button type="submit">Sačuvaj izmene</button>
      </form>
    </div>
  );
};

export default Profile;
