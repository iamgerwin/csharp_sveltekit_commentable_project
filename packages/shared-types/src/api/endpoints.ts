/**
 * API Endpoint Constants
 *
 * Centralized API endpoints to prevent magic strings
 * and ensure consistency between frontend and backend
 */

export const API_ENDPOINTS = {
  // Auth
  AUTH: {
    REGISTER: '/api/auth/register',
    LOGIN: '/api/auth/login',
    LOGOUT: '/api/auth/logout',
    REFRESH: '/api/auth/refresh',
    ME: '/api/auth/me',
  },

  // Users
  USERS: {
    BASE: '/api/users',
    BY_ID: (id: string) => `/api/users/${id}`,
    ME: '/api/users/me',
  },

  // Videos
  VIDEOS: {
    BASE: '/api/videos',
    BY_ID: (id: string) => `/api/videos/${id}`,
    COMMENTS: (id: string) => `/api/videos/${id}/comments`,
  },

  // Posts
  POSTS: {
    BASE: '/api/posts',
    BY_ID: (id: string) => `/api/posts/${id}`,
    COMMENTS: (id: string) => `/api/posts/${id}/comments`,
  },

  // Comments
  COMMENTS: {
    BASE: '/api/comments',
    BY_ID: (id: string) => `/api/comments/${id}`,
    REACTIONS: (id: string) => `/api/comments/${id}/reactions`,
    REPLIES: (id: string) => `/api/comments/${id}/replies`,
  },

  // Reactions
  REACTIONS: {
    BASE: '/api/reactions',
    BY_ID: (id: string) => `/api/reactions/${id}`,
    UPSERT: '/api/reactions/upsert',
  },

  // Reports
  REPORTS: {
    BASE: '/api/reports',
    BY_ID: (id: string) => `/api/reports/${id}`,
    REVIEW: (id: string) => `/api/reports/${id}/review`,
  },
} as const;
