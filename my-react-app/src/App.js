import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import Students from './Students';
import ParentDashboard from './ParentDashboard';
import AdminDashboard from './AdminDashboard';
import { Loader2, LogOut, User as UserIcon, LayoutDashboard } from 'lucide-react';

const ProtectedRoute = ({ children, allowedRoles }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh' }}>
        <Loader2 className="animate-spin" size={48} color="var(--primary)" />
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" />;
  }

  // Kontrollojmë rolin në vlerën specifike 'Admin', 'Teacher', 'Parent'
  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return <Navigate to="/" />;
  }

  return children;
};

const Layout = ({ children }) => {
  const { user, logout } = useAuth();

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <header className="glass-card" style={{ margin: '1rem', padding: '1rem 2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderRadius: 'var(--radius)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div style={{ background: 'var(--primary)', color: 'white', padding: '0.5rem', borderRadius: '0.5rem' }}>
            <LayoutDashboard size={24} />
          </div>
          <h1 style={{ fontSize: '1.25rem', margin: 0 }}>eDitari</h1>
        </div>
        
        <div style={{ display: 'flex', alignItems: 'center', gap: '1.5rem' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: 'var(--secondary)', fontSize: '0.875rem' }}>
            <UserIcon size={16} />
            <span style={{ fontWeight: 600, color: 'var(--foreground)' }}>{user?.role}</span>
          </div>
          <button onClick={logout} className="btn btn-outline" style={{ padding: '0.4rem 0.8rem', fontSize: '0.875rem' }}>
            <LogOut size={16} />
            Dalje
          </button>
        </div>
      </header>

      <main style={{ flex: 1, padding: '1rem 2rem' }}>
        {children}
      </main>
    </div>
  );
};

const DashboardRedirect = () => {
  const { user } = useAuth();
  if (user?.role === 'Admin') return <Navigate to="/admin" />;
  if (user?.role === 'Teacher') return <Navigate to="/teacher" />;
  if (user?.role === 'Parent') return <Navigate to="/parent" />;
  return <Navigate to="/login" />;
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          
          <Route path="/admin" element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <Layout>
                <AdminDashboard />
              </Layout>
            </ProtectedRoute>
          } />

          <Route path="/teacher" element={
            <ProtectedRoute allowedRoles={['Teacher', 'Admin']}>
              <Layout>
                <Students />
              </Layout>
            </ProtectedRoute>
          } />

          <Route path="/parent" element={
            <ProtectedRoute allowedRoles={['Parent']}>
              <Layout>
                <ParentDashboard />
              </Layout>
            </ProtectedRoute>
          } />

          <Route path="/" element={<DashboardRedirect />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;