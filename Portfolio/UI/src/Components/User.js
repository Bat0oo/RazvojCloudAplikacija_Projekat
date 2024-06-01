import React, { useState, useEffect } from 'react';
import axios from 'axios';

const User = () => {
    const [users, setUsers] = useState([]);
    const [newUser, setNewUser] = useState({ Id: 7, Name: '', Email: '' });

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = () => {
        axios.get('http://localhost:5000/api/user')
            .then(response => {
                setUsers(response.data);
            })
            .catch(error => {
                console.error('Greška prilikom dohvaćanja korisnika:', error);
            });
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setNewUser({ ...newUser, [name]: value });
    };

    const addUser = () => {
        axios.post('http://localhost:5000/api/user', newUser)
            .then(response => {
                setNewUser({ Id: 7, Name: '', Email: '' });
                fetchUsers();
            })
            .catch(error => {
                console.error('Greška prilikom dodavanja korisnika:', error);
            });
    };

    return (
        <div>
            <h1>Lista korisnika</h1>
            <ul>
                {users.map(user => (
                    <li key={user.Id}>{user.Name} - {user.Email}</li>
                ))}
            </ul>
            <h2>Dodaj novog korisnika</h2>
            <input
                type="text"
                placeholder="Ime"
                name="Name"
                onChange={handleInputChange}
            />
            <input
                type="text"
                placeholder="Email"
                name="Email"
                onChange={handleInputChange}
            />
            <button onClick={addUser}>Dodaj korisnika</button>
        </div>
    );
};

export default User;
