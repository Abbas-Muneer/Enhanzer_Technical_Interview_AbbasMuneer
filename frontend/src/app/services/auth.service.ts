import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';

const AUTH_STORAGE_KEY = 'enhanzer-auth-state';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly loginUrl = `${environment.apiBaseUrl}/auth/login`;

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.loginUrl, payload).pipe(
      tap((response) => {
        if (response.success) {
          localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify({
            isAuthenticated: true,
            email: payload.email
          }));
        }
      }),
      catchError((error: HttpErrorResponse) => {
        const message = error.error?.message || 'Unable to login. Please try again.';
        return throwError(() => new Error(message));
      })
    );
  }

  logout(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }

  isAuthenticated(): boolean {
    const value = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!value) {
      return false;
    }

    try {
      const parsedValue = JSON.parse(value) as { isAuthenticated?: boolean };
      return parsedValue.isAuthenticated === true;
    } catch {
      return false;
    }
  }

  getAuthenticatedEmail(): string | null {
    const value = localStorage.getItem(AUTH_STORAGE_KEY);
    if (!value) {
      return null;
    }

    try {
      const parsedValue = JSON.parse(value) as { email?: string };
      return parsedValue.email ?? null;
    } catch {
      return null;
    }
  }
}
