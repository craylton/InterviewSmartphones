import axios, { AxiosResponse } from 'axios';
import { LoginRequest, LoginResponse, SmartphoneDto, PriceUpdateRequest, ApiResponse } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || '';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include auth token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Add response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.reload();
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    try {
      const response: AxiosResponse<ApiResponse<LoginResponse>> = await apiClient.post(
        '/api/auth/login',
        credentials
      );
      
      if (response.data.success && response.data.data?.token) {
        localStorage.setItem('authToken', response.data.data.token);
        localStorage.setItem('user', JSON.stringify(response.data.data));
      }
      
      return response.data;
    } catch (error: any) {
      console.error('Login error:', error);
      return {
        success: false,
        data: null,
        message: error.response?.data?.message || 'Login failed',
        errors: error.response?.data?.errors || [error.message]
      };
    }
  },

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
  },

  getCurrentUser(): LoginResponse | null {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('authToken');
  }
};

export const smartphoneService = {
  async getTopThreeSmartphones(): Promise<ApiResponse<SmartphoneDto[]>> {
    try {
      const response: AxiosResponse<ApiResponse<SmartphoneDto[]>> = await apiClient.get(
        '/api/smartphones/top-three'
      );
      return response.data;
    } catch (error: any) {
      console.error('Get smartphones error:', error);
      return {
        success: false,
        data: null,
        message: error.response?.data?.message || 'Failed to fetch smartphones',
        errors: error.response?.data?.errors || [error.message]
      };
    }
  },

  async updatePrices(request: PriceUpdateRequest): Promise<ApiResponse<SmartphoneDto[]>> {
    try {
      const response: AxiosResponse<ApiResponse<SmartphoneDto[]>> = await apiClient.put(
        '/api/smartphones/update-prices',
        request
      );
      return response.data;
    } catch (error: any) {
      console.error('Update prices error:', error);
      return {
        success: false,
        data: null,
        message: error.response?.data?.message || 'Failed to update prices',
        errors: error.response?.data?.errors || [error.message]
      };
    }
  }
};