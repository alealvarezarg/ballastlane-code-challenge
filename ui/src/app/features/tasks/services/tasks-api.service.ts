import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { APP_ENVIRONMENT } from '@app/core/providers/api-config.provider';
import { AppEnvironment } from '@app/shared/models/app-environment.model';
import {
  CreateManagementTaskRequest,
  ManagementTask,
  ManagementTaskSummary,
  PagedManagementTaskResponse,
  TaskQueryState,
  UpdateManagementTaskRequest,
  UpdateManagementTaskStatusRequest
} from '@app/shared/models/task.models';
import { toTaskQueryParams } from '@app/shared/utils/task-query.util';

@Injectable({ providedIn: 'root' })
export class TasksApiService {
  constructor(
    private readonly httpClient: HttpClient,
    @Inject(APP_ENVIRONMENT) private readonly environment: AppEnvironment
  ) {}

  queryTasks(query: TaskQueryState): Observable<PagedManagementTaskResponse> {
    return this.httpClient.get<PagedManagementTaskResponse>(`${this.environment.apiBaseUrl}/management-tasks`, {
      params: toTaskQueryParams(query)
    });
  }

  getSummary(includeArchived: boolean): Observable<ManagementTaskSummary> {
    return this.httpClient.get<ManagementTaskSummary>(`${this.environment.apiBaseUrl}/management-tasks/summary`, {
      params: { includeArchived }
    });
  }

  getOverdue(includeArchived: boolean): Observable<ManagementTask[]> {
    return this.httpClient.get<ManagementTask[]>(`${this.environment.apiBaseUrl}/management-tasks/overdue`, {
      params: { includeArchived }
    });
  }

  getDueWithin(days: number, includeArchived: boolean): Observable<ManagementTask[]> {
    return this.httpClient.get<ManagementTask[]>(`${this.environment.apiBaseUrl}/management-tasks/due-within`, {
      params: { days, includeArchived }
    });
  }

  createTask(request: CreateManagementTaskRequest): Observable<ManagementTask> {
    return this.httpClient.post<ManagementTask>(`${this.environment.apiBaseUrl}/management-tasks`, request);
  }

  updateTask(id: string, request: UpdateManagementTaskRequest): Observable<ManagementTask> {
    return this.httpClient.put<ManagementTask>(`${this.environment.apiBaseUrl}/management-tasks/${id}`, request);
  }

  updateTaskStatus(id: string, request: UpdateManagementTaskStatusRequest): Observable<ManagementTask> {
    return this.httpClient.patch<ManagementTask>(`${this.environment.apiBaseUrl}/management-tasks/${id}/status`, request);
  }

  archiveTask(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.environment.apiBaseUrl}/management-tasks/${id}`);
  }
}
