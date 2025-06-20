export function getApiBaseUrl() {
  if (process.env.NEXT_PUBLIC_API_BASE_URL) {
    return process.env.NEXT_PUBLIC_API_BASE_URL;
  }
  // Fallback for production
  if (process.env.NODE_ENV === "production") {
    return "https://your-production-api-url/";
  }
} 