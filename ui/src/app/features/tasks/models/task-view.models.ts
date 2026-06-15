import { ManagementTask, ManagementTaskSummary } from '@app/shared/models/task.models';
import { RequestStatus, initialRequestStatus } from '@app/shared/models/request-status.model';

export interface TasksState {
  tasks: ManagementTask[];
  summary: ManagementTaskSummary | null;
  totalCount: number;
  selectedTask: ManagementTask | null;
  query: {
    status?: 'Pending' | 'InProgress' | 'Completed' | null;
    userId?: string | null;
    search?: string | null;
    sortBy?: 'title' | 'status' | 'dueDate' | null;
    sortDirection?: 'asc' | 'desc' | null;
    page: number;
    pageSize: number;
    includeArchived: boolean;
  };
  loadStatus: RequestStatus;
  mutationStatus: RequestStatus;
}

export const initialTasksState = (): TasksState => ({
  tasks: [],
  summary: null,
  totalCount: 0,
  selectedTask: null,
  query: {
    page: 1,
    pageSize: 10,
    includeArchived: true,
    search: '',
    status: null,
    sortBy: null,
    sortDirection: null,
    userId: ''
  },
  loadStatus: initialRequestStatus(),
  mutationStatus: initialRequestStatus()
});
