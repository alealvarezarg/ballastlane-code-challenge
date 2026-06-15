export interface LoginFormValue {
  email: string;
  password: string;
}

export interface SignUpFormValue extends LoginFormValue {
  username: string;
}
