import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

@Component({ selector: 'app-op-dashboard', standalone: true, imports: [CommonModule], templateUrl: './op-dashboard.html', styleUrls: ['./operator.css'] })
export class OpDashboardComponent implements OnInit {
  bookings: any[] = []; loading = true;
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Operator/bookings`).subscribe({
      next: d => { this.bookings = d; this.loading = false; },
      error: () => this.loading = false
    });
  }
}

@Component({ selector: 'app-add-bus', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './add-bus.html', styleUrls: ['./operator.css'] })
export class AddBusComponent implements OnInit {
  routes: any[] = []; routeId = ''; startTime = ''; endTime = ''; price = 0; totalSeats = 40; busNumber = ''; loading = false;
  constructor(private http: HttpClient, private toast: ToastService) {}
  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/routes`).subscribe({ next: d => this.routes = d, error: () => {} });
  }
  submit(): void {
    this.loading = true;
    this.http.post(`${environment.apiUrl}/Operator/add-bus`, {
      routeId: this.routeId, startTime: this.startTime, endTime: this.endTime,
      price: this.price, totalSeats: this.totalSeats, busNumber: this.busNumber
    }).subscribe({
      next: () => { this.loading = false; this.toast.success('Bus added!'); this.busNumber = ''; },
      error: () => { this.loading = false; this.toast.error('Failed. Are you an approved operator?'); }
    });
  }
}

@Component({ selector: 'app-manage-buses', standalone: true, imports: [CommonModule], templateUrl: './manage-buses.html', styleUrls: ['./operator.css'] })
export class ManageBusesComponent implements OnInit {
  buses: any[] = []; loading = true;
  constructor(private http: HttpClient, private toast: ToastService) {}
  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Operator/my-buses`).subscribe({
      next: d => { this.buses = d; this.loading = false; },
      error: () => this.loading = false
    });
  }
  toggleStatus(bus: any): void {
    const newStatusStr = bus.status === 'ACTIVE' ? 'INACTIVE' : 'ACTIVE';
    this.http.put(`${environment.apiUrl}/Operator/bus-status/${bus.id}?status=${newStatusStr}`, {}).subscribe({
      next: () => { bus.status = newStatusStr; this.toast.success('Status updated'); },
      error: () => this.toast.error('Failed to update')
    });
  }
}
