import React, { useEffect, useState } from 'react';
import api from './api/axios';
import { useAuth } from './context/AuthContext';
import './Students.css';

function Students() {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [errorMsg, setErrorMsg] = useState("");
  
  // Modal & Form State
  const [showModal, setShowModal] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [classes, setClasses] = useState([]);
  const [parents, setParents] = useState([]);
  
  const [formData, setFormData] = useState({
    studentId: 0,
    name: "",
    surname: "",
    dateOfBirth: "",
    email: "",
    phone: "",
    address: "",
    classId: "",
    parentId: ""
  });

  // const API_URL = "http://localhost:5102/api/Students";
  // const PARENTS_URL = "http://localhost:5102/api/Parents";
  const fetchStudents = async () => {
    setLoading(true);
    try {
      const response = await api.get('/Students');
      setStudents(response.data);
      setErrorMsg("");
    } catch (error) {
      console.error(error);
      setErrorMsg(error.response?.data || "Nuk ke autorizim për të parë nxënësit.");
    } finally {
      setLoading(false);
    }
  };

  const fetchClasses = async () => {
    try {
      const response = await api.get('/Classes');
      setClasses(response.data);
    } catch (err) { console.error(err); }
  };

  const fetchParents = async () => {
    try {
      const response = await api.get('/Parents');
      setParents(response.data);
    } catch (err) { console.error(err); }
  };

  useEffect(() => {
    fetchStudents();
    fetchClasses();
    fetchParents();
  }, []);

  const handleOpenModal = (student = null) => {
    if (student) {
      setEditMode(true);
      setFormData({
        ...student,
        dateOfBirth: student.dateOfBirth ? student.dateOfBirth.split('T')[0] : "",
        classId: student.classId || "",
        parentId: student.parentId || ""
      });
    } else {
      setEditMode(false);
      setFormData({
        studentId: 0,
        name: "",
        surname: "",
        dateOfBirth: "",
        email: "",
        phone: "",
        address: "",
        classId: "",
        parentId: ""
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setErrorMsg("");
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Convert IDs to numbers if present
    const payload = {
      ...formData,
      classId: formData.classId ? parseInt(formData.classId) : null,
      parentId: formData.parentId ? parseInt(formData.parentId) : null
    };
 
    try {
      if (editMode) {
        await api.put(`/Students/${formData.studentId}`, payload);
      } else {
        await api.post('/Students', payload);
      }
      handleCloseModal();
      fetchStudents();
    } catch (error) {
      console.error(error);
      alert("Gabim gjatë ruajtjes: " + (error.response?.data || "Gabim në server!"));
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("A jeni të sigurt që dëshironi të fshini këtë nxënës?")) return;
 
    try {
      await api.delete(`/Students/${id}`);
      fetchStudents();
    } catch (error) {
      console.error(error);
      alert("Gabim gjatë fshirjes: " + (error.response?.data || "Gabim në server!"));
    }
  };

  const { user } = useAuth();
  const staffClassId = students.length > 0 ? students[0].classId : null;
  const managedClassName = classes.find(c => c.classId === staffClassId)?.className;

  if (loading && students.length === 0) {
    return (
      <div className="loader-container">
        <div className="spinner"></div>
        <p>Duke ngarkuar nxënësit...</p>
      </div>
    );
  }

  return (
    <div className="students-container">
      <div className="header-dashboard">
        <div className="header-info">
          <h1>Menaxhimi i Nxënësve</h1>
          {user?.userType === 'Staff' && (
            <p className="managed-class-info">
              Sistemi po shfaq nxënësit {managedClassName ? `for ${managedClassName}` : "e klasës tuaj të caktuar"}
            </p>
          )}
        </div>
        <button className="add-btn" onClick={() => handleOpenModal()}>
          <span className="plus-icon">+</span> Regjistro Nxënës
        </button>
      </div>

      {errorMsg && <div className="alert alert-danger" style={{ color: 'red', marginBottom: '1rem' }}>{errorMsg}</div>}

      <div className="table-wrapper">
        <table className="premium-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nxënësi</th>
              <th>Klasa</th>
              <th>Email</th>
              <th>Kodi i Lidhjes</th>
              <th>Veprimet</th>
            </tr>
          </thead>
          <tbody>
            {students.length === 0 ? (
              <tr>
                <td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Nuk u gjet asnjë nxënës.</td>
              </tr>
            ) : (
              students.map((s) => (
                <tr key={s.studentId}>
                  <td style={{ color: '#888', fontSize: '0.9rem' }}>{s.studentId}</td>
                  <td style={{ fontWeight: '600' }}>{s.name} {s.surname}</td>
                  <td>
                    <span className="class-badge">
                      {classes.find(c => c.classId === s.classId)?.className || (s.classId ? `Klasa ${s.classId}` : 'Pa Klasë')}
                    </span>
                  </td>
                  <td>{s.email}</td>
                  <td>
                    <span className="link-code-box">
                      {s.linkCode || '---'}
                    </span>
                  </td>
                  <td>
                    <div className="action-btns">
                      <button className="edit-btn" title="Edito" onClick={() => handleOpenModal(s)}>
                        ✎
                      </button>
                      <button className="delete-btn" title="Fshij" onClick={() => handleDelete(s.studentId)}>
                        🗑
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {showModal && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3>{editMode ? "Edito Nxënësin" : "Regjistro Nxënës të Ri"}</h3>
              <button className="close-modal" onClick={handleCloseModal}>&times;</button>
            </div>
            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-field">
                  <label>Emri</label>
                  <input name="name" value={formData.name} onChange={handleInputChange} required />
                </div>
                <div className="form-field">
                  <label>Mbiemri</label>
                  <input name="surname" value={formData.surname} onChange={handleInputChange} required />
                </div>
                <div className="form-field">
                  <label>Email</label>
                  <input type="email" name="email" value={formData.email} onChange={handleInputChange} required />
                </div>
                <div className="form-field">
                  <label>Klasa</label>
                  <select name="classId" value={formData.classId} onChange={handleInputChange} className="premium-select">
                    <option value="">Zgjidh Klasën</option>
                    {classes.map(c => (
                      <option key={c.classId} value={c.classId}>{c.className}</option>
                    ))}
                  </select>
                </div>
                <div className="form-field">
                  <label>Prindi</label>
                  <select name="parentId" value={formData.parentId} onChange={handleInputChange} className="premium-select">
                    <option value="">Zgjidh Prindin</option>
                    {parents.map(p => (
                      <option key={p.parentId} value={p.parentId}>{p.name} {p.surname}</option>
                    ))}
                  </select>
                </div>
                <div className="form-field">
                  <label>Data e Lindjes</label>
                  <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleInputChange} required />
                </div>
                <div className="form-field full">
                  <label>Adresa</label>
                  <input name="address" value={formData.address} onChange={handleInputChange} />
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="cancel-btn" onClick={handleCloseModal}>Anulo</button>
                <button type="submit" className="save-btn">{editMode ? "Përditëso" : "Ruaj"}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Students;