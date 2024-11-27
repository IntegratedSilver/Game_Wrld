import axios from 'axios';

interface SignupResponse {
  success: boolean;
  message?: string;
  data?: any;
}

interface SignupFormData {
  email: string;
  username: string;
  password: string;
}

export const authService = {
  signup: async (formData: SignupFormData): Promise<SignupResponse> => {
    try {

      const response = await axios.post('http://localhost:5182/api/user/AddUsers', {
        username: formData.username,
        email: formData.email,
        password: formData.password,
      });

      return response.data;
    } catch (error: any) {
      return {
        success: false,
        message: error?.response?.data?.message || 'An unexpected error occurred',
      };
    }
  },
};
