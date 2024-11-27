// authService.ts
import axios from 'axios';

export const authService = {
  signup: async (formData: { email: string; username: string; password: string }) => {
    try {
      // Send the signup data to the backend API
      const response = await axios.post('http://localhost:5182/api/user/AddUsers', {
        username: formData.username,
        email: formData.email,
        password: formData.password,
      });

      return response.data; 
    } catch (error) {

    }
  },
};
