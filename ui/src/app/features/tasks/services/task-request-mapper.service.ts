import { Injectable } from '@angular/core';

import {
  CreateManagementTaskRequest,
  ManagementTask,
  PatchManagementTaskRequest,
  UpdateManagementTaskRequest
} from '@app/shared/models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskRequestMapperService {
  toCreateRequest(task: Partial<ManagementTask>): CreateManagementTaskRequest {
    return {
      title: task.title ?? '',
      description: task.description ?? '',
      status: task.status ?? 'Pending',
      dueDate: task.dueDate ?? '',
      userId: task.userId ?? ''
    };
  }

  toUpdateRequest(task: ManagementTask): UpdateManagementTaskRequest {
    return {
      id: task.id,
      title: task.title,
      description: task.description,
      status: task.status,
      dueDate: task.dueDate,
      userId: task.userId
    };
  }

  toPatchRequest(task: Partial<ManagementTask>): PatchManagementTaskRequest {
    return {
      title: task.title,
      description: task.description,
      dueDate: task.dueDate,
      userId: task.userId
    };
  }
}
