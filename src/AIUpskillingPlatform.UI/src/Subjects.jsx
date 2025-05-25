import React, { useEffect, useState } from "react";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
const PAGE_SIZE_DEFAULT = 5;

const Subjects = () => {
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [modalMode, setModalMode] = useState("add"); // 'add' or 'edit'
  const [subjectName, setSubjectName] = useState("");
  const [editId, setEditId] = useState(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(PAGE_SIZE_DEFAULT);

  useEffect(() => {
    fetchSubjects();
  }, []);

  const fetchSubjects = () => {
    setLoading(true);
    fetch(`${API_BASE_URL}/content/subjects`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch subjects");
        return res.json();
      })
      .then((data) => {
        setSubjects(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  };

  const openAddModal = () => {
    setModalMode("add");
    setSubjectName("");
    setEditId(null);
    setShowModal(true);
  };

  const openEditModal = (subject) => {
    setModalMode("edit");
    setSubjectName(subject.name);
    setEditId(subject.id);
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
    setSubjectName("");
    setEditId(null);
  };

  const handleSave = async () => {
    if (!subjectName.trim()) return;
    setLoading(true);
    try {
      if (modalMode === "add") {
        await fetch(`${API_BASE_URL}/content/subjects`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ name: subjectName }),
        });
      } else if (modalMode === "edit") {
        await fetch(`${API_BASE_URL}/content/subjects/${editId}`, {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ name: subjectName }),
        });
      }
      fetchSubjects();
      closeModal();
    } catch (err) {
      setError("Failed to save subject");
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this subject?")) return;
    setLoading(true);
    try {
      await fetch(`${API_BASE_URL}/content/subjects/${id}`, { method: "DELETE" });
      fetchSubjects();
    } catch (err) {
      setError("Failed to delete subject");
      setLoading(false);
    }
  };

  // Paging logic
  const totalPages = Math.ceil(subjects.length / pageSize);
  const pagedSubjects = subjects.slice((page - 1) * pageSize, page * pageSize);

  return (
    <section style={{ padding: "2rem" }}>
      <h2>Subjects</h2>
      <button onClick={openAddModal} style={{ marginBottom: "1rem", padding: "0.5rem 1rem" }}>
        Add Subject
      </button>
      <div style={{ marginBottom: "1rem" }}>
        <label htmlFor="page-size">Page Size: </label>
        <input
          id="page-size"
          name="page-size"
          type="number"
          min={1}
          value={pageSize}
          onChange={(e) => {
            setPageSize(Number(e.target.value));
            setPage(1);
          }}
          style={{ width: 60, marginLeft: 8 }}
        />
      </div>
      {loading && <p>Loading...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}
      <table style={{ width: "100%", borderCollapse: "collapse", marginBottom: "1rem" }}>
        <thead>
          <tr>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>ID</th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>Name</th>
            <th style={{ border: "1px solid #ccc", padding: "8px" }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {pagedSubjects.map((subject) => (
            <tr key={subject.id}>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>{subject.id}</td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>{subject.name}</td>
              <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                <button onClick={() => openEditModal(subject)} style={{ marginRight: 8 }}>
                  Edit
                </button>
                <button onClick={() => handleDelete(subject.id)} style={{ color: "red" }}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div style={{ marginBottom: "1rem" }}>
        <button onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1}>
          Prev
        </button>
        <span style={{ margin: "0 1rem" }}>
          Page {page} of {totalPages || 1}
        </span>
        <button
          onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
          disabled={page === totalPages || totalPages === 0}
        >
          Next
        </button>
      </div>
      {showModal && (
        <div
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            background: "rgba(0,0,0,0.3)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            zIndex: 1000,
          }}
        >
          <div
            style={{
              background: "#fff",
              padding: 32,
              borderRadius: 8,
              minWidth: 320,
              boxShadow: "0 2px 8px #0002",
            }}
          >
            <h3>{modalMode === "add" ? "Add Subject" : "Edit Subject"}</h3>
            <input
              id="subject-name"
              name="subject-name"
              type="text"
              value={subjectName}
              onChange={(e) => setSubjectName(e.target.value)}
              placeholder="Subject Name"
              style={{ width: "100%", padding: 8, marginBottom: 16 }}
              autoFocus
            />
            <div style={{ display: "flex", justifyContent: "flex-end" }}>
              <button onClick={closeModal} style={{ marginRight: 8 }}>
                Cancel
              </button>
              <button onClick={handleSave} style={{ background: "#007bff", color: "#fff" }}>
                Save
              </button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
};

export default Subjects;
