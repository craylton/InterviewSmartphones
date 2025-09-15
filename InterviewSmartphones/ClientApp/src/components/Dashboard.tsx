import React, { useState, useEffect } from 'react';
import { SmartphoneDto, LoginResponse } from '../types';
import { smartphoneService, authService } from '../services/api';
import './Dashboard.css';

interface DashboardProps {
  user: LoginResponse;
  onLogout: () => void;
}

const Dashboard: React.FC<DashboardProps> = ({ user, onLogout }) => {
  const [smartphones, setSmartphones] = useState<SmartphoneDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [percentageIncrease, setPercentageIncrease] = useState<string>('');
  const [updating, setUpdating] = useState(false);

  useEffect(() => {
    loadSmartphones();
  }, []);

  const loadSmartphones = async () => {
    setLoading(true);
    setError(null);

    try {
      const result = await smartphoneService.getTopThreeSmartphones();
      
      if (result.success && result.data) {
        setSmartphones(result.data);
      } else {
        setError(result.message || 'Failed to load smartphones');
      }
    } catch (err: any) {
      setError(err.message || 'An unexpected error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdatePrices = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const percentage = parseFloat(percentageIncrease);
    if (isNaN(percentage) || percentage < 0 || percentage > 1000) {
      setError('Please enter a valid percentage between 0 and 1000');
      return;
    }

    setUpdating(true);
    setError(null);

    try {
      const result = await smartphoneService.updatePrices({
        percentageIncrease: percentage
      });
      
      if (result.success && result.data) {
        setSmartphones(result.data);
        setPercentageIncrease('');
      } else {
        setError(result.message || 'Failed to update prices');
      }
    } catch (err: any) {
      setError(err.message || 'An unexpected error occurred');
    } finally {
      setUpdating(false);
    }
  };

  const handleLogout = () => {
    authService.logout();
    onLogout();
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <div className="header-content">
          <h1>Interview Smartphones</h1>
          <div className="user-info">
            <img src={user.image} alt={user.firstName} className="user-avatar" />
            <span>Welcome, {user.firstName} {user.lastName}</span>
            <button onClick={handleLogout} className="logout-button">
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="dashboard-main">
        <div className="container">
          <section className="smartphones-section">
            <div className="section-header">
              <h2>Top 3 Most Expensive Smartphones</h2>
              <button 
                onClick={loadSmartphones} 
                className="refresh-button"
                disabled={loading}
              >
                {loading ? 'Loading...' : 'Refresh'}
              </button>
            </div>

            {error && (
              <div className="error-message">
                {error}
              </div>
            )}

            {loading ? (
              <div className="loading-spinner">Loading smartphones...</div>
            ) : (
              <div className="smartphones-grid">
                {smartphones.map((smartphone, index) => (
                  <div key={smartphone.id} className="smartphone-card">
                    <div className="smartphone-rank">#{index + 1}</div>
                    <div className="smartphone-info">
                      <h3 className="smartphone-title">{smartphone.title}</h3>
                      <p className="smartphone-brand">{smartphone.brand}</p>
                      <div className="smartphone-price">
                        <span className="current-price">
                          {formatPrice(smartphone.price)}
                        </span>
                        {smartphone.originalPrice && smartphone.originalPrice !== smartphone.price && (
                          <span className="original-price">
                            was {formatPrice(smartphone.originalPrice)}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}

            {smartphones.length === 0 && !loading && (
              <div className="no-data">
                No smartphones found. Try refreshing or check your connection.
              </div>
            )}
          </section>

          <section className="price-update-section">
            <h2>Update Prices</h2>
            <form onSubmit={handleUpdatePrices} className="price-update-form">
              <div className="form-group">
                <label htmlFor="percentage">Percentage Increase (%):</label>
                <input
                  type="number"
                  id="percentage"
                  value={percentageIncrease}
                  onChange={(e) => setPercentageIncrease(e.target.value)}
                  min="0"
                  max="1000"
                  step="0.1"
                  placeholder="Enter percentage (e.g., 10 for 10%)"
                  disabled={updating || smartphones.length === 0}
                  required
                />
              </div>
              <button 
                type="submit" 
                className="update-button"
                disabled={updating || smartphones.length === 0 || !percentageIncrease}
              >
                {updating ? 'Updating...' : 'Update Prices'}
              </button>
            </form>
            <p className="price-update-help">
              Enter a percentage to increase the prices of all displayed smartphones.
            </p>
          </section>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;