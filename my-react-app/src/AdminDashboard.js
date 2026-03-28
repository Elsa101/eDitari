import React, { useEffect, useState, useCallback, useRef } from 'react';
import api from './api/axios';
import './AdminDashboard.css';

/* ── helpers ─────────────────────────────── */
const flash = (setMsg, text, type = 'success') => {
  setMsg({ text, type });
  setTimeout(() => setMsg({ text: '', type: '' }), 4500);
};

const emptyForm = {
  name: '', surname: '', dateOfBirth: '', email: '',
  phone: '', address: '', className: '', parentId: '', staffId: '',
};

/* ── Searchable Dropdown ─────────────────── */
function SearchDrop({ label, items, valueKey, displayFn, value, onChange, placeholder }) {
  const [q, setQ] = useState('');
  const [open, setOpen] = useState(false);
  const ref = useRef();

  useEffect(() => {
    const handler = e => { if (!ref.current?.contains(e.target)) setOpen(false); };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, []);

  const filtered = items.filter(i => displayFn(i).toLowerCase().includes(q.toLowerCase()));
  const selected = items.find(i => String(i[valueKey]) === String(value));

  return (
    <div className="form-field" ref={ref}>
      <label>{label}</label>
      <div className="sdrop-wrap">
        <input
          className="sdrop-input"
          placeholder={selected ? displayFn(selected) : placeholder}
          value={q}
          onFocus={() => setOpen(true)}
          onChange={e => { setQ(e.target.value); setOpen(true); }}
          autoComplete="off"
        />
        {value && (
          <button type="button" className="sdrop-clear" onClick={() => { onChange(''); setQ(''); }}>✕</button>
        )}
        {open && filtered.length > 0 && (
          <div className="sdrop-list">
            {filtered.map(i => (
              <div
                key={i[valueKey]}
                className={`sdrop-item ${String(i[valueKey]) === String(value) ? 'active' : ''}`}
                onMouseDown={() => { onChange(String(i[valueKey])); setQ(''); setOpen(false); }}
              >
                {displayFn(i)}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

/* ════════════════════════════════════════════
   ADMIN DASHBOARD
════════════════════════════════════════════ */
export default function AdminDashboard() {
  const [tab, setTab] = useState('students');

  const [studentsRich, setStudentsRich] = useState([]); // with parents
  const [staff,   setStaff]   = useState([]);
  const [parents, setParents] = useState([]);

  const [selectedTeacher, setSelectedTeacher] = useState(null);
  const [teacherStudents, setTeacherStudents] = useState([]);

  const [showAddStudent, setShowAddStudent] = useState(false);
  const [studentForm,    setStudentForm]    = useState(emptyForm);

  const [showAddTeacher, setShowAddTeacher] = useState(false);
  const [teacherForm,    setTeacherForm]    = useState({name: '', username: '', password: '', role: 'Teacher'});

  const [detailStudent, setDetailStudent] = useState(null); // modal for student details

  const [assignTarget,  setAssignTarget]  = useState(null);
  const [assignClass,   setAssignClass]   = useState('');
  const [assignStaffId, setAssignStaffId] = useState('');

  const [loading, setLoading] = useState(true);
  const [msg,     setMsg]     = useState({ text: '', type: '' });

  /* ── fetch ──────────────────────────── */
  const fetchAll = useCallback(async () => {
    setLoading(true);
    try {
      const [sRes, stRes, pRes] = await Promise.all([
        api.get('/Students/with-parents'),
        api.get('/Staff'),
        api.get('/Parents'),
      ]);
      setStudentsRich(sRes.data);
      setStaff(stRes.data);
      setParents(pRes.data);
    } catch (e) {
      flash(setMsg, 'Gabim ngarkimi: ' + (e.response?.data || e.message), 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchAll(); }, [fetchAll]);

  /* ── teacher click ──────────────────── */
  const handleTeacher = t => {
    if (selectedTeacher?.staffId === t.staffId) { setSelectedTeacher(null); setTeacherStudents([]); return; }
    setSelectedTeacher(t);
    setTeacherStudents(t.classId ? studentsRich.filter(s => s.classId === t.classId) : []);
  };

  /* ── deletes ────────────────────────── */
  const delStudent = async id => {
    if (!window.confirm('Fshi nxënësin?')) return;
    try { await api.delete(`/Students/${id}`); setStudentsRich(p => p.filter(s => s.studentId !== id)); setTeacherStudents(p => p.filter(s => s.studentId !== id)); flash(setMsg, 'Nxënësi u fshi.'); }
    catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };
  const delStaff = async id => {
    if (!window.confirm('Fshi mësuesin?')) return;
    try { await api.delete(`/Staff/${id}`); setStaff(p => p.filter(s => s.staffId !== id)); if (selectedTeacher?.staffId === id) { setSelectedTeacher(null); setTeacherStudents([]); } flash(setMsg, 'Mësuesi u fshi.'); }
    catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };
  const delParent = async id => {
    if (!window.confirm('Fshi prindin?')) return;
    try { await api.delete(`/Parents/${id}`); setParents(p => p.filter(x => x.parentId !== id)); flash(setMsg, 'Prindi u fshi.'); }
    catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };

  /* ── add student ────────────────────── */
  const handleStudentChange = e => setStudentForm(p => ({ ...p, [e.target.name]: e.target.value }));

  const handleAddStudent = async e => {
    e.preventDefault();
    try {
      const createRes = await api.post('/Students', {
        name: studentForm.name, surname: studentForm.surname,
        dateOfBirth: studentForm.dateOfBirth || '2000-01-01',
        email: studentForm.email, phone: studentForm.phone,
        address: studentForm.address,
        classId: null,
        parentId: studentForm.parentId ? parseInt(studentForm.parentId) : null,
      });
      const ns = createRes.data;
      if (studentForm.className.trim()) {
        await api.post('/Students/assign-class', {
          studentId: ns.studentId,
          className: studentForm.className.trim(),
          staffId: studentForm.staffId ? parseInt(studentForm.staffId) : null,
        });
      }
      flash(setMsg, `✅ ${ns.name} ${ns.surname} u shtua! ID:${ns.studentId} | Kodi:${ns.linkCode}`);
      setShowAddStudent(false);
      setStudentForm(emptyForm);
      fetchAll();
    } catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };

  /* ── add teacher ────────────────────── */
  const handleTeacherChange = e => setTeacherForm(p => ({ ...p, [e.target.name]: e.target.value }));

  const handleAddTeacher = async e => {
    e.preventDefault();
    try {
      await api.post('/Staff/register', teacherForm);
      flash(setMsg, `✅ Mësuesi ${teacherForm.name} u shtua me sukses!`);
      setShowAddTeacher(false);
      setTeacherForm({name: '', username: '', password: '', role: 'Teacher'});
      fetchAll();
    } catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };

  /* ── assign class ───────────────────── */
  const handleAssign = async e => {
    e.preventDefault();
    try {
      await api.post('/Students/assign-class', {
        studentId: assignTarget.studentId,
        className: assignClass,
        staffId: assignStaffId ? parseInt(assignStaffId) : null,
      });
      flash(setMsg, 'Klasa u caktua!');
      setAssignTarget(null); setAssignClass(''); setAssignStaffId('');
      fetchAll();
    } catch (e) { flash(setMsg, e.response?.data || e.message, 'error'); }
  };

  /* ── render ─────────────────────────── */
  if (loading) return <Loader />;

  const staffName = id => { const s = staff.find(x => x.classId === id); return s ? s.name : '—'; };

  return (
    <div className="admin-dashboard">

      {/* HEADER */}
      <div className="admin-header">
        <div>
          <h1>🏫 Paneli i Adminit</h1>
          <p>Menaxhoni nxënësit, mësuesit dhe prindërit e sistemit eDitari.</p>
        </div>
        <div className="admin-stats">
          <span className="stat-chip">🎓 {studentsRich.length}</span>
          <span className="stat-chip">🧑‍🏫 {staff.length}</span>
          <span className="stat-chip">👨‍👩‍👧 {parents.length}</span>
        </div>
      </div>

      {msg.text && <div className={`msg-banner ${msg.type}`}>{msg.text}</div>}

      {/* TABS */}
      <div className="tab-bar">
        {[['students','🎓 Nxënësit'],['teachers','🧑‍🏫 Mësuesit'],['parents','👨‍👩‍👧 Prindërit']].map(([k,l])=>(
          <button key={k} className={`tab-btn ${tab===k?'active':''}`} onClick={()=>setTab(k)}>{l}</button>
        ))}
      </div>

      {/* ══ STUDENTS ══ */}
      {tab==='students' && (
        <div className="tab-panel">
          <div className="panel-actions">
            <h2>Nxënësit ({studentsRich.length})</h2>
            <button className="btn-primary" onClick={()=>setShowAddStudent(v=>!v)}>
              {showAddStudent?'✕ Mbyll':'+ Regjistro Nxënës'}
            </button>
          </div>

          {/* REGISTRATION FORM */}
          {showAddStudent && (
            <div className="add-form-box">
              <h3>📋 Regjistrim Nxënësi</h3>
              <p className="form-note">🔑 ID dhe Kodi i Lidhjes gjenerohen <strong>automatikisht</strong> nga sistemi.</p>
              <form onSubmit={handleAddStudent} className="student-form">
                <div className="form-row">
                  <F label="Emri *"    name="name"    value={studentForm.name}    onChange={handleStudentChange} required />
                  <F label="Mbiemri *" name="surname" value={studentForm.surname} onChange={handleStudentChange} required />
                </div>
                <div className="form-row">
                  <F label="Datëlindja" name="dateOfBirth" type="date" value={studentForm.dateOfBirth} onChange={handleStudentChange} />
                  <F label="Email"      name="email"        type="email" value={studentForm.email}       onChange={handleStudentChange} />
                </div>
                <div className="form-row">
                  <F label="Telefon" name="phone"   value={studentForm.phone}   onChange={handleStudentChange} />
                  <F label="Adresa"  name="address" value={studentForm.address} onChange={handleStudentChange} />
                </div>
                <div className="form-row">
                  <div className="form-field">
                    <label>Klasa (shkruaj vetë)</label>
                    <input name="className" value={studentForm.className} onChange={handleStudentChange} placeholder="p.sh. Klasa 6A" />
                  </div>
                </div>
                <div className="form-row">
                  <SearchDrop
                    label="Prindi (opsionale)"
                    items={parents}
                    valueKey="parentId"
                    displayFn={p=>`${p.name} ${p.surname}`}
                    value={studentForm.parentId}
                    onChange={v=>setStudentForm(x=>({...x,parentId:v}))}
                    placeholder="Kërko prind..."
                  />
                  <SearchDrop
                    label="Mësuesi (opsionale)"
                    items={staff}
                    valueKey="staffId"
                    displayFn={s=>`${s.name} (${s.role})`}
                    value={studentForm.staffId}
                    onChange={v=>setStudentForm(x=>({...x,staffId:v}))}
                    placeholder="Kërko mësues..."
                  />
                </div>
                <button type="submit" className="btn-primary" style={{alignSelf:'flex-start'}}>✅ Regjistro</button>
              </form>
            </div>
          )}

          {/* ASSIGN MODAL */}
          {assignTarget && (
            <div className="modal-overlay" onClick={()=>setAssignTarget(null)}>
              <div className="modal-box" onClick={e=>e.stopPropagation()}>
                <h3>🔗 Cakto Klasë / Mësues</h3>
                <p>Nxënësi: <strong>{assignTarget.name} {assignTarget.surname}</strong></p>
                <form onSubmit={handleAssign}>
                  <div className="form-field">
                    <label>Emri i Klasës</label>
                    <input value={assignClass} onChange={e=>setAssignClass(e.target.value)} placeholder="p.sh. Klasa 7B" required />
                  </div>
                  <SearchDrop
                    label="Mësuesi (opsionale)"
                    items={staff}
                    valueKey="staffId"
                    displayFn={s=>`${s.name} (${s.role})`}
                    value={assignStaffId}
                    onChange={setAssignStaffId}
                    placeholder="Kërko mësues..."
                  />
                  <div style={{display:'flex',gap:'.8rem',marginTop:'1.2rem'}}>
                    <button type="submit" className="btn-primary">Ruaj</button>
                    <button type="button" className="btn-outline" onClick={()=>setAssignTarget(null)}>Anulo</button>
                  </div>
                </form>
              </div>
            </div>
          )}

          {/* STUDENT DETAIL MODAL */}
          {detailStudent && (
            <div className="modal-overlay" onClick={()=>setDetailStudent(null)}>
              <div className="modal-box detail-modal" onClick={e=>e.stopPropagation()}>
                <div className="detail-header">
                  <div className="detail-avatar">{detailStudent.name[0]}{detailStudent.surname[0]}</div>
                  <div>
                    <h3>{detailStudent.name} {detailStudent.surname}</h3>
                    <span className="badge-class">ID #{detailStudent.studentId}</span>
                  </div>
                </div>
                <div className="detail-grid">
                  <DetailLine label="Email"    value={detailStudent.email||'—'} />
                  <DetailLine label="Telefon"  value={detailStudent.phone||'—'} />
                  <DetailLine label="Adresa"   value={detailStudent.address||'—'} />
                  <DetailLine label="Klasa #"  value={detailStudent.classId||'—'} />
                  <DetailLine label="Mësuesi"  value={staffName(detailStudent.classId)} />
                  <DetailLine label="Kodi Lidhjes" value={<code className="link-code">{detailStudent.linkCode||'—'}</code>} />
                </div>
                {/* PARENTS */}
                <div style={{marginTop:'1rem'}}>
                  <strong style={{fontSize:'.85rem',color:'#4a5568'}}>👨‍👩‍👧 Prindërit e lidhur:</strong>
                  {(!detailStudent.parents || detailStudent.parents.length===0) ? (
                    <p className="muted small">Nuk ka prindër të lidhur.</p>
                  ) : (
                    <div className="parent-chips">
                      {detailStudent.parents.map(p=>(
                        <div key={p.parentId} className="parent-chip">
                          <span className="parent-chip-name">👤 {p.name} {p.surname}</span>
                          <span className="parent-chip-email">{p.email}</span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
                <button className="btn-outline" style={{marginTop:'1.2rem'}} onClick={()=>setDetailStudent(null)}>Mbyll</button>
              </div>
            </div>
          )}

          {/* STUDENTS TABLE */}
          {studentsRich.length===0
            ? <Empty text="Nuk ka nxënës të regjistruar." />
            : <div className="table-wrap">
                <table className="admin-table">
                  <thead><tr>
                    <Th>ID</Th><Th>Emri</Th><Th>Mbiemri</Th>
                    <Th>Klasa</Th><Th>Mësuesi</Th><Th>Prindërit</Th><Th>Kodi</Th><Th>Veprime</Th>
                  </tr></thead>
                  <tbody>
                    {studentsRich.map(s=>(
                      <tr key={s.studentId}>
                        <td className="id-cell">{s.studentId}</td>
                        <td><strong className="clickable" onClick={()=>setDetailStudent(s)}>{s.name}</strong></td>
                        <td>{s.surname}</td>
                        <td>{s.classId?<span className="badge-class">#{s.classId}</span>:'—'}</td>
                        <td className="muted small">{staffName(s.classId)}</td>
                        <td>
                          {s.parents?.length>0 ? s.parents.map(p=>(
                            <span key={p.parentId} className="parent-tag">{p.name} {p.surname}</span>
                          )) : <span className="muted">—</span>}
                        </td>
                        <td><code className="link-code">{s.linkCode||'—'}</code></td>
                        <td>
                          <div style={{display:'flex',gap:'.4rem'}}>
                            <button className="btn-sm btn-assign" onClick={()=>{setAssignTarget(s);setAssignClass('');setAssignStaffId('');}}>🔗</button>
                            <button className="btn-sm btn-delete" onClick={()=>delStudent(s.studentId)}>🗑</button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
          }
        </div>
      )}

      {/* ══ TEACHERS ══ */}
      {tab==='teachers' && (
        <div className="tab-panel">
          <div className="panel-actions">
            <h2>Mësuesit ({staff.length})</h2>
            <button className="btn-primary" onClick={()=>setShowAddTeacher(v=>!v)}>
              {showAddTeacher?'✕ Mbyll':'+ Regjistro Mësues'}
            </button>
          </div>
          <p className="hint">Kliko mbi mësuesin për të parë nxënësit e tij.</p>

          {/* TEACHER REGISTRATION FORM */}
          {showAddTeacher && (
            <div className="add-form-box">
              <h3>🧑‍🏫 Regjistrim Mësuesi</h3>
              <form onSubmit={handleAddTeacher} className="student-form">
                <div className="form-row">
                  <F label="Emri i Plotë *" name="name" value={teacherForm.name} onChange={handleTeacherChange} required />
                  <F label="Username/Email *" name="username" value={teacherForm.username} onChange={handleTeacherChange} required />
                </div>
                <div className="form-row">
                  <F label="Fjalëkalimi *" name="password" type="password" value={teacherForm.password} onChange={handleTeacherChange} required />
                  <div className="form-field">
                    <label>Roli</label>
                    <select name="role" value={teacherForm.role} onChange={handleTeacherChange} className="premium-input-field" style={{padding: '0.6rem 1rem', border: '1px solid #e2e8f0', borderRadius:'10px'}}>
                      <option value="Teacher">Mësues (Teacher)</option>
                    </select>
                  </div>
                </div>
                <button type="submit" className="btn-primary" style={{alignSelf:'flex-start', marginTop: '1rem'}}>✅ Regjistro</button>
              </form>
            </div>
          )}

          {staff.length===0 ? <Empty text="Nuk ka mësues." /> : (
            <div className="teachers-layout">
              <div className="teacher-list-col">
                {staff.map(t=>(
                  <div key={t.staffId} className={`teacher-card ${selectedTeacher?.staffId===t.staffId?'selected':''}`} onClick={()=>handleTeacher(t)}>
                    <div className="teacher-avatar">{t.name?.[0]?.toUpperCase()}</div>
                    <div className="teacher-info-text">
                      <span className="teacher-name">{t.name}</span>
                      <span className="teacher-role">{t.role}</span>
                      {t.classId&&<span className="badge-class mini">Klasa #{t.classId}</span>}
                    </div>
                    <button className="btn-sm btn-delete" onClick={e=>{e.stopPropagation();delStaff(t.staffId);}}>🗑</button>
                  </div>
                ))}
              </div>
              {selectedTeacher && (
                <div className="teacher-panel">
                  <h3>Nxënësit e <strong>{selectedTeacher.name}</strong> ({teacherStudents.length})</h3>
                  {teacherStudents.length===0 ? <Empty text="Asnjë nxënës." /> : (
                    <table className="admin-table">
                      <thead><tr><Th>ID</Th><Th>Emri</Th><Th>Mbiemri</Th><Th>Prindërit</Th><Th>Email</Th></tr></thead>
                      <tbody>
                        {teacherStudents.map(s=>(
                          <tr key={s.studentId}>
                            <td className="id-cell">{s.studentId}</td>
                            <td><strong>{s.name}</strong></td>
                            <td>{s.surname}</td>
                            <td>{s.parents?.map(p=><span key={p.parentId} className="parent-tag">{p.name} {p.surname}</span>)||'—'}</td>
                            <td className="muted small">{s.email||'—'}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* ══ PARENTS ══ */}
      {tab==='parents' && (
        <div className="tab-panel">
          <h2>Prindërit ({parents.length})</h2>
          {parents.length===0 ? <Empty text="Nuk ka prindër." /> : (
            <div className="table-wrap">
              <table className="admin-table">
                <thead><tr><Th>ID</Th><Th>Emri</Th><Th>Mbiemri</Th><Th>Email</Th><Th>Telefoni</Th><Th>Veprime</Th></tr></thead>
                <tbody>
                  {parents.map(p=>(
                    <tr key={p.parentId}>
                      <td className="id-cell">{p.parentId}</td>
                      <td><strong>{p.name}</strong></td>
                      <td>{p.surname}</td>
                      <td className="muted small">{p.email}</td>
                      <td className="muted small">{p.phone||'—'}</td>
                      <td><button className="btn-sm btn-delete" onClick={()=>delParent(p.parentId)}>🗑 Fshi</button></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

/* ── atoms ──────────────────────────────── */
function F({ label, name, value, onChange, type='text', required=false }) {
  return <div className="form-field"><label>{label}</label><input type={type} name={name} value={value} onChange={onChange} required={required} /></div>;
}
function Th({ children }) { return <th>{children}</th>; }
function Empty({ text }) { return <div className="empty-list">{text}</div>; }
function Loader() {
  return <div className="admin-loader"><div className="admin-spinner" /><p>Duke ngarkuar...</p></div>;
}
function DetailLine({ label, value }) {
  return <div className="detail-line"><span className="dl-label">{label}</span><span className="dl-value">{value}</span></div>;
}
