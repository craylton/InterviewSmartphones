export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  gender: string;
  image: string;
  token: string;
  refreshToken: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  gender: string;
  image: string;
  token: string;
  refreshToken: string;
}

export interface SmartphoneDto {
  id: number;
  brand: string;
  title: string;
  price: number;
  originalPrice?: number;
}

export interface PriceUpdateRequest {
  percentageIncrease: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  message: string;
  errors: string[];
}

export interface AppState {
  user: User | null;
  smartphones: SmartphoneDto[];
  loading: boolean;
  error: string | null;
}