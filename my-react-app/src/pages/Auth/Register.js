import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { UserPlus, User, Mail, Phone, Lock, AlertCircle, Loader2, CheckCircle } from 'lucide-react';
import api from '../../api/axios';
import { motion } from 'framer-motion';

const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: '',
    surname: '',
    email: '',
    phone: '',
    password: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const parentData = {
        name: formData.name,
        surname: formData.surname,
        email: formData.email,
        phone: formData.phone,
        password: formData.password
      };
      await api.post('/Parents/register', parentData);
      
      setSuccess(true);
      setTimeout(() => navigate('/login'), 2000);
    } catch (err) {
      const data = err.response?.data;
      const errorMessage = typeof data === 'string' ? data : data?.message || data?.error || 'Gabim gjatë regjistrimit';
      setError(errorMessage);
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  if (success) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh', padding: '1rem' }}>
        <motion.div 
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          className="glass-card" 
          style={{ width: '100%', maxWidth: '400px', padding: '2.5rem', textAlign: 'center' }}
        >
          <div style={{ color: '#10b981', marginBottom: '1rem' }}>
            <CheckCircle size={64} style={{ margin: '0 auto' }} />
          </div>
          <h2 style={{ fontSize: '1.5rem', marginBottom: '0.5rem' }}>Regjistrimi i Suksesshëm!</h2>
          <p style={{ color: 'var(--secondary)' }}>Po ju ridrejtojmë te faqja e hyrjes...</p>
        </motion.div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh', padding: '2rem 1rem' }}>
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="glass-card" 
        style={{ width: '100%', maxWidth: '500px', padding: '2.5rem' }}
      >
        <div className="text-center mb-6" style={{ textAlign: 'center' }}>
          <div style={{ background: 'var(--primary)', color: 'white', width: '3rem', height: '3rem', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1rem' }}>
            <UserPlus size={28} />
          </div>
          <h2 style={{ fontSize: '1.875rem', marginBottom: '0.5rem' }}>Krijo Llogari Prindi</h2>
          <p style={{ color: 'var(--secondary)' }}>Plotësoni të dhënat për t'u regjistruar në eDitari si Prind</p>
        </div>

        {error && (
          <motion.div 
            initial={{ opacity: 0, x: -10 }}
            animate={{ opacity: 1, x: 0 }}
            style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', background: '#fee2e2', color: '#dc2626', padding: '0.75rem', borderRadius: 'var(--radius)', marginBottom: '1.5rem', fontSize: '0.875rem' }}
          >
            <AlertCircle size={18} />
            <span>{error}</span>
          </motion.div>
        )}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label className="label">Emri</label>
            <div style={{ position: 'relative' }}>
              <User size={18} style={{ position: 'absolute', left: '0.875rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--secondary)' }} />
              <input 
                type="text" 
                name="name" 
                className="input" 
                style={{ paddingLeft: '2.75rem' }}
                placeholder="Emri" 
                onChange={handleChange} 
                required 
              />
            </div>
          </div>

          <div className="form-group">
            <label className="label">Mbiemri</label>
            <input type="text" name="surname" className="input" placeholder="Mbiemri" onChange={handleChange} required />
          </div>

          <div className="form-group">
            <label className="label">Email</label>
            <div style={{ position: 'relative' }}>
              <Mail size={18} style={{ position: 'absolute', left: '0.875rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--secondary)' }} />
              <input type="email" name="email" className="input" style={{ paddingLeft: '2.75rem' }} placeholder="shembull@email.com" onChange={handleChange} required />
            </div>
          </div>

          <div className="form-group">
            <label className="label">Telefoni</label>
            <div style={{ position: 'relative' }}>
              <Phone size={18} style={{ position: 'absolute', left: '0.875rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--secondary)' }} />
              <input type="text" name="phone" className="input" style={{ paddingLeft: '2.75rem' }} placeholder="+383 4X XXX XXX" onChange={handleChange} />
            </div>
          </div>

          <div className="form-group">
            <label className="label">Fjalëkalimi</label>
            <div style={{ position: 'relative' }}>
              <Lock size={18} style={{ position: 'absolute', left: '0.875rem', top: '50%', transform: 'translateY(-50%)', color: 'var(--secondary)' }} />
              <input
                type="password"
                name="password"
                className="input"
                style={{ paddingLeft: '2.75rem' }}
                placeholder="••••••••"
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <button 
            type="submit" 
            className="btn btn-primary" 
            style={{ width: '100%', marginTop: '1rem' }}
            disabled={loading}
          >
            {loading ? (
              <>
                <Loader2 className="animate-spin" size={20} />
                Duke u regjistruar...
              </>
            ) : (
              'Krijo Llogari'
            )}
          </button>
        </form>

        <div className="mt-4" style={{ textAlign: 'center', marginTop: '1.5rem' }}>
          <p style={{ fontSize: '0.875rem', color: 'var(--secondary)' }}>
            Keni llogari?{' '}
            <Link to="/login" style={{ color: 'var(--primary)', fontWeight: '600', textDecoration: 'none' }}>
              Kyçu këtu
            </Link>
          </p>
        </div>
      </motion.div>
    </div>
  );
};

export default Register;
