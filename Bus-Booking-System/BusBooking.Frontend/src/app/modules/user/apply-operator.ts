import { Component, OnInit } from '@angular/core';
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
export class ApplyOperatorComponent implements OnInit {
  companyName = '';
  loading = false;

  // Status data
  statusLoading = true;
  existingStatus: string | null = null;
  existingCompanyName = '';

  constructor(
    private http: HttpClient, 
    public router: Router, 
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.checkStatus();
  }

  checkStatus(): void {
    this.statusLoading = true;
    this.http.get<{ status: string, companyName: string }>(`${environment.apiUrl}/Operator/my-status`).subscribe({
      next: (data) => {
        this.existingStatus = data.status;
        this.existingCompanyName = data.companyName;
        this.statusLoading = false;
      },
      error: () => {
        this.statusLoading = false;
        // 404 is expected if they haven't applied yet
      }
    });
  }

  submit(): void {
    if (!this.companyName) { this.toast.warning('Enter your company name'); return; }
    this.loading = true;
    this.http.post(`${environment.apiUrl}/Operator/apply`, { companyName: this.companyName }).subscribe({
      next: () => {
        this.toast.success('Application submitted! An admin will review it.');
        this.checkStatus();
      },
      error: () => { this.loading = false; this.toast.error('Failed. You may have already applied.'); }
    });
  }

  getStatusColor(): string {
    switch (this.existingStatus) {
      case 'PENDING': return '#f59e0b'; // Amber
      case 'APPROVED': return '#10b981'; // Green
      case 'REJECTED': return '#ef4444'; // Red
      default: return '#64748b'; // Slate
    }
  }

  getStatusMessage(): string {
    switch (this.existingStatus) {
      case 'PENDING': return 'Our team is currently reviewing your application. This usually takes 24-48 hours.';
      case 'APPROVED': return 'Congratulations! Your application has been approved. You can now manage your buses.';
      case 'REJECTED': return 'Unfortunately, your application was not approved at this time. Please contact support for more details.';
      default: return '';
    }
  }
}
