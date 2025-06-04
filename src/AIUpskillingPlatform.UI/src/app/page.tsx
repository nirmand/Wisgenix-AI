"use client";
import React from "react";

export default function Home() {
  return (
    <main className="flex flex-col items-center justify-center min-h-[70vh] px-4">
      <section className="w-full max-w-2xl text-center mt-16">
        <h1 className="text-4xl font-bold mb-4 text-blue-700">AI Upskilling Platform Admin</h1>
        <p className="text-lg text-gray-600 mb-8">Manage your assessments, subjects, topics, and questions with ease.</p>
        <nav className="flex flex-col sm:flex-row gap-4 justify-center mb-8">
          <a href="/subjects" className="px-6 py-3 rounded-lg bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold transition-colors">Subjects</a>
          <a href="/topics" className="px-6 py-3 rounded-lg bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold transition-colors">Topics</a>
          <a href="/questions" className="px-6 py-3 rounded-lg bg-blue-50 hover:bg-blue-100 text-blue-700 font-semibold transition-colors">Questions</a>
        </nav>
        <button className="mt-4 px-6 py-3 rounded-lg bg-blue-600 text-white font-semibold shadow hover:bg-blue-700 transition-colors" onClick={() => {/* TODO: Add social login redirect */}}>
          Log In
        </button>
      </section>
    </main>
  );
}
