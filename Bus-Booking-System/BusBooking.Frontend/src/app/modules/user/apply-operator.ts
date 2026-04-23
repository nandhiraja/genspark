import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-apply-operator',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './apply-operator.html',
  styleUrls: ['./dashboard.css']
})
export class ApplyOperatorComponent {
  companyName = '';
  loading = false;

  constructor(private http: HttpClient, private router: Router, private toast: ToastService) {}

  submit(): void {
    if (!this.companyName) { this.toast.warning('Enter your company name'); return; }
    this.loading = true;
    this.http.post(`${environment.apiUrl}/Operator/apply`, { companyName: this.companyName }).subscribe({
      next: () => {
        this.toast.success('Application submitted! An admin will review it.');
        this.router.navigate(['/user/dashboard']);
      },
      error: () => { this.loading = false; this.toast.error('Failed. You may have already applied.'); }
    });
  }
}
