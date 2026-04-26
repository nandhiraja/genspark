import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';
import { DialogService } from '../../core/dialog.service';

@Component({ selector: 'app-op-dashboard', standalone: true, imports: [CommonModule], templateUrl: './op-dashboard.html', styleUrls: ['./operator.css'] })
export class OpDashboardComponent implements OnInit {
  loading = true;
  revenue: any = null;
  bookings: any[] = [];
  buses: any[] = [];
  notifications: string[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any>(`${environment.apiUrl}/Operator/revenue`).subscribe({
      next: (d) => {
        this.revenue = d;
      },
      error: () => {}
    });

    this.http.get<any[]>(`${environment.apiUrl}/Operator/bookings`).subscribe({
      next: d => { this.bookings = d ?? []; this.checkLoaded(); },
      error: () => this.checkLoaded()
    });

    this.http.get<any[]>(`${environment.apiUrl}/Operator/my-buses`).subscribe({
      next: d => { this.buses = d ?? []; this.buildNotifications(); this.checkLoaded(); },
      error: () => this.checkLoaded()
    });
  }

  private checkLoaded(): void {
    if (this.bookings != null && this.buses != null) this.loading = false;
  }

  private buildNotifications(): void {
    const pending = this.buses.filter(b => b.status === 'PENDING_APPROVAL').length;
    const hidden = this.buses.filter(b => !!b.hiddenFromSearch).length;
    const adminDown = this.buses.filter(b => !!b.adminDeactivated).length;
    const notes: string[] = [];
    if (pending > 0) notes.push(`${pending} bus(es) are waiting for admin approval.`);
    if (hidden > 0) notes.push(`${hidden} bus(es) are hidden from user search due to pending route changes or status.`);
    if (adminDown > 0) notes.push(`${adminDown} bus(es) were deactivated by admin; submit reactivation requests from Manage Buses.`);
    this.notifications = notes;
  }
}

