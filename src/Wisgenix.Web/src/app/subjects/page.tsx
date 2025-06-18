"use client";

import React, { useState, useEffect } from "react";
import { getApiBaseUrl } from "../../utils/apiConfig";
import { getErrorMessage } from "@/utils/getErrorMessage";
import { ApiError } from "next/dist/server/api-utils";

interface Subject {
  id: number;
  subjectName: string;
}

const PAGE_SIZE_OPTIONS = [5, 10, 20, 50];

const SubjectsPage: React.FC = () => {
  const pageKey = typeof window !== "undefined" ? window.location.pathname + "_pageSize" : "subjects_pageSize";
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(() => {
    if (typeof window !== "undefined") {
      const stored = localStorage.getItem(pageKey);
      return stored ? Number(stored) : 10;
    }
    return 10;
  });
  const [showModal, setShowModal] = useState(false);
  const [editSubject, setEditSubject] = useState<null | Subject>(null);
  const [subjectName, setSubjectName] = useState("");
  const [saving, setSaving] = useState(false);

  const totalPages = Math.ceil(subjects.length / pageSize);
  const pagedSubjects = subjects.slice((page - 1) * pageSize, page * pageSize);

  // Persist pageSize to localStorage
  useEffect(() => {
    if (typeof window !== "undefined") {
      localStorage.setItem(pageKey, String(pageSize));
    }
  }, [pageSize, pageKey]);

  // Fetch subjects from API
  useEffect(() => {
    const fetchSubjects = async () => {
      setLoading(true);
      setError(null);
      try {
        const res = await fetch(`${getApiBaseUrl()}/api/content/subjects`);
        if (!res.ok) {
          // If the API provides a specific error message in the response body,
          // you could try to parse it here, e.g.:
          const errorBody = await res.json().catch(() => ({ message: res.statusText }));
          throw new Error(errorBody.message || `Failed to fetch subjects: Status ${res.status}`);
        }
        const data = await res.json();
        setSubjects(data);
      } catch (err) {
        const messageToDisplay = getErrorMessage(err as ApiError);
        setError(messageToDisplay);
        setSubjects(Array.from({ length: 23 }, (_, i) => ({
          id: i + 1,
          subjectName: `Subject ${i + 1}`,
        })));
      } finally {
        setLoading(false);
      }
    };
    fetchSubjects();
  }, []);

  const openCreateModal = () => {
    setEditSubject(null);
    setSubjectName("");
    setShowModal(true);
  };

  const openEditModal = (subject: Subject) => {
    setEditSubject(subject);
    setSubjectName(subject.subjectName);
    setShowModal(true);
  };

  const handleSave = async () => {
    setError(null); // Clear previous errors before a new attempt
    setSaving(true);
    try {
      if (editSubject) {
        // Update
        const res = await fetch(`${getApiBaseUrl()}/api/content/update-subject/${editSubject.id}`, {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ subjectName }),
        });
        if (!res.ok) {
          // If the API provides a specific error message in the response body,
          // you could try to parse it here, e.g.:
          const errorBody = await res.json().catch(() => ({ message: res.statusText }));
          throw new Error(errorBody.message || `Failed to update subject: Status ${res.status}`);
        }
        setSubjects((prev) =>
          prev.map((s) => (s.id === editSubject.id ? { ...s, subjectName } : s))
        );
      } else {
        // Create
        const res = await fetch(`${getApiBaseUrl()}/api/content/create-subject`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ subjectName }),
        });
        if (!res.ok) {
          // If the API provides a specific error message in the response body,
          // you could try to parse it here, e.g.:
          const errorBody = await res.json().catch(() => ({ message: res.statusText }));
          throw new Error(errorBody.message || `Failed to create subject: Status ${res.status}`);
        }
        const newSubject = await res.json();
        setSubjects((prev) => [...prev, newSubject]);
      }
      setShowModal(false);
    } catch (err: unknown) {
      const messageToDisplay = getErrorMessage(err as ApiError);
      setError(messageToDisplay);

      // Always log the full error for debugging in development/production monitoring
      console.error("Error deleting subject:", err);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: number) => {
    setError(null); // Clear previous errors before a new attempt

    if (!window.confirm("Are you sure you want to delete this subject?")) return;
    try {
      const res = await fetch(`${getApiBaseUrl()}/api/content/delete-subject/${id}`, {
        method: "DELETE",
      });
      if (!res.ok) {
        // If the API provides a specific error message in the response body,
        // you could try to parse it here, e.g.:
        const errorBody = await res.json().catch(() => ({ message: res.statusText }));
        throw new Error(errorBody.message || `Failed to delete subject: Status ${res.status}`);
      }
      setSubjects((prev) => prev.filter((s) => s.id !== id));
    } catch (err: unknown) {
      const messageToDisplay = getErrorMessage(err as ApiError);
      setError(messageToDisplay);

      // Always log the full error for debugging in development/production monitoring
      console.error("Error deleting subject:", err);
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-4">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-6 gap-4">
        <h2 className="text-2xl font-bold text-blue-700">Subjects</h2>
        <button
          className="px-5 py-2 rounded-lg bg-blue-600 text-white font-semibold shadow hover:bg-blue-700 transition-colors"
          onClick={openCreateModal}
        >
          + Create Subject
        </button>
      </div>
      {error && <div className="mb-4 text-red-600">{error}</div>}
      <div className="overflow-x-auto rounded-lg shadow border border-gray-200 bg-white">
        {loading ? (
          <div className="p-8 text-center text-gray-400">Loading...</div>
        ) : (
          <table className="min-w-full text-sm">
            <thead className="bg-blue-50">
              <tr>
                <th className="px-4 py-3 text-left font-semibold">Subject</th>
                <th className="px-4 py-3 text-left font-semibold">Topics</th>
                <th className="px-4 py-3 text-left font-semibold">Actions</th>
              </tr>
            </thead>
            <tbody>
              {pagedSubjects.map((subject) => (
                <tr key={subject.id} className="border-t hover:bg-blue-50 transition-colors">
                  <td className="px-4 py-2">{subject.subjectName}</td>
                  <td className="px-4 py-2">
                    <a
                      href={`/topics?subjectId=${subject.id}`}
                      className="px-3 py-1 rounded bg-blue-100 text-blue-700 hover:bg-blue-200 transition-colors text-xs font-medium"
                    >
                      Topics
                    </a>
                  </td>
                  <td className="px-4 py-2 flex gap-2">
                    <button
                      className="px-3 py-1 rounded bg-gray-100 text-gray-700 hover:bg-gray-200 transition-colors text-xs font-medium"
                      onClick={() => openEditModal(subject)}
                    >
                      Edit
                    </button>
                    <button
                      className="px-3 py-1 rounded bg-red-100 text-red-700 hover:bg-red-200 transition-colors text-xs font-medium"
                      onClick={() => handleDelete(subject.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
              {pagedSubjects.length === 0 && !loading && (
                <tr>
                  <td colSpan={3} className="px-4 py-8 text-center text-gray-400">
                    No subjects found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>
      {/* Paging controls */}
      <div className="flex flex-col sm:flex-row items-center justify-between mt-4 gap-2">
        <div className="flex items-center gap-2">
          <span>Rows per page:</span>
          <select
            className="border rounded px-2 py-1"
            value={pageSize}
            onChange={(e) => {
              setPageSize(Number(e.target.value));
              setPage(1);
            }}
          >
            {PAGE_SIZE_OPTIONS.map((size) => (
              <option key={size} value={size}>{size}</option>
            ))}
          </select>
        </div>
        <div className="flex items-center gap-2">
          <button
            className="px-2 py-1 rounded disabled:opacity-50"
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
          >
            Prev
          </button>
          <span>
            Page {page} of {totalPages}
          </span>
          <button
            className="px-2 py-1 rounded disabled:opacity-50"
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
          >
            Next
          </button>
        </div>
      </div>
      {/* Modal for create/edit */}
      {showModal && (
        <div className="fixed inset-0 z-40 flex items-center justify-center bg-black/30">
          <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-sm">
            <h3 className="text-lg font-bold mb-4">
              {editSubject ? "Edit Subject" : "Create Subject"}
            </h3>
            <input
              className="w-full border rounded px-3 py-2 mb-4"
              placeholder="Subject Name"
              value={subjectName}
              onChange={(e) => setSubjectName(e.target.value)}
              autoFocus
            />
            <div className="flex justify-end gap-2">
              <button
                className="px-4 py-2 rounded bg-gray-100 hover:bg-gray-200 text-gray-700"
                onClick={() => setShowModal(false)}
                disabled={saving}
              >
                Cancel
              </button>
              <button
                className="px-4 py-2 rounded bg-blue-600 text-white font-semibold hover:bg-blue-700 disabled:opacity-50"
                onClick={handleSave}
                disabled={!subjectName.trim() || saving}
              >
                {saving ? "Saving..." : "Save"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default SubjectsPage; 