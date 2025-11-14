// Entity Types
export enum EntityStatus {
	Active = 0,
	Hidden = 1,
	Flagged = 2,
	UnderReview = 3,
	Removed = 4
}

export enum CommentableType {
	Video = 0,
	Post = 1,
	Comment = 2
}

export enum ReactionType {
	Like = 0,
	Love = 1,
	Laugh = 2,
	Wow = 3,
	Sad = 4,
	Angry = 5
}

export enum ReportCategory {
	Spam = 0,
	Harassment = 1,
	HateSpeech = 2,
	Violence = 3,
	Misinformation = 4,
	InappropriateContent = 5,
	Other = 6
}

export enum ReportStatus {
	Pending = 0,
	UnderReview = 1,
	Resolved = 2,
	Dismissed = 3
}

// Base Entity Interface
export interface BaseEntity {
	id: string;
	createdAt: string;
	updatedAt: string;
	status: EntityStatus;
}

// Video Entity
export interface Video extends BaseEntity {
	title: string;
	description: string;
	videoUrl: string;
	thumbnailUrl?: string;
	duration?: number;
	userId: string;
	username: string;
	viewCount: number;
	commentCount: number;
	reactionCounts: ReactionCounts;
	userReaction?: ReactionType | null;
}

export interface CreateVideoRequest {
	title: string;
	description: string;
	videoUrl: string;
	thumbnailUrl?: string;
	duration?: number;
}

export interface UpdateVideoRequest {
	title?: string;
	description?: string;
	videoUrl?: string;
	thumbnailUrl?: string;
	duration?: number;
}

// Post Entity
export interface Post extends BaseEntity {
	title: string;
	content: string;
	userId: string;
	username: string;
	commentCount: number;
	reactionCounts: ReactionCounts;
	userReaction?: ReactionType | null;
}

export interface CreatePostRequest {
	title: string;
	content: string;
}

export interface UpdatePostRequest {
	title?: string;
	content?: string;
}

// Comment Entity
export interface Comment extends BaseEntity {
	content: string;
	userId: string;
	username: string;
	commentableId: string;
	commentableType: CommentableType;
	parentCommentId?: string | null;
	replyCount: number;
	reactionCounts: ReactionCounts;
	userReaction?: ReactionType | null;
	replies?: Comment[];
}

export interface CreateCommentRequest {
	content: string;
	commentableId: string;
	commentableType: CommentableType;
	parentCommentId?: string | null;
}

export interface UpdateCommentRequest {
	content: string;
}

// Reaction Entity
export interface Reaction extends BaseEntity {
	userId: string;
	username: string;
	commentableId: string;
	commentableType: CommentableType;
	reactionType: ReactionType;
}

export interface ReactionCounts {
	[ReactionType.Like]: number;
	[ReactionType.Love]: number;
	[ReactionType.Laugh]: number;
	[ReactionType.Wow]: number;
	[ReactionType.Sad]: number;
	[ReactionType.Angry]: number;
	total: number;
}

export interface UpsertReactionRequest {
	commentableId: string;
	commentableType: CommentableType;
	reactionType: ReactionType;
}

// Report Entity
export interface Report extends BaseEntity {
	reporterId: string;
	reporterUsername: string;
	reportedEntityId: string;
	reportedEntityType: CommentableType;
	category: ReportCategory;
	description: string;
	reportStatus: ReportStatus;
	reviewerId?: string | null;
	reviewerUsername?: string | null;
	reviewNotes?: string | null;
	reviewedAt?: string | null;
}

export interface CreateReportRequest {
	reportedEntityId: string;
	reportedEntityType: CommentableType;
	category: ReportCategory;
	description: string;
}

export interface ReviewReportRequest {
	reportStatus: ReportStatus;
	reviewNotes: string;
}

// Pagination & Filtering
export interface PaginationParams {
	page?: number;
	pageSize?: number;
	sortBy?: string;
	sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResponse<T> {
	data: T[];
	page: number;
	pageSize: number;
	totalCount: number;
	totalPages: number;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
}

export interface VideoFilters extends PaginationParams {
	userId?: string;
	status?: EntityStatus;
	search?: string;
}

export interface PostFilters extends PaginationParams {
	userId?: string;
	status?: EntityStatus;
	search?: string;
}

export interface CommentFilters extends PaginationParams {
	commentableId?: string;
	commentableType?: CommentableType;
	userId?: string;
	parentCommentId?: string | null;
}

export interface ReportFilters extends PaginationParams {
	reportStatus?: ReportStatus;
	category?: ReportCategory;
	reportedEntityType?: CommentableType;
}

// Stats
export interface Stats {
	totalVideos: number;
	totalPosts: number;
	totalComments: number;
	totalReactions: number;
	myVideos: number;
	myPosts: number;
	myComments: number;
	pendingReports: number;
	flaggedContent: number;
}
