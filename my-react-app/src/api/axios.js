import axios from 'axios';
import { getAccessToken, getRefreshToken, setAuthTokens, clearAuthTokens } from './tokenStorage';

const api = axios.create({
  baseURL: 'http://localhost:5102/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Helper for logout without circular dependency
let logoutCallback = null;
export const setLogoutCallback = (cb) => { logoutCallback = cb; };

// Request interceptor: attach token from memory
api.interceptors.request.use(
  (config) => {
    const token = getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor: handle 401 & token refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If 401 and we haven't already retried
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const refToken = getRefreshToken();

      if (refToken) {
        try {
          // Internal call to refresh
          const response = await axios.post('http://localhost:5102/api/Auth/refresh', {
            refreshToken: refToken,
          });

          const { accessToken } = response.data;
          
          // Actually, some backends might rotating refresh tokens too
          // but for now we only get a new access token
          setAuthTokens(accessToken, refToken);
          
          // Update the original request and retry
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return api(originalRequest);
        } catch (refreshError) {
          // Refresh failed - token probably expired or blacklisted
          clearAuthTokens();
          if (logoutCallback) logoutCallback();
          return Promise.reject(refreshError);
        }
      } else {
        // No refresh token available, must login again
        if (logoutCallback) logoutCallback();
      }
    }

    return Promise.reject(error);
  }
);

export default api;
