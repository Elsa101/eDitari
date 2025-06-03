import React, { useEffect, useState } from 'react';

function Attendance() {
  const [attendanceRecords, setAttendanceRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchAttendance() {
      try {
        const res = await fetch('/api/attendance');
        if (!res.ok) throw new Error('Failed to fetch attendance');
        const data = await res.json();
        setAttendanceRecords(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchAttendance();
  }, []);

  if (loading) return <div className="container mt-4">Loading attendance...</div>;
  if (error) return <div className="container mt-4 text-danger">Error: {error}</div>;

  return (
    <div className="container mt-4">
      <h2>Attendance Records</h2>
      <table className="table table-striped table-hover mt-3">
        <thead className="table-warning">
          <tr>
            <th>Student</th>
            <th>Date</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {attendanceRecords.length === 0 && (
            <tr>
              <td colSpan="3" className="text-center">No attendance records found.</td>
            </tr>
          )}
          {attendanceRecords.map(record => (
            <tr key={record.attendanceId}>
              <td>{record.studentName}</td>
              <td>{record.date}</td>
              <td>{record.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Attendance;
