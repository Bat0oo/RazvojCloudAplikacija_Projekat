import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Header from './Header';
import '../styles/Alarm.css';

const AlarmPage = () => {
    const [user, setUser] = useState(JSON.parse(localStorage.getItem('user')));
    const [cryptos, setCryptos] = useState([]);
    const [cryptoSymbol, setCryptoSymbol] = useState('');
    const [cryptoName, setCryptoName] = useState('');
    const [cryptoPrice, setCryptoPrice] = useState(0);
    const [alarmPrice, setAlarmPrice] = useState(0);
    const [alarmCondition, setAlarmCondition] = useState('above');
    const [cryptoOptions, setCryptoOptions] = useState([]);
    const [transactionHistory, setTransactionHistory] = useState([]);
    const [alarms, setAlarms] = useState([]);
    const [currentValues, setCurrentValues] = useState({});

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
        if (user) {
            getAllCryptos(user.email);
            getTopCryptoSymbols();
            getTransactionHistory(user.email);
            getAllAlarms(user.email);
            fetchAlarmPrices();
            fetchCurrentPriceForAllCryptos();
        }
    }, [user]);

    const getAllCryptos = async (userEmail) => {
        try {
            const response = await axios.get(`http://localhost:5000/api/crypto?userEmail=${userEmail}`);
            console.log("Cryptos after refresh:", response.data);
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

    const fetchCurrentPrice = async (symbol) => {
    try {
        const response = await axios.get(`https://api.coinbase.com/v2/prices/${symbol.toUpperCase()}-USD/spot`);
        return response.data.data.amount;
    } catch (error) {
        console.error('Error fetching current price:', error);
        return null;
    }
};

    
const fetchAlarmPrices = async () => {
    try {
        const promises = alarms.map(async (alarm) => {
            const currentPricePromise = fetchCurrentPrice(alarm.CryptoSymbol.toLowerCase());
            return currentPricePromise.then(currentPrice => {
                console.log(`Alarm: ${alarm.CryptoSymbol}, Current price: ${currentPrice}`);
                
                if (currentPrice !== null) {
                    setCurrentValues(prevValues => ({
                        ...prevValues,
                        [alarm.CryptoSymbol.toUpperCase()]: currentPrice
                    }));
                }
                
                return { ...alarm, currentPrice: currentPrice };
            });
        });

        const updatedAlarms = await Promise.all(promises);
        setAlarms(updatedAlarms);
    } catch (error) {
        console.error('Error fetching alarm prices:', error);
    }
};

    const handleAddAlarm = async () => {
        if(cryptoSymbol!=''){
        try {
            const alarmId = `${cryptoSymbol}-${Date.now()}`;
            const newAlarm = {
                userEmail: user.email,
                alarmId: alarmId,
                cryptoSymbol: cryptoSymbol.toUpperCase(),
                targetPrice: alarmPrice,
                aboveorbelow: alarmCondition,
                isTriggered: false
            };
            await axios.post('http://localhost:5000/api/alarm', newAlarm);
            getAllAlarms(user.email);
        } catch (error) {
            console.error('Error adding alarm:', error);
        }
    }
    };

    const getAllAlarms = async (userEmail) => {
        try {
            const response = await axios.get(`http://localhost:5000/api/alarm?userEmail=${userEmail}`);
            setAlarms(response.data);
        } catch (error) {
            console.error('Error fetching alarms:', error);
        }
    };

    const fetchCurrentPriceForAllCryptos = async () => {
        try {
            if (cryptos.length === 0) {
                console.log("Nema dostupnih kriptovaluta.");
                return;
            }
            
            const symbols = cryptos.map(crypto => crypto.Symbol);
            const prices = await Promise.all(symbols.map(async (symbol) => {
                const currentPrice = await fetchCurrentPrice(symbol);
                return { symbol, currentPrice };
            }));
            console.log("Trenutne cene za sve kriptovalute:", prices);
        } catch (error) {
            console.error('Error fetching current prices for all cryptos:', error);
        }
    };

    if (!user) {
        return <p>Niste ulogovani.</p>;
    }

    return (
        <div className="alarm-container container">
            <Header />
            <div className="form-container mt-4">
                <form className="alarm-form card p-4">
                    <h3>Postavite alarm za kriptovalutu:</h3>
                    <div className="form-group">
                        <label>Symbol:</label>
                        <select className="form-control" value={cryptoSymbol} onChange={e => setCryptoSymbol(e.target.value.toUpperCase())}>
                            <option value="">Izaberite simbol</option>
                            {cryptoOptions.map(symbol => (
                                <option key={symbol} value={symbol.toUpperCase()}>{symbol.toUpperCase()}</option>
                            ))}
                        </select>
                    </div>
                    {/*
                    <div className="form-group">
                        <label>Ime:</label>
                        <input className="form-control" type="text" value={cryptoName} readOnly />
                    </div>
                    */}
                    <div className="form-group">
                        <label>Ciljana cena:</label>
                        <input className="form-control" type="number" value={alarmPrice} onChange={e => setAlarmPrice(parseFloat(e.target.value))} />
                    </div>
                    <div className="form-group">
                        <label>Uslov:</label>
                        <select className="form-control" value={alarmCondition} onChange={e => setAlarmCondition(e.target.value)}>
                            <option value="above">Iznad</option>
                            <option value="below">Ispod</option>
                        </select>
                    </div>
                    <button className="btn btn-primary mt-3" type="button" onClick={handleAddAlarm}>Postavi alarm</button>
                </form>
            </div>
            <div className="alarm-list mt-3">
                <h3>Aktivni alarmi:</h3>
                <table className="table">
                    <thead>
                        <tr>
                            <th>CryptoSymbol</th>
                            <th>Ciljana cena</th>
                            {/*<th>Trenutna cena</th>*/}
                            <th>Uslov</th>
                        </tr>
                    </thead>
                    <tbody>
                        {alarms.filter(alarm => alarm.IsTriggered === false)
                        .map((alarm, index) => (                            
                            <tr key={index}>
                                <td>{alarm.CryptoSymbol}</td>
                                <td>{alarm.TargetPrice} USD</td>
                                {/*<td>{currentValues[alarm.CryptoSymbol.toUpperCase()] !== undefined ? currentValues[alarm.CryptoSymbol.toUpperCase()] : 'Loading...'}</td>*/}
                                <td>{alarm.AboveOrBelow==='above' ? 'Iznad' : 'Ispod'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default AlarmPage;
