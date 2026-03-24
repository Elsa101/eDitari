import { useState } from "react";

function Login({ setToken }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleLogin = async () => {
    try {
      const response = await fetch("http://localhost:5102/api/Auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          username: email,
          password: password,
        }),
      });

      const text = await response.text();

      if (response.ok) {
        const data = JSON.parse(text);

        setToken(data.accessToken); 

        alert("Login success!");
        console.log("TOKEN:", data.accessToken);
      } else {
        alert("Login failed!");
        console.log("ERROR:", text);
      }
    } catch (error) {
      console.error(error);
      alert("Gabim ne kerkese");
    }
  };

  return (
    <div>
      <h2>Login Page</h2>

      <input
        type="text"
        placeholder="Email or Username"
        onChange={(e) => setEmail(e.target.value)}
      />

      <br /><br />

      <input
        type="password"
        placeholder="Password"
        onChange={(e) => setPassword(e.target.value)}
      />

      <br /><br />

      <button onClick={handleLogin}>Login</button>
    </div>
  );
}

export default Login;