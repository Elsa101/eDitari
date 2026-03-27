import React, { createContext, useContext, useState, useEffect } from 'react';
import api from '../api/axios';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');
    const userType = localStorage.getItem('userType');

    if (token) {
      setUser({ role, userType });
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    try {
      const response = await api.post('/Auth/login', { username, password });
      const { accessToken, refreshToken, role, userType } = response.data;

      localStorage.setItem('token', accessToken);
      if (refreshToken) localStorage.setItem('refreshToken', refreshToken);
      localStorage.setItem('role', role);
      localStorage.setItem('userType', userType);

      setUser({ role, userType });
      return { success: true };
    } catch (error) {
      const data = error.response?.data;
      const message = typeof data === 'string' ? data : data?.message || data?.error || 'Gabim gjatë identifikimit';
      return { success: false, message };
    }
  };

  const logout = async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken) {
      try {
        await api.post('/Auth/logout', { refreshToken });
      } catch (err) {
        console.error('Logout error:', err);
      }
    }
    localStorage.clear();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
