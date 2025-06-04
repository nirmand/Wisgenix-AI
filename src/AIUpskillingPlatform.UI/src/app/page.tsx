"use client";
import React from "react";

export default function Home() {
  return (
    <main className="flex flex-col items-center justify-center min-h-[70vh] px-4 bg-gradient-to-br from-indigo-50 via-purple-50 to-pink-50">
      <section className="w-full max-w-2xl text-center mt-16">
        <h1 className="text-4xl font-bold mb-4 text-transparent bg-clip-text bg-gradient-to-tr from-indigo-600 via-purple-600 to-pink-600">AI Upskilling Platform Admin</h1>
        <p className="text-lg text-gray-700 mb-8">Manage your assessments, subjects, topics, and questions with ease.</p>
        <nav className="flex flex-col sm:flex-row gap-4 justify-center mb-8">
          <a href="/subjects" className="px-6 py-3 rounded-lg bg-gradient-to-tr from-indigo-100 via-purple-100 to-pink-100 hover:from-indigo-200 hover:to-pink-200 text-indigo-700 font-semibold transition-colors shadow">Subjects</a>
          <a href="/topics" className="px-6 py-3 rounded-lg bg-gradient-to-tr from-indigo-100 via-purple-100 to-pink-100 hover:from-indigo-200 hover:to-pink-200 text-indigo-700 font-semibold transition-colors shadow">Topics</a>
          <a href="/questions" className="px-6 py-3 rounded-lg bg-gradient-to-tr from-indigo-100 via-purple-100 to-pink-100 hover:from-indigo-200 hover:to-pink-200 text-indigo-700 font-semibold transition-colors shadow">Questions</a>
        </nav>
        <button className="mt-4 px-6 py-3 rounded-lg bg-gradient-to-tr from-indigo-600 via-purple-600 to-pink-600 text-white font-semibold shadow hover:opacity-90 transition-colors" onClick={() => {/* TODO: Add social login redirect */}}>
          Log In
        </button>
      </section>
    </main>
  );
}
