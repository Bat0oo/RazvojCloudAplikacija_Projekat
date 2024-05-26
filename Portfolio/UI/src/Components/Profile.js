import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Header from './Header';
import '../styles/Profile.css';

const Profile = () => {
  const [userEmail, setUserEmail] = useState(null);
  const [user, setUser] = useState(null);
  const [editMode, setEditMode] = useState(false);

  useEffect(() => {
    fetchUserData();
  }, []);
  
  const fetchUserData = () => {
    const userData = JSON.parse(localStorage.getItem('user'));
    if (userData) {
      const email = userData.email;
      setUserEmail(email);
      axios.get(`http://localhost:5000/api/getUser?email=${email}`)
        .then(response => {
          setUser(response.data);
        })
        .catch(error => {
          console.error('Error fetching user data:', error);
        });
    }
  };
  

  const handleEdit = () => {
    setEditMode(true);
  };

  const handleSaveChanges = () => {
    setEditMode(false);

    const userData = {
        FirstName: user.FirstName,
        LastName: user.LastName,
        Address: user.Address,
        City: user.City,
        Country: user.Country,
        PhoneNumber: user.PhoneNumber,
        ProfilePicture: user.ProfilePicture
    };

        axios.put(`http://localhost:5000/api/editProfile?email=${userEmail}`, userData)
        .then(response => {
            console.log(response.data);
        })
        .catch(error => {
            console.error('Error updating user data:', error);
        });
};

  const handleInputChange = (e, field) => {
    setUser({ ...user, [field]: e.target.value });
  };

  return (
    <div>
      <Header />
      {user && (
        <div className="profile-container">
          <h2>Profil korisnika</h2>
          <table className="profile-table">
            <tbody>
              <tr>
                <td>Ime:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.FirstName}
                      onChange={(e) => handleInputChange(e, 'FirstName')}
                    />
                  ) : (
                    user.FirstName
                  )}
                </td>
              </tr>
              <tr>
                <td>Prezime:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.LastName}
                      onChange={(e) => handleInputChange(e, 'LastName')}
                    />
                  ) : (
                    user.LastName
                  )}
                </td>
              </tr>
              <tr>
                <td>Email:</td>
                <td>{user.Email}</td>
              </tr>
              <tr>
                <td>Adresa:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.Address}
                      onChange={(e) => handleInputChange(e, 'Address')}
                    />
                  ) : (
                    user.Address
                  )}
                </td>
              </tr>
              <tr>
                <td>Grad:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.City}
                      onChange={(e) => handleInputChange(e, 'City')}
                    />
                  ) : (
                    user.City
                  )}
                </td>
              </tr>
              <tr>
                <td>Država:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.Country}
                      onChange={(e) => handleInputChange(e, 'Country')}
                    />
                  ) : (
                    user.Country
                  )}
                </td>
              </tr>
              <tr>
                <td>Broj telefona:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.PhoneNumber}
                      onChange={(e) => handleInputChange(e, 'PhoneNumber')}
                    />
                  ) : (
                    user.PhoneNumber
                  )}
                </td>
              </tr>
              <tr>
                <td>Slika profila:</td>
                <td>
                  {editMode ? (
                    <input
                      type="text"
                      value={user.ProfilePicture}
                      onChange={(e) => handleInputChange(e, 'ProfilePicture')}
                    />
                  ) : (
                    user.ProfilePicture
                  )}
                </td>
              </tr>
            </tbody>
          </table>
          {editMode ? (
            <button className="btn btn-primary" onClick={handleSaveChanges}>Sačuvaj promene</button>
          ) : (
            <button className="btn btn-secondary" onClick={handleEdit}>Uredi</button>
          )}
        </div>
      )}
    </div>
  );
};

export default Profile;
