import React, { useEffect, useState } from 'react';
import api from './api/axios';

function Attendance() {
  const [attendanceRecords, setAttendanceRecords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState('');

  useEffect(() => {
    async function fetchAttendance() {
      try {
        const res = await api.get('/Attendances');
        setAttendanceRecords(res.data);
      } catch (err) {
        setErrorMsg(err.response?.data || err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchAttendance();
  }, []);

  if (loading) return <div>Po ngarkohen mungesat...</div>;
  if (errorMsg) return <div style={{ color: "red" }}>Gabim: {errorMsg}</div>;

  return (
    <div>
      <h2 style={{ color: "#d39e00" }}>Regjistri i Mungesave (Attendance)</h2>
      
      {attendanceRecords.length === 0 ? (
        <p>Nnavik u gjet asnjë mungesë në database.</p>
      ) : (
        <div style={{ overflowX: "auto" }}>
          <table style={{ width: "100%", borderCollapse: "collapse", marginTop: "1rem" }}>
            <thead>
              <tr style={{ backgroundColor: "#fff3cd", textAlign: "left" }}>
                <th style={{ padding: "0.75rem", borderBottom: "2px solid #ffeeba" }}>Nxënësi ID</th>
                <th style={{ padding: "0.75rem", borderBottom: "2px solid #ffeeba" }}>Data</th>
                <th style={{ padding: "0.75rem", borderBottom: "2px solid #ffeeba" }}>Statusi</th>
              </tr>
            </thead>
            <tbody>
              {attendanceRecords.map(record => (
                <tr key={record.attendanceId} style={{ borderBottom: "1px solid #dee2e6" }}>
                  <td style={{ padding: "0.75rem" }}>{record.studentId}</td>
                  <td style={{ padding: "0.75rem" }}>{new Date(record.date).toLocaleDateString("en-GB")}</td>
                  <td style={{ padding: "0.75rem", fontWeight: "bold", color: record.status.toLowerCase() === 'mungon' ? 'red' : 'inherit' }}>
                    {record.status}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default Attendance;
