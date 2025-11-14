/**
 * BaseEntity Interface
 *
 * Base interface for all entities with common audit fields
 */
export interface BaseEntity {
  id: string;
  createdAt: Date;
  updatedAt: Date;
  deletedAt?: Date | null;
}
