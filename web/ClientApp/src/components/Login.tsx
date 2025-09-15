import React, { useState } from 'react';
import { LoginRequest } from '../types';
import { authService } from '../services/api';
import './Login.css';

interface LoginProps {
  onLoginSuccess: () => void;
}

const Login: React.FC<LoginProps> = ({ onLoginSuccess }) => {
  const [credentials, setCredentials] = useState<LoginRequest>({
    username: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const result = await authService.login(credentials);
      
      if (result.success) {
        onLoginSuccess();
      } else {
        setError(result.message || 'Login failed');
      }
    } catch (err: any) {
      setError(err.message || 'An unexpected error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setCredentials(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const useSampleCredentials = () => {
    setCredentials({
      username: 'emilys',
      password: 'emilyspass'
    });
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <h1>Interview Smartphones</h1>
        <p className="login-subtitle">Login with DummyJSON credentials</p>
        
        <form onSubmit={handleSubmit} className="login-form">
          {error && (
            <div className="error-message">
              {error}
            </div>
          )}
          
          <div className="form-group">
            <label htmlFor="username">Username:</label>
            <input
              type="text"
              id="username"
              name="username"
              value={credentials.username}
              onChange={handleInputChange}
              required
              disabled={loading}
              placeholder="Enter username"
            />
          </div>
          
          <div className="form-group">
            <label htmlFor="password">Password:</label>
            <input
              type="password"
              id="password"
              name="password"
              value={credentials.password}
              onChange={handleInputChange}
              required
              disabled={loading}
              placeholder="Enter password"
            />
          </div>
          
          <button 
            type="submit" 
            className="login-button"
            disabled={loading || !credentials.username || !credentials.password}
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
        
        <div className="sample-credentials">
          <p>Don't have credentials? Try sample user:</p>
          <button 
            type="button" 
            onClick={useSampleCredentials}
            className="sample-button"
            disabled={loading}
          >
            Use Sample User (emilys)
          </button>
          <small>Other users: atuny0, hbingley1, rshawe2, yraigatt3</small>
        </div>
      </div>
    </div>
  );
};

export default Login;