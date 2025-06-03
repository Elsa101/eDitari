import React, { useEffect, useState } from 'react';

function Subjects() {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchSubjects() {
      try {
        const res = await fetch('/api/subjects'); // Your API endpoint
        if (!res.ok) throw new Error('Failed to fetch subjects');
        const data = await res.json();
        setSubjects(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchSubjects();
  }, []);

  if (loading) return <div className="container mt-4">Loading subjects...</div>;
  if (error) return <div className="container mt-4 text-danger">Error: {error}</div>;

  return (
    <div className="container mt-4">
      <h2>Subjects List</h2>
      <table className="table table-striped table-hover mt-3">
        <thead className="table-info">
          <tr>
            <th>ID</th>
            <th>Subject Name</th>
            <th>Teacher</th>
          </tr>
        </thead>
        <tbody>
          {subjects.length === 0 && (
            <tr>
              <td colSpan="3" className="text-center">No subjects found.</td>
            </tr>
          )}
          {subjects.map(subject => (
            <tr key={subject.subjectId}>
              <td>{subject.subjectId}</td>
              <td>{subject.name}</td>
              <td>{subject.teacherName || 'Unassigned'}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Subjects;
