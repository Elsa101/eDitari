import React, { useEffect, useState, useCallback } from 'react';
import api from './api/axios';
import './ParentDashboard.css';

/* ── notification types ───────────────────────────────── */
const NOTIF_TYPES = {
  grade:      { icon: '📝', label: 'Notë e re',    color: '#4f46e5', bg: '#eef2ff' },
  attendance: { icon: '📅', label: 'Mungesë',       color: '#dc2626', bg: '#fee2e2' },
  comment:    { icon: '💬', label: 'Mesazh',        color: '#059669', bg: '#d1fae5' },
};

/* ─────────────────────────────────────────────────────────
   MAIN COMPONENT
───────────────────────────────────────────────────────── */
export default function ParentDashboard() {
  const [children,    setChildren]    = useState([]);
  const [grades,      setGrades]      = useState([]);
  const [attendance,  setAttendance]  = useState([]);
  const [comments,    setComments]    = useState([]);

  const [selectedChild, setSelectedChild] = useState(null);
  const [activeTab,     setActiveTab]     = useState('grades');

  const [notifications, setNotifications] = useState([]);
  const [showNotif,     setShowNotif]     = useState(false);
  const [seenIds,       setSeenIds]       = useState(() => {
    try { return JSON.parse(localStorage.getItem('seenNotifs') || '[]'); } catch { return []; }
  });

  const [loading, setLoading] = useState(true);
  const [msg,     setMsg]     = useState({ text: '', type: '' });

  /* ── search for linking child ── */
  const [linkStudentId, setLinkStudentId] = useState('');
  const [linkCode,      setLinkCode]      = useState('');
  const [linking,       setLinking]       = useState(false);

  /* ── fetch data ───────────────────────────────────────── */
  const fetchAll = useCallback(async () => {
    setLoading(true);
    try {
      const [cRes, gRes, aRes, cmRes] = await Promise.all([
        api.get('/Parents/my-children'),
        api.get('/Parents/my-children/grades'),
        api.get('/Parents/my-children/attendance'),
        api.get('/Parents/my-children/comments'),
      ]);
      setChildren(cRes.data);
      setGrades(gRes.data);
      setAttendance(aRes.data);
      setComments(cmRes.data);

      // Build notifications from new grades, absences, comments
      const notifs = [];

      gRes.data.forEach(g => {
        notifs.push({
          id: `g-${g.gradeId}`,
          type: 'grade',
          studentId: g.studentId,
          text: `Notë e re: ${g.subject} — ${g.gradeValue}`,
          date: g.date,
        });
      });

      aRes.data
        .filter(a => a.status?.toLowerCase() === 'absent')
        .forEach(a => {
          notifs.push({
            id: `a-${a.attendanceId}`,
            type: 'attendance',
            studentId: a.studentId,
            text: `Mungesë e shënuar më ${fmtDate(a.date)}`,
            date: a.date,
          });
        });

      cmRes.data.forEach(c => {
        notifs.push({
          id: `c-${c.commentId}`,
          type: 'comment',
          studentId: c.studentId,
          text: `Mesazh nga mësuesi: "${c.commentText?.slice(0, 60)}${c.commentText?.length > 60 ? '...' : ''}"`,
          date: c.date,
        });
      });

      // Sort newest first
      notifs.sort((a, b) => new Date(b.date) - new Date(a.date));
      setNotifications(notifs);

    } catch (e) {
      flash(setMsg, 'Gabim ngarkimi: ' + (e.response?.data || e.message), 'error');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchAll(); }, [fetchAll]);

  /* ── link child ──────────────────────────────────────── */
  const handleLink = async e => {
    e.preventDefault();
    setLinking(true);
    try {
      await api.post('/Parents/link-student', {
        studentId: parseInt(linkStudentId),
        linkCode,
      });
      flash(setMsg, '✅ Fëmija u lidh me sukses!');
      setLinkStudentId(''); setLinkCode('');
      fetchAll();
    } catch (err) {
      flash(setMsg, err.response?.data || 'Kodi ose ID është i pasaktë.', 'error');
    } finally {
      setLinking(false);
    }
  };

  /* ── mark notification seen ──────────────────────────── */
  const markSeen = id => {
    const next = [...new Set([...seenIds, id])];
    setSeenIds(next);
    localStorage.setItem('seenNotifs', JSON.stringify(next));
  };
  const markAllSeen = () => {
    const next = notifications.map(n => n.id);
    setSeenIds(next);
    localStorage.setItem('seenNotifs', JSON.stringify(next));
  };

  const unseen = notifications.filter(n => !seenIds.includes(n.id));

  /* ── child-filtered data ─────────────────────────────── */
  const cGrades     = grades.filter(g => !selectedChild || g.studentId === selectedChild.studentId);
  const cAttendance = attendance.filter(a => !selectedChild || a.studentId === selectedChild.studentId);
  const cComments   = comments.filter(c => !selectedChild || c.studentId === selectedChild.studentId);

  const childName = id => { const c = children.find(x => x.studentId === id); return c ? `${c.name} ${c.surname}` : `#${id}`; };

  /* ── grade color ─────────────────────────────────────── */
  const gradeColor = v => {
    if (v >= 9) return '#059669';
    if (v >= 7) return '#2563eb';
    if (v >= 5) return '#d97706';
    return '#dc2626';
  };

  if (loading) return (
    <div className="pd-loader"><div className="pd-spinner" /><p>Duke ngarkuar...</p></div>
  );

  return (
    <div className="pd-root">

      {/* ── NOTIFICATION BELL ── */}
      <div className="notif-bar-wrapper">
        {msg.text && <div className={`msg-banner ${msg.type}`}>{msg.text}</div>}

        <button className="notif-bell" onClick={() => { setShowNotif(v=>!v); }}>
          🔔
          {unseen.length > 0 && <span className="notif-badge">{unseen.length}</span>}
        </button>

        {showNotif && (
          <div className="notif-dropdown">
            <div className="notif-header">
              <span>Njoftimet</span>
              {unseen.length > 0 && (
                <button className="notif-mark-all" onClick={markAllSeen}>Shëno të gjitha</button>
              )}
            </div>
            <div className="notif-list">
              {notifications.length === 0 && <div className="notif-empty">Nuk ka njoftime.</div>}
              {notifications.map(n => {
                const cfg = NOTIF_TYPES[n.type];
                const isSeen = seenIds.includes(n.id);
                return (
                  <div
                    key={n.id}
                    className={`notif-item ${isSeen ? 'seen' : 'unseen'}`}
                    style={{ borderLeft: `4px solid ${cfg.color}` }}
                    onClick={() => markSeen(n.id)}
                  >
                    <div className="notif-icon" style={{ background: cfg.bg, color: cfg.color }}>{cfg.icon}</div>
                    <div className="notif-body">
                      <div className="notif-text">{n.text}</div>
                      <div className="notif-meta">{childName(n.studentId)} · {fmtDate(n.date)}</div>
                    </div>
                    {!isSeen && <div className="notif-dot" />}
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>

      {/* ── HEADER ── */}
      <div className="pd-header">
        <div className="pd-header-text">
          <h1>👨‍👩‍👧 Paneli i Prindit</h1>
          <p>Kontrolloni notat, mungesat dhe mesazhet e fëmijëve tuaj.</p>
        </div>
        <div className="pd-stat-row">
          <div className="pd-stat"><span>{children.length}</span>Fëmijë</div>
          <div className="pd-stat"><span>{grades.length}</span>Nota</div>
          <div className="pd-stat"><span>{attendance.filter(a=>a.status?.toLowerCase()==='absent').length}</span>Mungesa</div>
          <div className="pd-stat"><span>{comments.length}</span>Mesazhe</div>
        </div>
      </div>

      {/* ── LINK CHILD ── */}
      <div className="pd-card link-card">
        <h3>🔗 Lidh Fëmijën me Kodin</h3>
        <form onSubmit={handleLink} className="link-form">
          <div className="form-field">
            <label>ID e Nxënësit</label>
            <input type="number" value={linkStudentId} onChange={e=>setLinkStudentId(e.target.value)} placeholder="p.sh. 7" required />
          </div>
          <div className="form-field">
            <label>Kodi i Lidhjes</label>
            <input value={linkCode} onChange={e=>setLinkCode(e.target.value)} placeholder="p.sh. A1B2C3D4" required />
          </div>
          <button type="submit" className="btn-link" disabled={linking}>
            {linking ? '⏳' : '🔗 Lidho'}
          </button>
        </form>
      </div>

      {/* ── CHILDREN SELECTOR ── */}
      {children.length === 0 ? (
        <div className="pd-empty-children">
          <span style={{fontSize:'3rem'}}>👶</span>
          <p>Nuk keni asnjë fëmijë të lidhur ende.</p>
          <p className="muted small">Përdorni formën e mësipërme për të lidhur fëmijën tuaj.</p>
        </div>
      ) : (
        <>
          <div className="children-row">
            <button className={`child-chip ${!selectedChild?'active':''}`} onClick={()=>setSelectedChild(null)}>Të gjithë</button>
            {children.map(c=>(
              <button key={c.studentId} className={`child-chip ${selectedChild?.studentId===c.studentId?'active':''}`} onClick={()=>setSelectedChild(c)}>
                {c.name} {c.surname}
              </button>
            ))}
          </div>

          {/* ── TABS ── */}
          <div className="tab-bar">
            {[['grades','📝 Notat'],['attendance','📅 Prezenca'],['comments','💬 Mesazhe']].map(([k,l])=>(
              <button key={k} className={`tab-btn ${activeTab===k?'active':''}`} onClick={()=>setActiveTab(k)}>{l}</button>
            ))}
          </div>

          {/* GRADES */}
          {activeTab==='grades' && (
            <div className="pd-card">
              <h3>📝 Notat</h3>
              {cGrades.length===0 ? <Empty text="Nuk ka nota." /> : (
                <table className="pd-table">
                  <thead><tr><th>Nxënësi</th><th>Lënda</th><th>Nota</th><th>Data</th></tr></thead>
                  <tbody>
                    {cGrades.map(g=>(
                      <tr key={g.gradeId}>
                        <td>{childName(g.studentId)}</td>
                        <td><strong>{g.subject}</strong></td>
                        <td><span className="grade-badge" style={{background: gradeColor(g.gradeValue)}}>{g.gradeValue}</span></td>
                        <td className="muted small">{fmtDate(g.date)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          )}

          {/* ATTENDANCE */}
          {activeTab==='attendance' && (
            <div className="pd-card">
              <h3>📅 Prezenca dhe Mungesat</h3>
              {cAttendance.length===0 ? <Empty text="Nuk ka të dhëna prezence." /> : (
                <table className="pd-table">
                  <thead><tr><th>Nxënësi</th><th>Data</th><th>Statusi</th></tr></thead>
                  <tbody>
                    {cAttendance.map(a=>(
                      <tr key={a.attendanceId}>
                        <td>{childName(a.studentId)}</td>
                        <td className="muted small">{fmtDate(a.date)}</td>
                        <td>
                          <span className={`status-badge ${a.status?.toLowerCase()==='absent'?'absent':'present'}`}>
                            {a.status?.toLowerCase()==='absent'?'❌ Mungon':'✅ Prezent'}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          )}

          {/* COMMENTS */}
          {activeTab==='comments' && (
            <div className="pd-card">
              <h3>💬 Mesazhet nga Mësuesi</h3>
              {cComments.length===0 ? <Empty text="Nuk ka mesazhe." /> : (
                <div className="comment-list">
                  {cComments.map(c=>(
                    <div key={c.commentId} className="comment-card">
                      <div className="comment-header">
                        <span className="comment-child">{childName(c.studentId)}</span>
                        <span className="muted small">{fmtDate(c.date)}</span>
                      </div>
                      <p className="comment-text">{c.commentText}</p>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </>
      )}
    </div>
  );
}

/* ── utils ────────────────────────────────────── */
function fmtDate(d) {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('sq-AL', { day:'2-digit', month:'short', year:'numeric' });
}
function flash(setMsg, text, type='success') {
  setMsg({ text, type });
  setTimeout(() => setMsg({ text:'', type:'' }), 4500);
}
function Empty({ text }) { return <div style={{textAlign:'center',padding:'2rem',color:'#a0aec0',fontSize:'.875rem'}}>{text}</div>; }
