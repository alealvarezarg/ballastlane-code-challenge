import { HttpParams } from '@angular/common/http';

import { TaskQueryState } from '@app/shared/models/task.models';

export function toTaskQueryParams(query: TaskQueryState): HttpParams {
  const page = query.page ?? 1;
  const pageSize = query.pageSize ?? 10;

  let params = new HttpParams()
    .set('page', page)
    .set('pageSize', pageSize)
    .set('includeArchived', query.includeArchived);

  if (query.status) {
    params = params.set('status', query.status);
  }
  if (query.userId) {
    params = params.set('userId', query.userId);
  }
  if (query.search) {
    params = params.set('search', query.search);
  }
  if (query.sortBy) {
    params = params.set('sortBy', query.sortBy);
  }
  if (query.sortDirection) {
    params = params.set('sortDirection', query.sortDirection);
  }

  return params;
}
