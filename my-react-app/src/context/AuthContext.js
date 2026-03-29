import React, { createContext, useContext, useState, useEffect } from 'react';
import api, { setLogoutCallback } from '../api/axios';
import { setAuthTokens, clearAuthTokens, getAccessToken } from '../api/tokenStorage';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Define logout first to pass to api interceptor
  const logout = async () => {
    // Optional: notify backend to revoke the token
    // const refreshToken = getRefreshToken();
    // if (refreshToken) {
    //   try { await api.post('/Auth/revoke', { refreshToken }); } catch (err) {}
    // }
    
    clearAuthTokens();
    localStorage.removeItem('role');
    localStorage.removeItem('userType');
    // Note: tokens were never in localStorage according to new requirement
    
    setUser(null);
    window.location.href = '/login';
  };

  useEffect(() => {
    // Register the logout callback for axios interceptor
    setLogoutCallback(logout);

    // Bootstrap local state
    // Note: Access token cannot be recovered from memory after refresh
    // So if user refreshes the page, they are logged out.
    // This is the literal security requirement: Neve stored in localStorage.
    const hasRole = localStorage.getItem('role');
    if (hasRole) {
      // Warning: User is logged in but has no tokens until they login again
      // We could ideally use http-only cookies for persistence, but here we follow memory-only rules.
      // So if getAccessToken is null, user must login.
      if (!getAccessToken()) {
        localStorage.removeItem('role');
        localStorage.removeItem('userType');
        setUser(null);
      } else {
        setUser({ 
           role: localStorage.getItem('role'), 
           userType: localStorage.getItem('userType') 
        });
      }
    }
    setLoading(false);
  }, []);

  const login = async (username, password) => {
    try {
      const response = await api.post('/Auth/login', { username, password });
      const { accessToken, refreshToken, role, userType } = response.data;

      // Store in memory instead of localStorage
      setAuthTokens(accessToken, refreshToken);
      
      // We can store non-sensitive data in localStorage for UI
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

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
