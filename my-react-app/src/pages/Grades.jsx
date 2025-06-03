import React, { useEffect, useState } from 'react';

function Grades() {
  const [grades, setGrades] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchGrades() {
      try {
        const res = await fetch('/api/grades'); // Your API endpoint
        if (!res.ok) throw new Error('Failed to fetch grades');
        const data = await res.json();
        setGrades(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchGrades();
  }, []);

  if (loading) return <div className="container mt-4">Loading grades...</div>;
  if (error) return <div className="container mt-4 text-danger">Error: {error}</div>;

  return (
    <div className="container mt-4">
      <h2>Grades</h2>
      <table className="table table-striped table-hover mt-3">
        <thead className="table-secondary">
          <tr>
            <th>Student</th>
            <th>Subject</th>
            <th>Grade</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {grades.length === 0 && (
            <tr>
              <td colSpan="4" className="text-center">No grades found.</td>
            </tr>
          )}
          {grades.map(grade => (
            <tr key={grade.gradeId}>
              <td>{grade.studentName}</td>
              <td>{grade.subject}</td>
              <td>{grade.gradeValue}</td>
              <td>{grade.date}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Grades;
