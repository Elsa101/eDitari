import { useState } from "react";

export default function App() {
  const [username, setUsername] = useState("arben.hoxha@example.com");
  const [password, setPassword] = useState("12345");
  const [message, setMessage] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setMessage("Logging in...");

    try {
      const res = await fetch("http://localhost:5102/api/Auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, password }),
      });

      const text = await res.text(); // lexojmë si text, pastaj provojmë JSON
      let data;
      try {
        data = JSON.parse(text);
      } catch {
        data = text;
      }

      if (!res.ok) {
        setMessage(`Error ${res.status}: ${typeof data === "string" ? data : JSON.stringify(data)}`);
        return;
      }

      // pritet: { accessToken: "...", role: "Teacher" }
      const token = data.accessToken;
      setMessage("✅ Login OK! Token u mor (shiko console).");
      console.log("TOKEN:", token);

      // ruaje për më vonë (hapi tjetër)
      localStorage.setItem("token", token);
      localStorage.setItem("role", data.role ?? "");
    } catch (err) {
      setMessage("❌ Network error: " + err.message);
    }
  };

  return (
    <div style={{ fontFamily: "Arial", padding: 24, maxWidth: 420 }}>
      <h2>eDitari - Login (test)</h2>

      <form onSubmit={handleLogin}>
        <div style={{ marginBottom: 12 }}>
          <label>Username (Email)</label>
          <input
            style={{ width: "100%", padding: 8, marginTop: 6 }}
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="email"
          />
        </div>

        <div style={{ marginBottom: 12 }}>
          <label>Password</label>
          <input
            type="password"
            style={{ width: "100%", padding: 8, marginTop: 6 }}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="password"
          />
        </div>

        <button type="submit" style={{ padding: "10px 14px" }}>
          Login
        </button>
      </form>

      <p style={{ marginTop: 16 }}>{message}</p>
      <small>Token ruhet te localStorage pas login-it.</small>
    </div>
  );
}
