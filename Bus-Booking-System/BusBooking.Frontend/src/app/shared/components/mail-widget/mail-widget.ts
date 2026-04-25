import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/auth.service';
import { MailItem, MailService } from '../../../core/mail.service';
import { ToastService } from '../../../core/toast.service';
import { DialogService } from '../../../core/dialog.service';

@Component({
  selector: 'app-mail-widget',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mail-widget.html',
  styleUrls: ['./mail-widget.css']
})
export class MailWidgetComponent implements OnInit, OnDestroy {
  open = false;
  selected: MailItem | null = null;
  replyBody = '';

  unreadCount = 0;
  items: MailItem[] = [];

  private sub = new Subscription();

  constructor(
    public auth: AuthService,
    private mail: MailService,
    private toast: ToastService,
    private dialog: DialogService
  ) {}

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) this.mail.start();
    this.sub.add(this.auth.user$.subscribe(() => this.mail.start()));
    this.sub.add(this.mail.unreadCount$.subscribe((n) => (this.unreadCount = n)));
    this.sub.add(this.mail.items$.subscribe((items) => {
      this.items = items;
      if (this.selected) {
        const fresh = items.find((x) => x.id === this.selected!.id);
        this.selected = fresh ?? null;
      }
    }));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  toggleOpen(): void {
    if (!this.auth.isLoggedIn()) return;
    this.open = !this.open;
    if (this.open) this.mail.refresh();
  }

  select(item: MailItem): void {
    this.selected = item;
    if (item.direction === 'INBOX' && !item.isRead) this.mail.markRead(item.id);
  }

  sendReply(): void {
    if (!this.selected) return;
    const text = this.replyBody.trim();
    if (!text) {
      this.toast.warning('Reply text is required.');
      return;
    }
    this.mail.reply(this.selected.id, text, (ok, msg) => {
      if (ok) {
        this.toast.success(msg ?? 'Reply sent.');
        this.replyBody = '';
      } else {
        this.toast.error(msg ?? 'Reply failed.');
      }
    });
  }

  async deleteSelected(): Promise<void> {
    if (!this.selected) return;
    const ok = await this.dialog.confirm({
      title: 'Delete mail',
      message: 'Delete this mail from your mock inbox?',
      confirmText: 'Delete'
    });
    if (!ok) return;

    const id = this.selected.id;
    this.mail.remove(id, (success, msg) => {
      if (!success) {
        this.toast.error(msg ?? 'Failed to delete mail.');
        return;
      }
      this.toast.success(msg ?? 'Mail deleted.');
      this.selected = null;
    });
  }
}
