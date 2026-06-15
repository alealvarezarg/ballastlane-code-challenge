export interface ValidationError {
  propertyName: string;
  message: string;
}

export interface ApiErrorState {
  statusCode: number;
  message: string;
  traceId: string;
  errors: ValidationError[];
}
