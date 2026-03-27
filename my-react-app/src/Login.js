import { useState } from "react";
import "./App.css";

function Login({ setToken, setUserData }) {
  const [mode, setMode] = useState("login"); // 'login' or 'register'
  const [roleMode, setRoleMode] = useState("Staff"); // 'Staff' or 'Parent'
  
  // Form State
  const [email, setEmail] = useState(""); // Also used as Username for Staff
  const [password, setPassword] = useState("");
  const [name, setName] = useState("");
  const [surname, setSurname] = useState("");
  const [phone, setPhone] = useState("");
  const [staffRole, setStaffRole] = useState("Teacher"); // specific staff role

  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg("");
    setSuccessMsg("");

    try {
      const response = await fetch("http://localhost:5102/api/Auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          username: email,
          password: password,
        }),
      });

      const text = await response.text();

      if (response.ok) {
        const data = JSON.parse(text);
        localStorage.setItem("token", data.accessToken);
        if (data.refreshToken) {
           localStorage.setItem("refreshToken", data.refreshToken);
        }
        localStorage.setItem("role", data.role);
        localStorage.setItem("userType", data.userType);

        if (setUserData) setUserData({ role: data.role, userType: data.userType });
        setToken(data.accessToken);
      } else {
        setErrorMsg(text || "Kredenciale të pasakta!");
      }
    } catch (error) {
      console.error(error);
      setErrorMsg("Gabim në server! Nuk mund të lidhet me backend-in.");
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrorMsg("");
    setSuccessMsg("");

    try {
      let endpoint;
      let payload;

      if (roleMode === "Parent") {
        endpoint = "api/Parents/register";
        payload = {
            Name: name,
            Surname: surname,
            Email: email,
            Phone: phone,
            Password: password
        };
      } else if (staffRole === "Teacher") {
        endpoint = "api/Teachers/register";
        payload = {
            Name: name,
            Surname: surname,
            Email: email,
            Phone: phone,
            Username: email,
            Password: password
        };
      } else {
        endpoint = "api/Staff/register";
        payload = {
            Name: name,
            Role: staffRole,
            Username: email, 
            Password: password
        };
      }

      const response = await fetch(`http://localhost:5102/${endpoint}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      });

      const text = await response.text();

      if (response.ok) {
        setSuccessMsg("Regjistrimi u krye me sukses! Tani mund të kyçeni.");
        setMode("login");
        // Keep credentials pre-filled
      } else {
        setErrorMsg(text || "Gabim gjatë regjistrimit!");
      }
    } catch (error) {
      console.error(error);
      setErrorMsg("Gabim në server gjatë regjistrimit.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container-wrapper">
      <div className="login-container">
        
        <div className="auth-tabs">
          <div className={`auth-tab ${mode === 'login' ? 'active' : ''}`} onClick={() => setMode('login')}>
            Kyçu
          </div>
          <div className={`auth-tab ${mode === 'register' ? 'active' : ''}`} onClick={() => setMode('register')}>
            Regjistrohu
          </div>
        </div>

        {mode === 'login' ? (
          <div>
            <h2 className="login-title">Hyni në Llogari</h2>
            {successMsg && <div className="alert-danger" style={{ backgroundColor: '#d4edda', color: '#155724', borderColor: '#c3e6cb' }}>{successMsg}</div>}
            
            <form onSubmit={handleLogin}>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Email ose Përdoruesi</label>
                <input
                  type="text"
                  className="form-control"
                  style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  placeholder="Shkruani email-in ose username-in"
                />
              </div>

              <div className="form-group" style={{ marginBottom: '1.5rem' }}>
                <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Fjalëkalimi</label>
                <input
                  type="password"
                  className="form-control"
                  style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  placeholder="Shkruani fjalëkalimin"
                />
              </div>

              {errorMsg && (
                <div className="alert-danger" style={{ color: '#721c24', backgroundColor: '#f8d7da', padding: '0.75rem', borderRadius: '4px', marginBottom: '1.25rem', border: '1px solid #f5c6cb' }}>
                  {errorMsg}
                </div>
              )}

              <button 
                type="submit" 
                className="btn-primary" 
                style={{ width: '100%', padding: '0.75rem', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '1rem', fontWeight: 'bold' }}
                disabled={loading}
              >
                {loading ? "Po kyçet..." : "Kyçu"}
              </button>
            </form>
          </div>
        ) : (
          <div>
            <h2 className="login-title">Krijo Llogari</h2>
            
            <div className="role-selector">
              <label>
                <input type="radio" checked={roleMode === 'Staff'} onChange={() => setRoleMode('Staff')} />
                Përtej (Staff)
              </label>
              <label>
                <input type="radio" checked={roleMode === 'Parent'} onChange={() => setRoleMode('Parent')} />
                Prind
              </label>
            </div>

            <form onSubmit={handleRegister}>
              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Emri</label>
                <input
                  type="text"
                  className="form-control"
                  style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                />
              </div>

              {(roleMode === 'Parent' || staffRole === 'Teacher') && (
                <>
                  <div className="form-group" style={{ marginBottom: '1rem' }}>
                    <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Mbiemri</label>
                    <input
                      type="text"
                      className="form-control"
                      style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                      value={surname}
                      onChange={(e) => setSurname(e.target.value)}
                      required
                    />
                  </div>
                  <div className="form-group" style={{ marginBottom: '1rem' }}>
                    <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Telefoni</label>
                    <input
                      type="text"
                      className="form-control"
                      style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                      value={phone}
                      onChange={(e) => setPhone(e.target.value)}
                    />
                  </div>
                </>
              )}

              {roleMode === 'Staff' && (
                <div className="form-group" style={{ marginBottom: '1rem' }}>
                  <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Roli</label>
                  <select 
                    className="form-control" 
                    style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                    value={staffRole}
                    onChange={(e) => setStaffRole(e.target.value)}
                  >
                    <option value="Teacher">Mësues (Teacher)</option>
                    <option value="Admin">Administrator (Admin)</option>
                  </select>
                </div>
              )}

              <div className="form-group" style={{ marginBottom: '1rem' }}>
                <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>
                  {roleMode === 'Staff' ? 'Username / Përdoruesi' : 'Email'}
                </label>
                <input
                  type={roleMode === 'Parent' ? 'email' : 'text'}
                  className="form-control"
                  style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>

              <div className="form-group" style={{ marginBottom: '1.5rem' }}>
                <label className="form-label" style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Fjalëkalimi</label>
                <input
                  type="password"
                  className="form-control"
                  style={{ width: '100%', padding: '0.6rem', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' }}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>

              {errorMsg && (
                <div className="alert-danger" style={{ color: '#721c24', backgroundColor: '#f8d7da', padding: '0.75rem', borderRadius: '4px', marginBottom: '1.25rem', border: '1px solid #f5c6cb' }}>
                  {errorMsg}
                </div>
              )}

              <button 
                type="submit" 
                className="btn-primary" 
                style={{ width: '100%', padding: '0.75rem', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '1rem', fontWeight: 'bold', backgroundColor: '#28a745' }}
                disabled={loading}
              >
                {loading ? "Po regjistrohet..." : "Regjistrohu"}
              </button>
            </form>
          </div>
        )}
      </div>
    </div>
  );
}

export default Login;