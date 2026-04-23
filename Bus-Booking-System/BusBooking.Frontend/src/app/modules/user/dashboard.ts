import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  bookings: any[] = [];
  totalSpent = 0;
  loading = true;

  constructor(private http: HttpClient, private toast: ToastService) {}

  ngOnInit(): void {
    this.http.get<any>(`${environment.apiUrl}/User/dashboard`).subscribe({
      next: (data) => {
        this.bookings = data.bookings;
        this.totalSpent = data.totalSpent;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load dashboard data');
      }
    });
  }

  cancel(id: string): void {
    if (!confirm('Are you sure you want to cancel this booking?')) return;
    this.http.post(`${environment.apiUrl}/Booking/cancel/${id}`, {}).subscribe({
      next: () => {
        this.toast.success('Booking cancelled');
        const b = this.bookings.find(x => x.id === id);
        if (b) b.status = 'CANCELLED';
      },
      error: () => this.toast.error('Cannot cancel this booking')
    });
  }
}
