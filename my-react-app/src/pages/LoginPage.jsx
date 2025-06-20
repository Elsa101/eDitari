import React, { useState } from 'react';
import { login } from '../services/authService';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); // Clear previous error

    try {
      const token = await login(email, password);  // Call API
      localStorage.setItem('token', token);        // Save token
      alert('Login successful! Token saved.');     // Show success alert instead of redirect
    } catch (err) {
      setError(err.message || 'Login failed');     // Show error message
    }
  };

  return (
    <div 
      className="d-flex align-items-center justify-content-center vh-100" 
      style={{ backgroundColor: '#007bff' }} // Bootstrap primary blue
    >
      <div 
        className="p-4 rounded shadow" 
        style={{ 
          backgroundColor: 'white', 
          width: '350px',
          boxShadow: '0 0 15px rgba(0,123,255,0.4)'
        }}
      >
        <h3 className="text-center mb-4" style={{ color: '#007bff' }}>
          Login
        </h3>
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label className="form-label" style={{ color: '#0056b3' }}>Email address</label>
            <input
              type="email"
              className="form-control border-primary"
              value={email}
              onChange={e => setEmail(e.target.value)}
              required
              style={{ boxShadow: 'inset 0 1px 1px rgba(0,0,0,0.075)' }}
            />
          </div>
          <div className="mb-3">
            <label className="form-label" style={{ color: '#0056b3' }}>Password</label>
            <input
              type="password"
              className="form-control border-primary"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
              style={{ boxShadow: 'inset 0 1px 1px rgba(0,0,0,0.075)' }}
            />
          </div>
          {error && (
            <div className="alert alert-danger">
              {error}
            </div>
          )}
          <button 
            type="submit" 
            className="btn btn-primary w-100"
            style={{ backgroundColor: '#0056b3', borderColor: '#004085' }}
          >
            Login
          </button>
        </form>
      </div>
    </div>
  );
}

export default LoginPage;
