"use client";

import React from "react";
import { FiSearch } from "react-icons/fi";

const navLinks = [
  { href: "/subjects", label: "Subjects" },
  { href: "/topics", label: "Topics" },
  { href: "/questions", label: "Questions" },
];

const Header: React.FC = () => {
  return (
    <header className="sticky top-0 z-30 w-full bg-gradient-to-r from-indigo-50 via-purple-50 to-pink-50 backdrop-blur border-b border-gray-200 shadow-sm">
      <div className="max-w-7xl mx-auto flex items-center justify-between px-4 py-3">
        <div className="flex items-center gap-4">
          <a href="/" className="flex items-center gap-2 group focus:outline-none">
            <div className="w-10 h-10 flex items-center justify-center rounded-full bg-gradient-to-tr from-indigo-400 via-purple-400 to-pink-400 text-white font-bold text-xl shadow-md select-none group-hover:scale-105 transition-transform">
              <span>A</span>
            </div>
            <span className="text-xl font-bold text-transparent bg-clip-text bg-gradient-to-tr from-indigo-600 via-purple-600 to-pink-600 tracking-tight select-none ml-2 group-hover:underline">AIUpskill Admin</span>
          </a>
          <nav className="hidden md:flex gap-2 ml-6">
            {navLinks.map((link) => (
              <a
                key={link.href}
                href={link.href}
                className="px-4 py-2 rounded-lg text-gray-700 hover:bg-indigo-100 hover:text-indigo-700 transition-colors font-medium"
              >
                {link.label}
              </a>
            ))}
          </nav>
        </div>
        <div className="flex items-center gap-2">
          <button className="p-2 rounded-full hover:bg-indigo-100 transition-colors" aria-label="Search">
            <FiSearch size={20} color="#6366f1" />
          </button>
          <button className="ml-2 px-4 py-2 rounded-lg bg-gradient-to-tr from-indigo-600 via-purple-600 to-pink-600 text-white font-semibold shadow hover:opacity-90 transition-colors text-sm" onClick={() => {/* TODO: Add social login redirect */}}>
            Log In
          </button>
        </div>
      </div>
      {/* Mobile nav */}
      <nav className="flex md:hidden justify-center gap-2 pb-2">
        {navLinks.map((link) => (
          <a
            key={link.href}
            href={link.href}
            className="px-3 py-1 rounded text-gray-700 hover:bg-indigo-100 hover:text-indigo-700 transition-colors text-sm"
          >
            {link.label}
          </a>
        ))}
      </nav>
    </header>
  );
};

export default Header; 