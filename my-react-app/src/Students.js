import { useState } from "react";

function Students({ token }) {
  const [students, setStudents] = useState([]);
  const [error, setError] = useState("");

  const getStudents = async () => {
    try {
      setError("");

      const response = await fetch("http://localhost:5102/api/Students", {
        method: "GET",
        headers: {
          Authorization: "Bearer " + token,
        },
      });

      if (!response.ok) {
        const text = await response.text();
        setError("Gabim: " + text);
        return;
      }

      const data = await response.json();
      setStudents(data);
    } catch (err) {
      console.error(err);
      setError("Gabim gjate marrjes se studenteve.");
    }
  };

  return (
    <div>
      <h2>Students Page</h2>

      <button onClick={getStudents}>Get Students</button>

      <br />
      <br />

      {error && <p>{error}</p>}

      {students.length > 0 ? (
        <table border="1" cellPadding="10">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Surname</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Address</th>
            </tr>
          </thead>
          <tbody>
            {students.map((student) => (
              <tr key={student.studentId}>
                <td>{student.studentId}</td>
                <td>{student.name}</td>
                <td>{student.surname}</td>
                <td>{student.email}</td>
                <td>{student.phone}</td>
                <td>{student.address}</td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>Nuk ka studente te shfaqur ende.</p>
      )}
    </div>
  );
}

export default Students;