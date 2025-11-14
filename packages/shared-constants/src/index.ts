/**
 * Shared Constants Package
 *
 * Application-wide constants for validation, configuration, and business rules
 */

/**
 * Pagination Constants
 */
export const PAGINATION = {
  DEFAULT_PAGE: 1,
  DEFAULT_PAGE_SIZE: 20,
  MAX_PAGE_SIZE: 100,
  MIN_PAGE_SIZE: 1,
} as const;

/**
 * Validation Constants for User
 */
export const USER_VALIDATION = {
  USERNAME_MIN_LENGTH: 3,
  USERNAME_MAX_LENGTH: 30,
  PASSWORD_MIN_LENGTH: 8,
  PASSWORD_MAX_LENGTH: 128,
  FIRST_NAME_MAX_LENGTH: 50,
  LAST_NAME_MAX_LENGTH: 50,
  EMAIL_MAX_LENGTH: 255,
} as const;

/**
 * Validation Constants for Comment
 */
export const COMMENT_VALIDATION = {
  CONTENT_MIN_LENGTH: 1,
  CONTENT_MAX_LENGTH: 5000,
  MAX_REPLY_DEPTH: 5, // Maximum nesting level for replies
} as const;

/**
 * Validation Constants for Video
 */
export const VIDEO_VALIDATION = {
  TITLE_MIN_LENGTH: 1,
  TITLE_MAX_LENGTH: 200,
  DESCRIPTION_MAX_LENGTH: 5000,
  URL_MAX_LENGTH: 2048,
  MIN_DURATION: 1, // seconds
  MAX_DURATION: 86400, // 24 hours in seconds
} as const;

/**
 * Validation Constants for Post
 */
export const POST_VALIDATION = {
  TITLE_MIN_LENGTH: 1,
  TITLE_MAX_LENGTH: 200,
  CONTENT_MIN_LENGTH: 1,
  CONTENT_MAX_LENGTH: 50000,
  EXCERPT_MAX_LENGTH: 500,
} as const;

/**
 * Validation Constants for Report
 */
export const REPORT_VALIDATION = {
  DESCRIPTION_MAX_LENGTH: 1000,
  REVIEW_NOTES_MAX_LENGTH: 2000,
} as const;

/**
 * Cache TTL (Time To Live) in seconds
 */
export const CACHE_TTL = {
  USER_PROFILE: 300, // 5 minutes
  COMMENT: 60, // 1 minute
  VIDEO: 300, // 5 minutes
  POST: 300, // 5 minutes
  REACTION_SUMMARY: 30, // 30 seconds
  HOT_COMMENTS: 120, // 2 minutes
} as const;

/**
 * Rate Limiting Constants
 */
export const RATE_LIMIT = {
  COMMENT_CREATE_PER_MINUTE: 5,
  REACTION_CREATE_PER_MINUTE: 20,
  REPORT_CREATE_PER_HOUR: 10,
  API_REQUESTS_PER_MINUTE: 100,
} as const;

/**
 * Soft Delete Grace Period (in days)
 */
export const SOFT_DELETE = {
  COMMENT_GRACE_PERIOD_DAYS: 30,
  USER_GRACE_PERIOD_DAYS: 90,
} as const;

/**
 * JWT Token Constants
 */
export const JWT = {
  ACCESS_TOKEN_EXPIRY: '15m', // 15 minutes
  REFRESH_TOKEN_EXPIRY: '7d', // 7 days
} as const;

/**
 * File Upload Constants
 */
export const UPLOAD = {
  MAX_FILE_SIZE: 10 * 1024 * 1024, // 10MB
  ALLOWED_IMAGE_TYPES: ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
  ALLOWED_VIDEO_TYPES: ['video/mp4', 'video/webm', 'video/ogg'],
} as const;

/**
 * Application Metadata
 */
export const APP_METADATA = {
  NAME: 'Commentable',
  VERSION: '1.0.0',
  DESCRIPTION: 'Comments CRUD system with polymorphic relationships',
} as const;
