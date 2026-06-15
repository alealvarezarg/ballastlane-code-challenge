import { HttpErrorResponse } from '@angular/common/http';

import { ApiErrorState } from '@app/shared/models/api-error.models';

export function mapHttpError(error: HttpErrorResponse): ApiErrorState {
  const payload = error.error as Partial<ApiErrorState> | undefined;

  return {
    statusCode: payload?.statusCode ?? error.status,
    message: payload?.message ?? error.message ?? 'Unexpected API error.',
    traceId: payload?.traceId ?? '',
    errors: payload?.errors ?? []
  };
}