@Component({ selector: 'app-add-bus', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './add-bus.html', styleUrls: ['./operator.css'] })
export class AddBusComponent implements OnInit {
  routes: any[] = [];
  routeId = '';
  startTime = '';
  endTime = '';
  price = 0;
  totalSeats = 40;
  busNumber = '';
  sourceBoardingPointId = '';
  destinationBoardingPointId = '';
  sourceOptions: any[] = [];
  destinationOptions: any[] = [];
  loading = false;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) {}

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/routes`).subscribe({
      next: d => {
        this.routes = d ?? [];
      },
      error: () => {}
    });
  }

  onRouteChanged(): void {
    this.sourceBoardingPointId = '';
    this.destinationBoardingPointId = '';
    this.sourceOptions = [];
    this.destinationOptions = [];

    const selected = this.routes.find(r => r.id === this.routeId);
    if (!selected) return;

    this.http.get<any[]>(`${environment.apiUrl}/Operator/boarding-points`, { params: { city: selected.source } }).subscribe({
      next: points => this.sourceOptions = points ?? [],
      error: () => this.sourceOptions = []
    });
    this.http.get<any[]>(`${environment.apiUrl}/Operator/boarding-points`, { params: { city: selected.destination } }).subscribe({
      next: points => this.destinationOptions = points ?? [],
      error: () => this.destinationOptions = []
    });
  }

  submit(): void {
    this.loading = true;
    this.http.post(`${environment.apiUrl}/Operator/add-bus`, {
      routeId: this.routeId,
      startTime: this.startTime,
      endTime: this.endTime,
      price: this.price,
      totalSeats: this.totalSeats,
      busNumber: this.busNumber,
      sourceBoardingPointId: this.sourceBoardingPointId || null,
      destinationBoardingPointId: this.destinationBoardingPointId || null
    }).subscribe({
      next: () => {
        this.loading = false;
        this.toast.success('Bus submitted for admin approval.');
        this.busNumber = '';
        this.routeId = '';
        this.startTime = '';
        this.endTime = '';
        this.price = 0;
        this.totalSeats = 40;
        this.sourceBoardingPointId = '';
        this.destinationBoardingPointId = '';
      },
      error: (err) => {
        this.loading = false;
        this.toast.error(err?.error?.message ?? 'Failed to add bus.');
      }
    });
  }
}

@Component({ selector: 'app-manage-buses', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './manage-buses.html', styleUrls: ['./operator.css'] })
export class ManageBusesComponent implements OnInit {
  buses: any[] = [];
  routes: any[] = [];
  loading = true;
  changingBusId: string | null = null;
  newRouteId = '';
  newSourceBoardingPointId = '';
  newDestinationBoardingPointId = '';
  routeReason = '';
  sourceOptions: any[] = [];
  destinationOptions: any[] = [];

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) {}

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/routes`).subscribe({ next: d => this.routes = d ?? [], error: () => {} });
    this.load();
  }

  load(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Operator/my-buses`).subscribe({
      next: d => { this.buses = d; this.loading = false; },
      error: () => this.loading = false
    });
  }

  toggleStatus(bus: any): void {
    if (bus.adminDeactivated && bus.status !== 'ACTIVE') {
      this.toast.warning('Admin deactivated this bus. Request reactivation instead.');
      return;
    }
    const newStatusStr = bus.status === 'ACTIVE' ? 'INACTIVE' : 'ACTIVE';
    this.http.put(`${environment.apiUrl}/Operator/bus-status/${bus.id}?status=${newStatusStr}`, {}).subscribe({
      next: () => { bus.status = newStatusStr; this.toast.success('Status updated'); },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to update')
    });
  }

  openRouteChange(bus: any): void {
    this.changingBusId = bus.id;
    this.newRouteId = '';
    this.newSourceBoardingPointId = '';
    this.newDestinationBoardingPointId = '';
    this.routeReason = '';
    this.sourceOptions = [];
    this.destinationOptions = [];
  }

  onNewRouteChanged(): void {
    const selected = this.routes.find(r => r.id === this.newRouteId);
    if (!selected) return;
    this.http.get<any[]>(`${environment.apiUrl}/Operator/boarding-points`, { params: { city: selected.source } }).subscribe({
      next: points => this.sourceOptions = points ?? [],
      error: () => this.sourceOptions = []
    });
    this.http.get<any[]>(`${environment.apiUrl}/Operator/boarding-points`, { params: { city: selected.destination } }).subscribe({
      next: points => this.destinationOptions = points ?? [],
      error: () => this.destinationOptions = []
    });
  }

  submitRouteChange(bus: any): void {
    this.http.put(`${environment.apiUrl}/Operator/bus/${bus.id}/route-change`, {
      newRouteId: this.newRouteId,
      newSourceBoardingPointId: this.newSourceBoardingPointId || null,
      newDestinationBoardingPointId: this.newDestinationBoardingPointId || null,
      reason: this.routeReason || null
    }).subscribe({
      next: () => {
        this.toast.success('Route change request sent to admin.');
        this.changingBusId = null;
        this.load();
      },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to send route change request')
    });
  }

  async requestReactivation(bus: any): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Request bus reactivation',
      message: 'Share a short reason with admin for reactivating this bus.',
      inputLabel: 'Reason',
      inputPlaceholder: 'Enter reason for reactivation',
      confirmText: 'Submit request'
    });
    if (reason === null) return;
    this.http.post(`${environment.apiUrl}/Operator/bus/${bus.id}/reactivation-request`, { reason: reason || null }).subscribe({
      next: () => {
        bus.reactivationRequest = {
          status: 'PENDING',
          operatorReason: reason || null,
          adminReason: null,
          createdAt: new Date().toISOString(),
          reviewedAt: null
        };
        this.toast.success('Reactivation request submitted.');
      },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to submit reactivation request')
    });
  }

  hasPendingReactivation(bus: any): boolean {
    return (bus?.reactivationRequest?.status ?? '').toString().toUpperCase() === 'PENDING';
  }

  async deleteBus(bus: any): Promise<void> {
    const ok = await this.dialog.confirm({
      title: 'Delete bus',
      message: `Delete bus ${bus.busNumber} permanently?\n\nThis action cannot be undone.`,
      confirmText: 'Delete Bus'
    });
    if (!ok) return;

    this.http.delete(`${environment.apiUrl}/Operator/bus/${bus.id}`).subscribe({
      next: () => {
        this.toast.success('Bus deleted.');
        this.buses = this.buses.filter((b) => b.id !== bus.id);
      },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to delete bus')
    });
  }
}

@Component({ selector: 'app-operator-boarding-points', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './boarding-points.html', styleUrls: ['./operator.css'] })
export class OperatorBoardingPointsComponent implements OnInit {
  cities: any[] = [];
  points: any[] = [];
  city = '';
  label = '';
  addressLine = '';
  loading = true;
  editingId: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) {}

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Operator/cities`).subscribe({
      next: (d) => this.cities = d ?? [],
      error: () => this.cities = []
    });
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/Operator/boarding-points`).subscribe({
      next: d => { this.points = d ?? []; this.loading = false; },
      error: () => { this.loading = false; this.toast.error('Failed to load boarding points'); }
    });
  }

  edit(p: any): void {
    this.editingId = p.id;
    this.city = p.city;
    this.label = p.label;
    this.addressLine = p.addressLine;
  }

  clearForm(): void {
    this.editingId = null;
    this.city = '';
    this.label = '';
    this.addressLine = '';
  }

  save(): void {
    if (!this.city || !this.label || !this.addressLine) {
      this.toast.warning('City, label, and address are required.');
      return;
    }
    const payload = { city: this.city, label: this.label, addressLine: this.addressLine };
    if (!this.editingId) {
      this.http.post(`${environment.apiUrl}/Operator/boarding-points`, payload).subscribe({
        next: () => { this.toast.success('Boarding point added'); this.clearForm(); this.load(); },
        error: (err) => this.toast.error(err?.error?.message ?? 'Failed to add boarding point')
      });
      return;
    }
    this.http.put(`${environment.apiUrl}/Operator/boarding-points/${this.editingId}`, payload).subscribe({
      next: () => { this.toast.success('Boarding point updated'); this.clearForm(); this.load(); },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to update boarding point')
    });
  }

  async remove(p: any): Promise<void> {
    const ok = await this.dialog.confirm({
      title: 'Remove boarding point',
      message: `Remove boarding point "${p.label}"?`,
      confirmText: 'Remove'
    });
    if (!ok) return;
    this.http.delete(`${environment.apiUrl}/Operator/boarding-points/${p.id}`).subscribe({
      next: () => { this.toast.success('Boarding point removed'); this.load(); },
      error: (err) => this.toast.error(err?.error?.message ?? 'Failed to remove boarding point')
    });
  }
}
