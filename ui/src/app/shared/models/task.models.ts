export type ManagementTaskStatus = 'Pending' | 'InProgress' | 'Completed';

export interface ManagementTask {
  id: string;
  title: string;
  description: string;
  status: ManagementTaskStatus;
  dueDate: string;
  userId: string;
  isArchived: boolean;
  archivedAt: string | null;
}

export interface PagedManagementTaskResponse {
  items: ManagementTask[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface ManagementTaskStatusCount {
  status: ManagementTaskStatus;
  count: number;
}

export interface ManagementTaskSummary {
  statuses: ManagementTaskStatusCount[];
}

export interface CreateManagementTaskRequest {
  title: string;
  description: string;
  status?: ManagementTaskStatus | null;
  dueDate: string;
  userId: string;
}

export interface UpdateManagementTaskRequest {
  id: string;
  title: string;
  description: string;
  status: ManagementTaskStatus;
  dueDate: string;
  userId: string;
}

export interface PatchManagementTaskRequest {
  title?: string;
  description?: string;
  dueDate?: string;
  userId?: string;
}

export interface UpdateManagementTaskStatusRequest {
  status: ManagementTaskStatus;
}

export interface TaskQueryState {
  status?: ManagementTaskStatus | null;
  userId?: string | null;
  search?: string | null;
  sortBy?: 'title' | 'status' | 'dueDate' | null;
  sortDirection?: 'asc' | 'desc' | null;
  page: number;
  pageSize: number;
  includeArchived: boolean;
}
