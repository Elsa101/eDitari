import { useState } from "react";
import Login from "./Login";
import Students from "./Students";

function App() {
  const [token, setToken] = useState(null);

  const testSecure = async () => {
    try {
      const response = await fetch("http://localhost:5102/api/Students/secure-test", {
        method: "GET",
        headers: {
          Authorization: "Bearer " + token,
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
      <h1>Dashboard</h1>
      <button onClick={testSecure}>Test Secure Endpoint</button>
    <Students />
        </div>
      ) : (
        <Login setToken={setToken} />
      )}
    </div>
  );
}

export default App;