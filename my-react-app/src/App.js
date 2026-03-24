import { useState } from "react";
import Login from "./Login";

function App() {
  const [token, setToken] = useState(null);

  const testSecure = async () => {
    try {
      const response = await fetch("http://localhost:5102/api/Students/secure-test", {
        method: "GET",
        headers: {
          Authorization: "Bearer " + token, // 🔥 kjo është pika kryesore
        },
      });

      const text = await response.text();

      alert(text);
    } catch (error) {
      console.error(error);
      alert("Gabim");
    }
  };

  return (
    <div>
      {token ? (
        <div>
          <h1>Login u krye me sukses</h1>
          <button onClick={testSecure}>Test Secure Endpoint</button>
        </div>
      ) : (
        <Login setToken={setToken} />
      )}
    </div>
  );
}

export default App;