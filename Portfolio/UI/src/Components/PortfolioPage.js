import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Header from './Header';

const PortfolioPage = () => {
  const [user, setUser] = useState(JSON.parse(localStorage.getItem('user')));
  const [cryptos, setCryptos] = useState([]);
  const [newCryptoName, setNewCryptoName] = useState('');
  const [newCryptoPrice, setNewCryptoPrice] = useState(0);
  const [newCryptoSymbol, setNewCryptoSymbol] = useState('');

  useEffect(() => {
    if (user) {
      getAllCryptos(user.email);
    }
  }, [user]);

  const getAllCryptos = async (userEmail) => {
    try {
      const response = await axios.get(`http://localhost:64897/api/crypto?userEmail=${userEmail}`);
      setCryptos(response.data);
    } catch (error) {
      console.error('Error fetching cryptos:', error);
    }
  };

  const handleAddCrypto = async () => {
    try {
      await axios.post(`http://localhost:64897/api/crypto`, {
        userEmail: user.email,
        name: newCryptoName,
        currentPrice: newCryptoPrice,
        symbol: newCryptoSymbol
      });
      getAllCryptos(user.email);
    } catch (error) {
      console.error('Error adding crypto:', error);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('user');
    setUser(null);
  };

  if (!user) {
    return <p>Niste ulogovani.</p>;
  }

  return (
    <div>
      <Header />
      <h2>Dobrodošli, {user.email}</h2>
      <button onClick={handleLogout}>Logout</button>
      <p>Ovo je vaša portfolio stranica.</p>
      <h3>Vaše kriptovalute:</h3>
      <ul>
        {cryptos.map(crypto => (
          <li key={crypto.RowKey}>
            {crypto.Name} - {crypto.CurrentPrice}
          </li>
        ))}
      </ul>
      <h3>Dodaj novu kriptovalutu:</h3>
      <label>
        Symbol:
        <input
          type="text"
          value={newCryptoSymbol}
          onChange={e => setNewCryptoSymbol(e.target.value)}
        />
      </label>
      <label>
        Ime:
        <input
          type="text"
          value={newCryptoName}
          onChange={e => setNewCryptoName(e.target.value)}
        />
      </label>
      <label>
        Cena:
        <input
          type="number"
          value={newCryptoPrice}
          onChange={e => setNewCryptoPrice(parseFloat(e.target.value))}
        />
      </label>
      <button onClick={handleAddCrypto}>Dodaj</button>
    </div>
  );
};

export default PortfolioPage;
