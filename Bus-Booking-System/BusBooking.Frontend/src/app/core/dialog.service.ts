import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface DialogConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  requireInput?: boolean;
  inputLabel?: string;
  inputPlaceholder?: string;
  inputValue?: string;
}

interface ActiveDialog extends DialogConfig {
  kind: 'confirm' | 'prompt';
}

@Injectable({ providedIn: 'root' })
export class DialogService {
  readonly active$ = new BehaviorSubject<ActiveDialog | null>(null);
  private resolver: ((value: unknown) => void) | null = null;

  confirm(config: DialogConfig): Promise<boolean> {
    return this.open<boolean>('confirm', config);
  }

  prompt(config: DialogConfig): Promise<string | null> {
    return this.open<string | null>('prompt', { ...config, requireInput: true });
  }

  resolveConfirm(accepted: boolean): void {
    if (!this.resolver) return;
    this.resolver(accepted);
    this.cleanup();
  }

  resolvePrompt(value: string | null): void {
    if (!this.resolver) return;
    this.resolver(value);
    this.cleanup();
  }

  private open<T>(kind: 'confirm' | 'prompt', config: DialogConfig): Promise<T> {
    if (this.active$.value) {
      this.cleanup();
    }
    this.active$.next({ kind, ...config });
    return new Promise<T>((resolve) => {
      this.resolver = (value: unknown) => resolve(value as T);
    });
  }

  private cleanup(): void {
    this.resolver = null;
    this.active$.next(null);
  }
}
