export function getApiBaseUrl() {
  if (process.env.NEXT_PUBLIC_API_BASE_URL) {
    return process.env.NEXT_PUBLIC_API_BASE_URL;
  }
  // Fallback for production
  if (process.env.NODE_ENV === "production") {
    return "https://your-production-api-url/";
  }
  // For QA, set NEXT_PUBLIC_API_BASE_URL in env file or deployment config
  // Default to development
  return "https://localhost:7118";
} 