import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';
import { DialogService } from '../../core/dialog.service';

export interface TripBooking {
  id: string;
  status: string;
  totalAmount: number;
  createdAt?: string;
  busNumber: string;
  source: string;
  destination: string;
  departureTime: string;
  seatNumbers: number[];
  passengerNames: string[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  bookings: TripBooking[] = [];
  upcomingTrips: TripBooking[] = [];
  pastTrips: TripBooking[] = [];
  totalSpent = 0;
  loading = true;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  get cancelledTrips(): TripBooking[] {
    return this.bookings.filter((b) => b.status.toUpperCase() === 'CANCELLED');
  }

  seatsLabel(b: TripBooking): string {
    if (!b.seatNumbers?.length) return '';
    return `Seats: ${b.seatNumbers.join(', ')}`;
  }

  passengersLabel(b: TripBooking): string {
    if (!b.passengerNames?.length) return '';
    return b.passengerNames.join(', ');
  }

  cancelRefundSummary(departureIso: string): string {
    if (!departureIso) return '';
    const dep = new Date(departureIso).getTime();
    const now = Date.now();
    const hours = (dep - now) / 3600000;
    if (hours >= 24) return 'You will receive a full refund (100%).';
    if (hours >= 12) return 'You will receive a 50% refund.';
    return 'No refund for this timing, but your seats will be released for others.';
  }

  loadDashboard(): void {
    this.loading = true;
    const url = `${environment.apiUrl}/User/dashboard`;
    this.http.get<Record<string, unknown>>(url).subscribe({
      next: (data) => {
        this.applyDashboardPayload(data);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load dashboard data');
      }
    });
  }

  private applyDashboardPayload(data: Record<string, unknown>): void {
    const rawAll = data['allBookings'] ?? data['AllBookings'];
    const rawSpent = data['totalSpent'] ?? data['TotalSpent'];

    this.bookings = this.normalizeBookingList(rawAll);

    const isConfirmed = (status: string) => status.trim().toUpperCase() === 'CONFIRMED';
    const depMs = (iso: string) => {
      const t = new Date(iso).getTime();
      return Number.isNaN(t) ? 0 : t;
    };
    const now = Date.now();

    this.upcomingTrips = this.bookings.filter(
      (b) => isConfirmed(b.status) && depMs(b.departureTime) > now
    );
    this.pastTrips = this.bookings.filter(
      (b) => isConfirmed(b.status) && depMs(b.departureTime) > 0 && depMs(b.departureTime) <= now
    );
    this.totalSpent = this.bookings.filter((b) => isConfirmed(b.status)).reduce((s, b) => s + b.totalAmount, 0);

    const serverSpent = typeof rawSpent === 'number' ? rawSpent : Number(rawSpent);
    if (!Number.isNaN(serverSpent) && serverSpent > 0) this.totalSpent = serverSpent;
  }

  private normalizeBookingList(raw: unknown): TripBooking[] {
    if (!Array.isArray(raw)) return [];
    return raw.map((b: Record<string, unknown>) => this.normalizeBooking(b));
  }

  private normalizeBooking(b: Record<string, unknown>): TripBooking {
    const rawSeats = b['seatNumbers'] ?? b['SeatNumbers'];
    const rawNames = b['passengerNames'] ?? b['PassengerNames'];
    const seatNumbers = Array.isArray(rawSeats)
      ? (rawSeats as unknown[]).map((x) => Number(x)).filter((n) => !Number.isNaN(n))
      : [];
    const passengerNames = Array.isArray(rawNames)
      ? (rawNames as unknown[]).map((x) => String(x ?? '')).filter((s) => s.length > 0)
      : [];
    return {
      id: String(b['id'] ?? b['Id'] ?? ''),
      status: String(b['status'] ?? b['Status'] ?? '')
        .trim()
        .toUpperCase(),
      totalAmount: Number(b['totalAmount'] ?? b['TotalAmount'] ?? 0),
      createdAt: (b['createdAt'] ?? b['CreatedAt']) as string | undefined,
      busNumber: String(b['busNumber'] ?? b['BusNumber'] ?? ''),
      source: String(b['source'] ?? b['Source'] ?? ''),
      destination: String(b['destination'] ?? b['Destination'] ?? ''),
      departureTime: String(b['departureTime'] ?? b['DepartureTime'] ?? ''),
      seatNumbers,
      passengerNames
    };
  }

  formatDate(dt: string): string {
    if (!dt) return '—';
    return new Date(dt).toLocaleString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  bookingRef(id: string): string {
    if (!id) return '—';
    return id.replace(/-/g, '').slice(0, 8).toUpperCase();
  }

  async cancel(id: string, departureTime: string): Promise<void> {
    const trip = this.upcomingTrips.find((t) => t.id === id);
    const dep = departureTime || trip?.departureTime || '';
    const refundLine = this.cancelRefundSummary(dep);
    const ok = await this.dialog.confirm({
      title: 'Cancel booking',
      message: `Cancel this booking?\n\n${refundLine}\n\nSeats will become available for other passengers.`,
      confirmText: 'Yes, cancel booking'
    });
    if (!ok) return;
    this.http.post(`${environment.apiUrl}/Booking/cancel/${id}`, {}).subscribe({
      next: () => {
        this.toast.success('Booking cancelled');
        this.loadDashboard();
      },
      error: () => this.toast.error('Cannot cancel this booking')
    });
  }
}
