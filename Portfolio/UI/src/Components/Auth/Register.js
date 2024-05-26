import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import '../../styles/AuthForm.css';

const Register = () => {
    const [user, setUser] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        address: '',
        city: '',
        country: '',
        phoneNumber: '',
        profilePicture: null
    });

    const navigate = useNavigate();

    const handleChange = (e) => {
        setUser({ ...user, [e.target.name]: e.target.value });
    };

    const handleFileChange = (e) => {
        console.log(e.target.files[0]); // Log za proveru izabrane slike
        setUser({ ...user, profilePicture: e.target.files[0] });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const formData = new FormData();
        for (const key in user) {
            if (user[key]) {
                formData.append(key, user[key]);
            }
        }

        try {
            const response = await axios.post('http://localhost:5000/api/register', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            console.log(response.data);
            navigate('/login');
        } catch (error) {
            console.error('Registration error:', error.response ? error.response.data : error.message);
        }
    };

    return (
        <div className="Auth-form-container">
            <form className="Auth-form" onSubmit={handleSubmit}>
                <div className="Auth-form-content">
                    <h3 className="Auth-form-title">Register</h3>
                    {['firstName', 'lastName', 'email', 'password', 'address', 'city', 'country', 'phoneNumber'].map(field => (
                        <div className="form-group" key={field}>
                            <label htmlFor={field}>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                            <input
                                type={field === 'password' ? 'password' : 'text'}
                                className="form-control"
                                id={field}
                                name={field}
                                value={user[field]}
                                onChange={handleChange}
                                required
                            />
                        </div>
                    ))}
                    <div className="form-group">
                        <label htmlFor="profilePicture">Profile Picture</label>
                        <input
                            type="file"
                            className="form-control"
                            id="profilePicture"
                            name="profilePicture"
                            onChange={handleFileChange}
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">Register</button>
                    <p className="mt-3">Already have an account? <Link to="/login">Sign In</Link></p>
                </div>
            </form>
        </div>
    );
};

export default Register;
