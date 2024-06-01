import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../styles/PortfolioPage.css';
import Header from './Header';

const PortfolioPage = () => {
    const [user, setUser] = useState(JSON.parse(localStorage.getItem('user')));
    const [cryptos, setCryptos] = useState([]);
    const [cryptoSymbol, setCryptoSymbol] = useState('');
    const [cryptoName, setCryptoName] = useState('');
    const [cryptoPrice, setCryptoPrice] = useState(0);
    const [cryptoAmount, setCryptoAmount] = useState(0);
    const [cryptoOptions, setCryptoOptions] = useState([]);
    const [transactionHistory, setTransactionHistory] = useState([]);
    const [totalValue, setTotalValue] = useState(0);
    const [profitLossData, setProfitLossData] = useState({});
    const [transactionDate, setTransactionDate] = useState('');

    const symbolToName = {
        BTC: 'Bitcoin',
        ETH: 'Ethereum',
        USDT: 'Tether',
        BNB: 'Binancecoin',
        SOL: 'Solana',
        STETH: 'Staked-ether',
        USDC: 'Usd-coin',
        XRP: 'Ripple',
        DOGE: 'Dogecoin',
        TON: 'Ton-crystal',
        ADA: 'Cardano',
        AVAX: 'Avalanche-2',
        SHIB: 'Shiba-inu',
        WBTC: 'Wrapped-bitcoin',
        TRX: 'Tron',
        DOT: 'Polkadot',
        BCH: 'Bitcoin-cash',
        LINK: 'Chainlink',
        NEAR: 'Near',
        UNI: 'Uniswap'
    };

    useEffect(() => {
        if (cryptoSymbol) {
            setCryptoName(symbolToName[cryptoSymbol.toUpperCase()] || '');
        }
    }, [cryptoSymbol]);

    useEffect(() => {
        if (user) {
            getAllCryptos(user.email);
            getTopCryptoSymbols();
            getTransactionHistory(user.email);
        }
    }, [user]);

    useEffect(() => {
        transactionHistory.sort((a, b) => new Date(b.TransactionDate) - new Date(a.TransactionDate));
    }, [transactionHistory]);

    useEffect(() => {
        if (cryptos.length > 0) {
            fetchCryptoPrices();
        }
    }, [cryptos]);

    const getAllCryptos = async (userEmail) => {
        try {
            const response = await axios.get(`http://localhost:5000/api/crypto?userEmail=${userEmail}`);
            setCryptos(response.data);
        } catch (error) {
            console.error('Error fetching cryptos:', error);
        }
    };

    const getTransactionHistory = async (userEmail) => {
        try {
            const response = await axios.get(`http://localhost:5000/api/crypto/transactions?userEmail=${userEmail}`);
            setTransactionHistory(response.data);
        } catch (error) {
            console.error('Error fetching transaction history:', error);
        }
    };

    const getTopCryptoSymbols = async () => {
        try {
            const response = await axios.get('https://api.coingecko.com/api/v3/coins/markets', {
                params: {
                    vs_currency: 'usd',
                    per_page: 20,
                    page: 1,
                    order: 'market_cap_desc',
                    sparkline: false
                }
            });
            const symbols = response.data.map(crypto => crypto.symbol.toUpperCase());
            setCryptoOptions(symbols);
        } catch (error) {
            console.error('Error fetching top crypto symbols:', error);
        }
    };

    const handleDeleteCrypto = async (symbol) => {
        try {
            await axios.delete(`http://localhost:5000/api/crypto`, {
                params: {
                    userEmail: user.email,
                    symbol: symbol.toUpperCase()
                }
            });
            getAllCryptos(user.email);
            getTransactionHistory(user.email);
        } catch (error) {
            console.error('Error deleting crypto:', error);
        }
    };
    

    const handleSellCrypto = async () => {
        try {
            const existingCrypto = cryptos.find(c => c.Symbol === cryptoSymbol.toUpperCase());
    
            if (existingCrypto) {
                await axios.post(`http://localhost:5000/api/crypto/sell`, {
                    userEmail: user.email,
                    symbol: cryptoSymbol.toUpperCase(),
                    amount: cryptoAmount,
                    price: cryptoPrice,
                    transactionDate: new Date(transactionDate).toISOString() 
                });
    
                const updatedHistory = transactionHistory.map(transaction => {
                    if (transaction.CryptoSymbol === cryptoSymbol.toUpperCase() && transaction.IsPurchase) {
                        return { ...transaction, Price: cryptoPrice };
                    }
                    return transaction;
                });
                setTransactionHistory(updatedHistory);
            } else {
                console.error('Kriptovaluta koju pokušavate da prodate nije pronađena.');
            }
            getAllCryptos(user.email);
            getTransactionHistory(user.email);
        } catch (error) {
            console.error('Greška prilikom prodaje kriptovalute:', error);
        }
    };
    
    const handleAddOrBuyCrypto = async () => {
        try {
            const existingCrypto = cryptos.find(c => c.Symbol === cryptoSymbol.toUpperCase());
    
            if (existingCrypto) {
                await axios.post(`http://localhost:5000/api/crypto/buy`, {
                    userEmail: user.email,
                    symbol: cryptoSymbol.toUpperCase(),
                    amount: cryptoAmount,
                    price: cryptoPrice,
                    transactionDate: new Date(transactionDate).toISOString() 
                });
    
                const updatedHistory = transactionHistory.map(transaction => {
                    if (transaction.CryptoSymbol === cryptoSymbol.toUpperCase() && !transaction.IsPurchase) {
                        return { ...transaction, Price: cryptoPrice };
                    }
                    return transaction;
                });
                setTransactionHistory(updatedHistory);
            } else {
                await axios.post(`http://localhost:5000/api/crypto`, {
                    userEmail: user.email,
                    name: cryptoName,
                    symbol: cryptoSymbol.toUpperCase(),
                    amount: cryptoAmount,
                    currentPrice: cryptoPrice,
                    initialPrice: cryptoPrice,
                    transactionDate: new Date(transactionDate).toISOString() 
                });
            }
            getAllCryptos(user.email);
            getTransactionHistory(user.email);
        } catch (error) {
            console.error('Error adding or buying crypto:', error);
        }
    };    
    

    const fetchCryptoPrices = async () => {
        try {
            const response = await axios.get('https://api.coinbase.com/v2/exchange-rates?currency=usd');
            const rates = response.data.data.rates;

            let total = 0;
            const profitLoss = {};

            cryptos.forEach(crypto => {
                const rate = rates[crypto.Symbol.toUpperCase()];
                if (rate) {
                    const cryptoPrice = 1 / rate;
                    total += cryptoPrice * crypto.Amount;

                    if (crypto.CurrentPrice) {
                        const profitLossPercentage = ((cryptoPrice - crypto.CurrentPrice) / crypto.CurrentPrice)* 100;
                        profitLoss[crypto.Symbol.toUpperCase()] = profitLossPercentage.toFixed(2);
                        console.log('crypto.CurrentPrice: ', crypto.CurrentPrice);
                        console.log('profitLossPercentage: ', profitLossPercentage);
                    } else {
                        profitLoss[crypto.Symbol.toUpperCase()] = 'N/A';
                    }
                } else {
                    console.error(`Error fetching rate for ${crypto.Symbol.toUpperCase()}: Rate not found in response`);
                    profitLoss[crypto.Symbol.toUpperCase()] = 'N/A';
                }
            });

            setTotalValue(total);
            setProfitLossData(profitLoss);
        } catch (error) {
            console.error('Error fetching crypto prices:', error);
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
        <div className="portfolio-container container">
            <Header />
            <div className="crypto-container">
                <h3 className="mt-4">Vaše kriptovalute:</h3>
                <ul className="list-group">
                    {cryptos.map(crypto => (
                        <li className="list-group-item" key={crypto.RowKey}>
                            <span className="crypto-info">
                                {crypto.Name} ({crypto.Symbol.toUpperCase()}) - {crypto.Amount.toFixed(3)}
                                <span style={{ color: profitLossData[crypto.Symbol.toUpperCase()] < 0 ? 'red' : 'green' }}>
                                    {profitLossData[crypto.Symbol.toUpperCase()] !== 'N/A' ? (
                                        <> ({profitLossData[crypto.Symbol.toUpperCase()]}%) </>
                                    ) : (
                                        ' (N/A)'
                                    )}
                                </span>
                            </span>
                            <button className="btn btn-danger ml-2" onClick={() => handleDeleteCrypto(crypto.Symbol)}>X</button>
                        </li>
                    ))}
                </ul>

                <h4 className="mt-4">Ukupna vrednost portfolia: ${totalValue.toFixed(2)}</h4>
            </div>
            <div className="form-container mt-4">
                <form className="crypto-form card p-4">
                    <h3>Unesite informacije o kriptovaluti:</h3>
                    <div className="form-group">
                        <label>Symbol:</label>
                        <select className="form-control" value={cryptoSymbol} onChange={e => setCryptoSymbol(e.target.value.toUpperCase())}>
                            <option value="">Izaberite simbol</option>
                            {cryptoOptions.map(symbol => (
                                <option key={symbol} value={symbol.toUpperCase()}>{symbol.toUpperCase()}</option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Ime:</label>
                        <input className="form-control" type="text" value={cryptoName} readOnly />
                    </div>
                    <div className="form-group">
                        <label>Datum transakcije:</label>
                        <input className="form-control" type="date" value={transactionDate} onChange={e => setTransactionDate(e.target.value)} />
                    </div>
                    <div className="form-group">
                        <label>Cena:</label>
                        <input className="form-control" type="number" value={cryptoPrice} onChange={e => setCryptoPrice(parseFloat(e.target.value))} />
                    </div>
                    <div className="form-group">
                        <label>Količina:</label>
                        <input className="form-control" type="number" value={cryptoAmount} onChange={e => setCryptoAmount(parseFloat(e.target.value))} />
                    </div>
                    <button className="btn btn-primary mr-2 buy-btn" type="button" onClick={handleAddOrBuyCrypto}>Buy</button>
                    <button className="btn btn-secondary sell-btn" type="button" onClick={handleSellCrypto}>Sell</button>
                </form>
            </div>
            <div className="transaction-history mt-5">
                <h3>Istorija transakcija:</h3>
                <table className="table">
                    <thead>
                        <tr>
                            <th>Simbol</th>
                            <th>Datum</th>
                            <th>Količina</th>
                            <th>Cena</th>
                            <th>Tip transakcije</th>
                        </tr>
                    </thead>
                    <tbody>
                        {transactionHistory.map(transaction => (
                            <tr key={transaction.RowKey}>
                                <td>{transaction.CryptoSymbol}</td>
                                <td>{new Date(transaction.TransactionDate).toLocaleString()}</td>
                                <td>{transaction.Amount}</td>
                                <td>{transaction.Price}</td>
                                <td>{transaction.IsPurchase ? 'Kupovina' : 'Prodaja'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default PortfolioPage;
