import axios from 'axios';

const PortfolioService = {
  getAllCryptos: async (userEmail) => {
    try {
      const response = await axios.get(`http://localhost:64897/api/crypto?userEmail=${userEmail}`);
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  addCrypto: async (userEmail, newCrypto) => {
    try {

        console.log('userEmail:', userEmail);
        console.log('name:', newCrypto.Name);
        console.log('currentPrice:', newCrypto.CurrentPrice);

      await axios.post(`http://localhost:64897/api/crypto`, {
        userEmail: userEmail,
        name: newCrypto.Name,
        currentPrice: newCrypto.CurrentPrice,
        symbol: newCrypto.Symbol
      });
    } catch (error) {
      throw error;
    }
  }
};

export default PortfolioService;
