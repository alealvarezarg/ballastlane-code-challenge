export interface ManagementUser {
  id: string;
  username: string;
  email: string;
}

export interface AuthenticatedSession {
  accessToken: string;
  tokenType: string;
  expiresAt: string;
  user: ManagementUser;
}

export interface SignUpRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
