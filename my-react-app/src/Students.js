import React, { useEffect, useState, useMemo, useCallback } from 'react';
import api from './api/axios';
import { useAuth } from './context/AuthContext';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  User, 
  GraduationCap, 
  ClipboardCheck, 
  MessageSquare, 
  TrendingUp, 
  MoreHorizontal, 
  Plus, 
  Save, 
  X, 
  Check, 
  AlertCircle,
  Calendar,
  BookOpen,
  ChevronRight,
  Search,
  Filter,
  Trash2
} from 'lucide-react';
import './Students.css';

function Students() {
  const { user } = useAuth();
  const [students, setStudents] = useState([]);
  const [grades, setGrades] = useState([]);
  const [attendance, setAttendance] = useState([]);
  const [comments, setComments] = useState([]);
  const [classes, setClasses] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [markingAttendanceFor, setMarkingAttendanceFor] = useState(null);
  const [loading, setLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [searchTermClass, setSearchTermClass] = useState("");
  
  // Modal State
  const [activeModal, setActiveModal] = useState(null); // 'grade', 'attendance', 'comment', 'edit'
  const [selectedStudent, setSelectedStudent] = useState(null);
  const [formData, setFormData] = useState({});

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const fetchResource = async (endpoint, setter) => {
        try {
          const res = await api.get(endpoint);
          setter(res.data);
          return { success: true };
        } catch (err) {
          console.error(`Failed to fetch ${endpoint}:`, err);
          return { success: false, endpoint, status: err.response?.status };
        }
      };

      const results = await Promise.all([
        fetchResource('/Students', setStudents),
        fetchResource('/Grades', setGrades),
        fetchResource('/Attendances', setAttendance),
        fetchResource('/Comments', setComments),
        fetchResource('/Classes', setClasses),
        fetchResource('/Subjects', setSubjects)
      ]);

      // Only show error if Students (critical) or ALL resources failed
      const studentsFailed = results.find(r => r.endpoint === '/Students' && !r.success);
      const allFailed = results.every(r => !r.success);

      if (allFailed || studentsFailed) {
        setErrorMsg("Gabim gjatë marrjes së të dhënave kryesore. Ju lutem kontrolloni lidhjen.");
      } else {
        setErrorMsg("");
      }
    } catch (err) {
      console.error("Fetch Data Error:", err);
      setErrorMsg("Gabim teknik gjatë ngarkimit.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Helpers
  const getStats = (studentId) => {
    const studentGrades = grades.filter(g => g.studentId === studentId);
    const avg = studentGrades.length > 0 
      ? (studentGrades.reduce((acc, curr) => acc + curr.gradeValue, 0) / studentGrades.length).toFixed(1)
      : "N/A";
    
    const latest = studentGrades.length > 0
      ? studentGrades.sort((a, b) => new Date(b.dateAdded) - new Date(a.dateAdded))[0].gradeValue
      : "-";

    const studentAtt = attendance.filter(a => a.studentId === studentId);
    const attPct = studentAtt.length > 0
      ? Math.round((studentAtt.filter(a => a.status === 'Present').length / studentAtt.length) * 100)
      : 0;

    const recentCom = comments
      .filter(c => c.studentId === studentId)
      .sort((a, b) => new Date(b.date) - new Date(a.date))[0]?.commentText || "Mungon";

    return { avg, latest, allGrades: studentGrades, attPct, recentCom };
  };

  const handleAction = (type, student) => {
    setSelectedStudent(student);
    if (type === 'grade') setFormData({ studentId: student.studentId, subject: "", gradeValue: "", date: new Date().toISOString() });
    else if (type === 'comment') setFormData({ studentId: student.studentId, teacherId: user?.teacherId || 1, commentText: "", date: new Date().toISOString() });
    else if (type === 'edit') {
      if (student) {
        setFormData({
          ...student,
          dateOfBirth: student.dateOfBirth ? student.dateOfBirth.split('T')[0] : "",
          classId: student.classId || "",
          className: classes.find(c => c.classId === student.classId)?.className || "",
          parentId: student.parentId || ""
        });
      } else {
        setFormData({
          studentId: 0,
          name: "",
          surname: "",
          dateOfBirth: "",
          email: "",
          phone: "",
          address: "",
          classId: "",
          className: "",
          parentId: ""
        });
      }
    }
    if (type !== 'attendance') setActiveModal(type);
  };

  const markAttendance = async (studentId, status) => {
    try {
      const payload = {
        studentId: parseInt(studentId),
        status: status,
        date: new Date().toISOString().split('T')[0]
      };
      await api.post('/Attendances', payload);
      alert(`Pjesëmarrja (${status === 'Present' ? 'Prezent' : 'Mungon'}) u shënua me sukses!`);
      setMarkingAttendanceFor(null);
      fetchData();
    } catch (err) { 
      console.error("Attendance Error:", err);
      const msg = err.response?.data?.message || err.response?.data || (typeof err.response?.data === 'string' ? err.response.data : err.message);
      alert("Gabim gjatë regjistrimit të pjesëmarrjes: " + (typeof msg === 'object' ? JSON.stringify(msg) : msg)); 
    }
  };

  const deleteStudent = async (id) => {
    if (!window.confirm("A jeni të sigurt që dëshironi ta fshini këtë nxënës?")) return;
    try {
      await api.delete(`/Students/${id}`);
      alert("Nxënësi dhe të gjitha të dhënat e tij u fshinë me sukses.");
      fetchData();
    } catch (err) {
      console.error("Delete Error:", err);
      const msg = err.response?.data?.message || err.response?.data || err.message;
      alert("Gabim gjatë fshirjes: " + (typeof msg === 'object' ? JSON.stringify(msg) : msg));
    }
  };

  const submitModal = async (e) => {
    e.preventDefault();
    try {
      if (activeModal === 'grade') {
        await api.post('/Grades', formData);
      } else if (activeModal === 'comment') {
        await api.post('/Comments', formData);
      } else if (activeModal === 'edit') {
        const payload = {
          ...formData,
          classId: formData.classId ? parseInt(formData.classId) : null,
          parentId: (formData.parentId && formData.parentId != 0) ? parseInt(formData.parentId) : null,
          dateOfBirth: formData.dateOfBirth || new Date().toISOString().split('T')[0]
        };
        
        let savedStudent;
        if (payload.studentId && payload.studentId !== 0) {
          await api.put(`/Students/${payload.studentId}`, payload);
          savedStudent = payload;
        } else {
          const { studentId, ...createPayload } = payload;
          const res = await api.post('/Students', createPayload);
          savedStudent = res.data;
        }

        if (formData.className && formData.className.trim()) {
          await api.post('/Students/assign-class', {
            studentId: savedStudent.studentId,
            className: formData.className.trim()
          });
        }
        alert("Nxënësi u ruajt me sukses!");
      }
      setActiveModal(null);
      fetchData();
    } catch (err) { 
      console.error("Submission Error:", err);
      const msg = err.response?.data?.message || err.response?.data || err.message;
      alert(`Gabim: ${typeof msg === 'object' ? JSON.stringify(msg) : msg}`);
    }
  };

  const filteredStudents = useMemo(() => {
    return students.filter(s => {
      const matchesName = `${s.name} ${s.surname}`.toLowerCase().includes(searchTerm.toLowerCase());
      const classObj = classes.find(c => c.classId === s.classId);
      const matchesClass = !searchTermClass || 
        String(s.classId).includes(searchTermClass) || 
        (classObj && classObj.className.toLowerCase().includes(searchTermClass.toLowerCase()));
      return matchesName && matchesClass;
    });
  }, [students, searchTerm, searchTermClass, classes]);

  const statsSummary = useMemo(() => {
    // 1. Mesatarja e Klasës (Studentët e filtruar)
    const activeStudentIds = new Set(filteredStudents.map(s => s.studentId));
    const relevantGrades = grades.filter(g => activeStudentIds.has(g.studentId));
    
    const classAvg = relevantGrades.length > 0
      ? (relevantGrades.reduce((acc, curr) => acc + curr.gradeValue, 0) / relevantGrades.length).toFixed(1)
      : "0.0";

    // 2. Pjesëmarrja Sot (Studentët e filtruar)
    const todayStr = new Date().toISOString().split('T')[0];
    const todaysAtt = attendance.filter(a => 
      activeStudentIds.has(a.studentId) && 
      a.date.split('T')[0] === todayStr
    );

    const totalStudents = filteredStudents.length;
    const presentToday = todaysAtt.filter(a => a.status === 'Present').length;
    
    const attendanceTodayPct = totalStudents > 0
      ? Math.round((presentToday / totalStudents) * 100)
      : 0;

    return { classAvg, attendanceTodayPct };
  }, [filteredStudents, grades, attendance]);

  if (loading) return (
    <div className="loader-wrapper">
      <motion.div 
        animate={{ rotate: 360 }}
        transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
        className="premium-loader"
      />
      <p>Duke ngarkuar dashboard-in...</p>
    </div>
  );

  return (
    <div className="dashboard-wrapper">
      <header className="dashboard-header">
        <div className="welcome-section">
          <div className="icon-badge">
            <GraduationCap size={32} />
          </div>
          <div>
            <h1>Dashboard i Mësimdhënësit</h1>
            <p>Mirëseerdhët, {user?.name || 'Profesor'}. Menaxhoni nxënësit dhe performancën e tyre.</p>
          </div>
        </div>
        
        <div className="header-actions">
          <div className="search-bar">
            <Search size={18} />
            <input 
              type="text" 
              placeholder="Nxënësi..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
            <input 
              type="text" 
              placeholder="Nr. i Klasës (p.sh. 1, 7A)..." 
              value={searchTermClass}
              onChange={(e) => setSearchTermClass(e.target.value)}
              className="class-filter-input"
            />
          </div>
          <button className="btn-primary" onClick={() => handleAction('edit', null)}>
            <Plus size={18} />
            Regjistro Nxënës
          </button>
        </div>
      </header>

      {errorMsg && (
        <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} className="alert-banner">
          <AlertCircle size={20} />
          {errorMsg}
        </motion.div>
      )}

      <div className="stats-grid">
        <div className="stat-card">
          <TrendingUp className="stat-icon blue" />
          <div className="stat-info">
            <span>Mesatarja e Klasës</span>
            <h3>{statsSummary.classAvg}</h3>
          </div>
        </div>
        <div className="stat-card">
          <ClipboardCheck className="stat-icon green" />
          <div className="stat-info">
            <span>Pjesëmarrja Sot</span>
            <h3>{statsSummary.attendanceTodayPct}%</h3>
          </div>
        </div>
        <div className="stat-card">
          <User className="stat-icon purple" />
          <div className="stat-info">
            <span>Total Nxënës</span>
            <h3>{students.length}</h3>
          </div>
        </div>
      </div>

      <div className="table-container premium-shadow">
        <table className="teacher-table">
          <thead>
            <tr>
              <th>Nxënësi</th>
              <th>ID</th>
              <th>Statusi i Klasës</th>
              <th>Notat (Lënda: Nota)</th>
              <th>Mesatarja</th>
              <th>Pjesëmarrja</th>
              <th>Komentet</th>
            </tr>
          </thead>
          <tbody>
            <AnimatePresence>
              {filteredStudents.map((s, index) => {
                const stats = getStats(s.studentId);
                return (
                  <motion.tr 
                    key={s.studentId}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: index * 0.05 }}
                    exit={{ opacity: 0, scale: 0.95 }}
                  >
                    <td>
                      <div className="student-profile">
                        <div className="avatar-small">{s.name[0]}{s.surname[0]}</div>
                        <div>
                          <div className="student-name">{s.name} {s.surname}</div>
                          <div className="student-email">{s.email}</div>
                        </div>
                      </div>
                    </td>
                    <td>
                      <div className="id-code-wrapper">
                        <span className="id-tag">{s.studentId}</span>
                      </div>
                    </td>
                    <td>
                      <span className="badge-outline">
                        {classes.find(c => c.classId === s.classId)?.className || "Klasa A"}
                      </span>
                    </td>
                    <td>
                      <div className="grades-list">
                        {stats.allGrades.length > 0 ? (
                          stats.allGrades.map((g, idx) => (
                            <div key={idx} className="grade-item" title={new Date(g.date).toLocaleDateString()}>
                              <span className="subject-label">{g.subject}:</span>
                              <span className={`grade-value-small ${g.gradeValue >= 4 ? 'high' : 'low'}`}>
                                {g.gradeValue}
                              </span>
                            </div>
                          ))
                        ) : (
                          <span className="no-grades">-</span>
                        )}
                      </div>
                    </td>
                    <td>
                      <div className="avg-display">
                        <TrendingUp size={14} />
                        {stats.avg}
                      </div>
                    </td>
                    <td>
                      <div className="att-container">
                        <div className="progress-bar">
                          <motion.div 
                            className="progress-fill" 
                            initial={{ width: 0 }}
                            animate={{ width: `${stats.attPct}%` }}
                          />
                        </div>
                        <span className="att-label">{stats.attPct}%</span>
                      </div>
                    </td>
                    <td>
                      <div className="comment-preview" title={stats.recentCom}>
                        <MessageSquare size={14} />
                        {stats.recentCom}
                      </div>
                    </td>
                    <td className="text-right">
                      <div className="quick-actions">
                        <button className="action-circle gr" title="Shto Notë" onClick={() => handleAction('grade', s)}>
                          <BookOpen size={16} />
                        </button>
                        
                        {markingAttendanceFor === s.studentId ? (
                          <div className="attendance-selector-inline">
                            <button className="action-circle at-present" title="Prezent" onClick={() => markAttendance(s.studentId, 'Present')}>
                              <Check size={16} />
                            </button>
                            <button className="action-circle at-absent" title="Mungon" onClick={() => markAttendance(s.studentId, 'Absent')}>
                              <X size={16} />
                            </button>
                          </div>
                        ) : (
                          <button 
                            className="action-circle at" 
                            title="Shëno Pjesëmarrjen" 
                            onClick={() => setMarkingAttendanceFor(s.studentId)}
                          >
                            <ClipboardCheck size={16} />
                          </button>
                        )}

                        <button className="action-circle cm" title="Shto Koment" onClick={() => handleAction('comment', s)}>
                           <MessageSquare size={16} />
                         </button>

                        <button className="action-circle dl" title="Fshij Nxënësin" onClick={() => deleteStudent(s.studentId)}>
                          <Trash2 size={16} />
                        </button>
                      </div>
                    </td>
                  </motion.tr>
                );
              })}
            </AnimatePresence>
          </tbody>
        </table>
      </div>

      {activeModal && (
        <div className="modal-backdrop">
          <motion.div 
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className="premium-modal"
          >
            <div className="modal-header">
              <h3>
                {activeModal === 'grade' && `Shto Notë - ${selectedStudent?.name}`}
                {activeModal === 'comment' && `Koment i Ri - ${selectedStudent?.name}`}
                {activeModal === 'edit' && 'Regjistro Nxënës'}
              </h3>
              <button className="btn-close" onClick={() => setActiveModal(null)}><X /></button>
            </div>
            <form onSubmit={submitModal} className="modal-body">
              {activeModal === 'grade' && (
                <div className="form-stack">
                  <div className="field">
                    <label>Vendos Lëndën</label>
                    <input 
                      type="text" 
                      placeholder="Shkruaj lëndën (p.sh. Matematikë)..."
                      required 
                      value={formData.subject || ""} 
                      onChange={(e) => setFormData({...formData, subject: e.target.value})}
                      className="premium-input-field"
                    />
                  </div>
                  <div className="field">
                    <label>Vendos Notën (1-5)</label>
                    <input 
                      type="number" 
                      min="1" max="5" 
                      required 
                      value={formData.gradeValue} 
                      onChange={(e) => setFormData({...formData, gradeValue: parseInt(e.target.value)})}
                      className="premium-input-field"
                      placeholder="P.sh. 1, 5..."
                    />
                  </div>
                </div>
              )}

              {activeModal === 'comment' && (
                <div className="field">
                  <label>Feedback i Mësimdhënësit</label>
                  <textarea 
                    rows="4" 
                    required 
                    value={formData.commentText}
                    onChange={(e) => setFormData({...formData, commentText: e.target.value})}
                    placeholder="Shkruani vërejtjet ose lavdatat..."
                  />
                </div>
              )}

              {activeModal === 'edit' && (
                <div className="form-grid-modal">
                  <div className="field">
                    <label>Emri</label>
                    <input name="name" value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} required />
                  </div>
                  <div className="field">
                    <label>Mbiemri</label>
                    <input name="surname" value={formData.surname} onChange={(e) => setFormData({...formData, surname: e.target.value})} required />
                  </div>
                  <div className="field">
                    <label>Email</label>
                    <input type="email" name="email" value={formData.email} onChange={(e) => setFormData({...formData, email: e.target.value})} required />
                  </div>
                  <div className="field">
                    <label>Klasa</label>
                    <input 
                      type="text" 
                      placeholder="Klasa (10, 12a)"
                      value={formData.className || ""} 
                      onChange={(e) => setFormData({...formData, className: e.target.value})}
                      required
                      className="premium-input-field"
                    />
                  </div>
                  <div className="field">
                    <label>Data e Lindjes</label>
                    <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={(e) => setFormData({...formData, dateOfBirth: e.target.value})} required />
                  </div>
                </div>
              )}

              <div className="modal-footer">
                <button type="button" className="btn-secondary" onClick={() => setActiveModal(null)}>Anulo</button>
                <button type="submit" className="btn-primary">
                  <Save size={18} />
                  Ruaj të Dhënat
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}
    </div>
  );
}

export default Students;