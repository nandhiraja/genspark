import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from './environment';

export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; name: string; }
export interface AuthResponse { token: string; }
export interface DecodedToken { sub: string; email: string; role: string; exp: number; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = environment.apiUrl;
  private userSubject = new BehaviorSubject<DecodedToken | null>(this.getStoredUser());
  user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {}

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.API}/Auth/register`, data);
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API}/Auth/login`, data).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        const decoded = this.decodeToken(res.token);
        this.userSubject.next(decoded);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.userSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const decoded = this.decodeToken(token);
    return decoded ? decoded.exp * 1000 > Date.now() : false;
  }

  getRole(): string | null {
    const user = this.userSubject.value;
    return user ? user.role : null;
  }

  getUserId(): string | null {
    const user = this.userSubject.value;
    return user ? user.sub : null;
  }

  private getStoredUser(): DecodedToken | null {
    const token = this.getToken();
    if (!token) return null;
    const decoded = this.decodeToken(token);
    if (decoded && decoded.exp * 1000 > Date.now()) return decoded;
    localStorage.removeItem('token');
    return null;
  }

  private decodeToken(token: string): DecodedToken | null {
    try {
      const payload = token.split('.')[1];
      const decoded = JSON.parse(atob(payload));
      return {
        sub: decoded.sub || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role,
        exp: decoded.exp
      };
    } catch { return null; }
  }
}
