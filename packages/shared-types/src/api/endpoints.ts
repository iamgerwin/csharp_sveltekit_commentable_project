/**
 * API Endpoint Constants
 *
 * Centralized API endpoints to prevent magic strings
 * and ensure consistency between frontend and backend
 */

export const API_ENDPOINTS = {
  // Auth
  AUTH: {
    REGISTER: '/api/v1/auth/register',
    LOGIN: '/api/v1/auth/login',
    LOGOUT: '/api/v1/auth/logout',
    REFRESH: '/api/v1/auth/refresh',
    ME: '/api/v1/auth/me',
  },

  // Users
  USERS: {
    BASE: '/api/v1/users',
    BY_ID: (id: string) => `/api/v1/users/${id}`,
    ME: '/api/v1/users/me',
  },

  // Videos
  VIDEOS: {
    BASE: '/api/v1/videos',
    BY_ID: (id: string) => `/api/v1/videos/${id}`,
    COMMENTS: (id: string) => `/api/v1/videos/${id}/comments`,
  },

  // Posts
  POSTS: {
    BASE: '/api/v1/posts',
    BY_ID: (id: string) => `/api/v1/posts/${id}`,
    COMMENTS: (id: string) => `/api/v1/posts/${id}/comments`,
  },

  // Comments
  COMMENTS: {
    BASE: '/api/v1/comments',
    BY_ID: (id: string) => `/api/v1/comments/${id}`,
    REACTIONS: (id: string) => `/api/v1/comments/${id}/reactions`,
    REPLIES: (id: string) => `/api/v1/comments/${id}/replies`,
  },

  // Reactions
  REACTIONS: {
    BASE: '/api/v1/reactions',
    BY_ID: (id: string) => `/api/v1/reactions/${id}`,
    UPSERT: '/api/v1/reactions/upsert',
  },

  // Reports
  REPORTS: {
    BASE: '/api/v1/reports',
    BY_ID: (id: string) => `/api/v1/reports/${id}`,
    REVIEW: (id: string) => `/api/v1/reports/${id}/review`,
  },
} as const;
