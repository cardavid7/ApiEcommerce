import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserData } from '../models/user.model';

// Endpoints restringidos a rol Admin en el backend (UsersController [Authorize(Roles = "Admin")]).
@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly apiUrl = `${environment.apiUrl}/v1/Users`;

  constructor(private readonly http: HttpClient) {}

  getUsers(): Observable<UserData[]> {
    return this.http.get<UserData[]>(this.apiUrl);
  }

  getUserById(id: string): Observable<UserData> {
    return this.http.get<UserData>(`${this.apiUrl}/${id}`);
  }
}
