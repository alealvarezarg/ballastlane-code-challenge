import { ApiErrorState } from './api-error.models';

export interface RequestStatus {
  loading: boolean;
  successMessage: string | null;
  error: ApiErrorState | null;
}

export const initialRequestStatus = (): RequestStatus => ({
  loading: false,
  successMessage: null,
  error: null
});
