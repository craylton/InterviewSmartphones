import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import Login from '../components/Login';

// Mock the API service
jest.mock('../services/api', () => ({
  authService: {
    login: jest.fn(),
  },
}));

const mockOnLoginSuccess = jest.fn();

describe('Login Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('renders login form', () => {
    render(<Login onLoginSuccess={mockOnLoginSuccess} />);
    
    expect(screen.getByRole('heading', { name: /interview smartphones/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
  });

  test('enables login button when credentials are provided', () => {
    render(<Login onLoginSuccess={mockOnLoginSuccess} />);
    
    const usernameInput = screen.getByLabelText(/username/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const loginButton = screen.getByRole('button', { name: /login/i });

    expect(loginButton).toBeDisabled();

    fireEvent.change(usernameInput, { target: { value: 'testuser' } });
    fireEvent.change(passwordInput, { target: { value: 'testpass' } });

    expect(loginButton).toBeEnabled();
  });

  test('fills sample credentials when sample button is clicked', () => {
    render(<Login onLoginSuccess={mockOnLoginSuccess} />);
    
    const sampleButton = screen.getByRole('button', { name: /use sample user/i });
    const usernameInput = screen.getByLabelText(/username/i) as HTMLInputElement;
    const passwordInput = screen.getByLabelText(/password/i) as HTMLInputElement;

    fireEvent.click(sampleButton);

    expect(usernameInput.value).toBe('emilys');
    expect(passwordInput.value).toBe('emilyspass');
  });

  test('shows error message on login failure', async () => {
    const { authService } = require('../services/api');
    authService.login.mockResolvedValueOnce({
      success: false,
      message: 'Invalid credentials',
      errors: ['Login failed'],
    });

    render(<Login onLoginSuccess={mockOnLoginSuccess} />);
    
    const usernameInput = screen.getByLabelText(/username/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const loginButton = screen.getByRole('button', { name: /login/i });

    fireEvent.change(usernameInput, { target: { value: 'invalid' } });
    fireEvent.change(passwordInput, { target: { value: 'invalid' } });
    fireEvent.click(loginButton);

    await waitFor(() => {
      expect(screen.getByText(/invalid credentials/i)).toBeInTheDocument();
    });
  });
});