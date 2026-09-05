import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  RegisterUserRequest,
  RegisterUserResponse,
  UserData,
  UserLoginRequest,
  UserLoginResponse,
} from '../models/user.model';

const TOKEN_KEY = 'apiecommerce_token';
const USER_KEY = 'apiecommerce_user';

interface JwtPayload {
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/v1/Users`;

  private readonly tokenSignal = signal<string | null>(this.readToken());
  private readonly userSignal = signal<UserData | null>(this.readUser());

  readonly currentUser = this.userSignal.asReadonly();

  // Se recalcula solo con los signals de sesion (puros): no hace logout aqui
  // adentro porque un computed no puede escribir otros signals (NG0600).
  readonly isAuthenticated = computed(() => {
    const token = this.tokenSignal();
    return !!token && !this.isTokenExpired(token);
  });

  constructor(private readonly http: HttpClient) {}

  get token(): string | null {
    return this.tokenSignal();
  }

  login(request: UserLoginRequest): Observable<UserLoginResponse> {
    return this.http.post<UserLoginResponse>(`${this.apiUrl}/Login`, request).pipe(
      tap((response) => {
        if (response.isSuccess && response.token && response.user) {
          this.persistSession(response.token, response.user);
        }
      }),
    );
  }

  register(request: RegisterUserRequest): Observable<RegisterUserResponse> {
    return this.http.post<RegisterUserResponse>(`${this.apiUrl}/Register`, request);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.tokenSignal.set(null);
    this.userSignal.set(null);
  }

  hasRole(role: string): boolean {
    return !!this.userSignal()?.roles.includes(role);
  }

  private persistSession(token: string, user: UserData): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this.tokenSignal.set(token);
    this.userSignal.set(user);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const payload = jwtDecode<JwtPayload>(token);
      return payload.exp * 1000 <= Date.now();
    } catch {
      return true;
    }
  }

  private readToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  private readUser(): UserData | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? (JSON.parse(raw) as UserData) : null;
    } catch {
      return null;
    }
  }
}
