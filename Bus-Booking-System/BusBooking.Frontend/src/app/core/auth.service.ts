import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from './environment';

export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; name: string; }
export interface AuthResponse { token: string; }
export interface DecodedToken {
  sub: string;
  email: string;
  role: string;
  exp: number;
  /** From JWT Name claim when present */
  name?: string;
  /** Latest name from GET /User/profile (overrides display) */
  profileName?: string | null;
}

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
      tap((res) => {
        localStorage.setItem('token', res.token);
        const decoded = this.decodeToken(res.token);
        this.userSubject.next(decoded);
        this.refreshProfileFromApi();
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

  getEmail(): string | null {
    const user = this.userSubject.value;
    return user?.email ?? null;
  }

  /** Prefer server profile name, then JWT name, then email local-part. */
  getDisplayName(): string {
    const user = this.userSubject.value;
    if (!user) return '';
    if (user.profileName && user.profileName.trim()) return user.profileName.trim();
    if (user.name && user.name.trim()) return user.name.trim();
    if (user.email) return user.email.split('@')[0] ?? user.email;
    return 'Account';
  }

  /** Refreshes display name from GET /User/profile (all roles). */
  refreshProfileFromApi(): void {
    const token = this.getToken();
    if (!token) return;
    this.http.get<{ name?: string | null; email?: string; phone?: string | null; role?: string }>(`${this.API}/Auth/me`).subscribe({
      next: (p) => {
        const cur = this.userSubject.value;
        if (!cur) return;
        const roleFromApi = (p.role ?? '').toString().trim().toUpperCase();
        if (roleFromApi && roleFromApi !== cur.role) {
          localStorage.setItem('auth_notice', 'Your account role changed. Please log in again.');
          this.logout();
          return;
        }
        const next: DecodedToken = {
          ...cur,
          profileName: p.name ?? null,
          email: p.email ?? cur.email
        };
        this.userSubject.next(next);
      },
      error: () => {
        /* profile optional for navbar */
      }
    });
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
      const roleRaw = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;
      const role = Array.isArray(roleRaw) ? roleRaw[0] : roleRaw;
      const nameClaim =
        decoded.Name ||
        decoded.name ||
        decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        '';
      const roleNorm = (role != null && role !== '') ? String(role).trim().toUpperCase() : '';
      return {
        sub: decoded.sub || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        role: roleNorm,
        exp: decoded.exp,
        name: typeof nameClaim === 'string' ? nameClaim : ''
      };
    } catch { return null; }
  }
}
