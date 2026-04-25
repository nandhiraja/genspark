import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogService } from '../../../core/dialog.service';

@Component({
  selector: 'app-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dialog.html',
  styleUrls: ['./dialog.css']
})
export class DialogComponent {
  inputValue = '';

  constructor(public dialog: DialogService) {
    this.dialog.active$.subscribe((active) => {
      this.inputValue = active?.inputValue ?? '';
    });
  }

  onCancel(): void {
    const active = this.dialog.active$.value;
    if (!active) return;
    if (active.kind === 'prompt') {
      this.dialog.resolvePrompt(null);
      return;
    }
    this.dialog.resolveConfirm(false);
  }

  onConfirm(): void {
    const active = this.dialog.active$.value;
    if (!active) return;
    if (active.kind === 'prompt') {
      this.dialog.resolvePrompt(this.inputValue.trim() || null);
      return;
    }
    this.dialog.resolveConfirm(true);
  }

}
