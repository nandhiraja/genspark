import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, interval, Subscription } from 'rxjs';
import { environment } from './environment';
import { AuthService } from './auth.service';

export interface MailItem {
  id: string;
  fromEmail: string;
  toEmail: string;
  subject: string;
  body: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string | null;
  parentMessageId?: string | null;
  direction: 'INBOX' | 'SENT';
}

interface InboxResponse {
  unreadCount: number;
  items: MailItem[];
}

@Injectable({ providedIn: 'root' })
export class MailService {
  private readonly API = `${environment.apiUrl}/Mail`;
  private pollSub: Subscription | null = null;

  readonly unreadCount$ = new BehaviorSubject<number>(0);
  readonly items$ = new BehaviorSubject<MailItem[]>([]);

  constructor(private http: HttpClient, private auth: AuthService) {}

  start(): void {
    if (!this.auth.isLoggedIn()) {
      this.stop();
      this.items$.next([]);
      this.unreadCount$.next(0);
      return;
    }
    if (this.pollSub) return;
    this.refresh();
    this.pollSub = interval(15000).subscribe(() => this.refresh());
  }

  stop(): void {
    this.pollSub?.unsubscribe();
    this.pollSub = null;
  }

  refresh(): void {
    if (!this.auth.isLoggedIn()) return;
    this.http.get<InboxResponse>(`${this.API}/inbox`).subscribe({
      next: (res) => {
        this.unreadCount$.next(res.unreadCount ?? 0);
        this.items$.next(res.items ?? []);
      },
      error: () => {}
    });
  }

  markRead(id: string): void {
    this.http.put(`${this.API}/${id}/read`, {}).subscribe({
      next: () => this.refresh(),
      error: () => {}
    });
  }

  reply(messageId: string, body: string, cb?: (ok: boolean, msg?: string) => void): void {
    this.http.post<{ message?: string }>(`${this.API}/reply`, { messageId, body }).subscribe({
      next: (res) => {
        this.refresh();
        cb?.(true, res?.message ?? 'Reply sent.');
      },
      error: (err) => cb?.(false, err?.error?.message ?? 'Failed to send reply.')
    });
  }

  remove(messageId: string, cb?: (ok: boolean, msg?: string) => void): void {
    this.http.delete<{ message?: string }>(`${this.API}/${messageId}`).subscribe({
      next: (res) => {
        this.refresh();
        cb?.(true, res?.message ?? 'Mail deleted.');
      },
      error: (err) => cb?.(false, err?.error?.message ?? 'Failed to delete mail.')
    });
  }
}
