export interface UserData {
  id: string;
  userName: string;
  name: string;
  roles: string[];
}

export interface UserLoginRequest {
  userName: string;
  password: string;
}

export interface UserLoginResponse {
  isSuccess: boolean;
  user: UserData | null;
  token: string | null;
  message: string | null;
}

export interface RegisterUserRequest {
  name: string;
  userName: string;
  password: string;
  // El backend fuerza este valor a "User" en el registro publico; se manda fijo
  // desde el front porque el DTO lo exige (ver UsersController.RegisterUser).
  role: 'User';
}

export interface RegisterUserResponse {
  isSuccess: boolean;
  message: string | null;
  user: UserData | null;
}
