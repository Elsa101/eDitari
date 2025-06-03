import React, { useEffect, useState } from 'react';

function Dashboard() {
  const [studentsCount, setStudentsCount] = useState(0);
  const [teachersCount, setTeachersCount] = useState(0);
  const [attendancePercent, setAttendancePercent] = useState(0);
  const [commentsCount, setCommentsCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchDashboardData() {
      try {
        const [studentsRes, teachersRes, attendanceRes, commentsRes] = await Promise.all([
          fetch('/api/students/count'),
          fetch('/api/teachers/count'),
          fetch('/api/attendance/today'),
          fetch('/api/comments/today'),
        ]);

        if (!studentsRes.ok) throw new Error('Failed to fetch students count');
        if (!teachersRes.ok) throw new Error('Failed to fetch teachers count');
        if (!attendanceRes.ok) throw new Error('Failed to fetch attendance data');
        if (!commentsRes.ok) throw new Error('Failed to fetch comments count');

        const studentsData = await studentsRes.json();
        const teachersData = await teachersRes.json();
        const attendanceData = await attendanceRes.json();
        const commentsData = await commentsRes.json();

        setStudentsCount(studentsData?.count ?? 0);
        setTeachersCount(teachersData?.count ?? 0);

        const total = attendanceData?.total ?? 0;
        const present = attendanceData?.present ?? 0;
        const percent = total > 0 ? ((present / total) * 100).toFixed(1) : 0;
        setAttendancePercent(percent);

        setCommentsCount(commentsData?.count ?? 0);

      } catch (err) {
        setError(err.message || 'Error loading dashboard data');
      } finally {
        setLoading(false);
      }
    }

    fetchDashboardData();
  }, []);

  if (loading) {
    return <div className="container mt-4"><p>Loading dashboard data...</p></div>;
  }

  if (error) {
    return <div className="container mt-4"><p className="text-danger">Error: {error}</p></div>;
  }

  return (
    <div className="container mt-4">
      <h1 className="mb-4">Dashboard</h1>
      <div className="row">
        <div className="col-md-3">
          <div className="card text-white bg-primary mb-3">
            <div className="card-body">
              <h5 className="card-title">Students</h5>
              <p className="card-text display-4">{studentsCount}</p>
            </div>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card text-white bg-success mb-3">
            <div className="card-body">
              <h5 className="card-title">Teachers</h5>
              <p className="card-text display-4">{teachersCount}</p>
            </div>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card text-white bg-warning mb-3">
            <div className="card-body">
              <h5 className="card-title">Attendance Today</h5>
              <p className="card-text display-4">{attendancePercent}%</p>
            </div>
          </div>
        </div>
        <div className="col-md-3">
          <div className="card text-white bg-danger mb-3">
            <div className="card-body">
              <h5 className="card-title">New Comments</h5>
              <p className="card-text display-4">{commentsCount}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
