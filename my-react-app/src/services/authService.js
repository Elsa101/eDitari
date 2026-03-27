import axios from 'axios';

export async function login(email, password) {
  try {
    const response = await axios.post('https://localhost:5002/api/auth/login', {
      username: email,  // or 'email' depending on your backend
      password: password
    });
    return response.data.token;  // adjust if your backend returns token differently
  } catch (error) {
    // Provide meaningful error message
    const data = error.response?.data;
    const errorMessage = typeof data === 'string' ? data : data?.message || data?.error || 'Login failed';
    throw new Error(errorMessage);
  }
}
