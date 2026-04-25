import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, distinctUntilChanged, filter, map, of, switchMap, tap } from 'rxjs';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';
import { DialogService } from '../../core/dialog.service';

@Component({
  selector: 'app-admin-nav',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './admin-nav.html',
  styleUrls: ['./admin.css']
})
export class AdminNavComponent implements OnInit {
  pendingBusApprovals = 0;
  pendingOperators = 0;
  pendingReactivations = 0;
  loadingCounts = true;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadCounts();
  }

  private loadCounts(): void {
    this.loadingCounts = true;

    this.http
      .get<any>(`${environment.apiUrl}/Admin/dashboard`)
      .subscribe({
        next: (d) => {
          this.pendingBusApprovals = Number(d?.pendingBusApprovals ?? d?.PendingBusApprovals ?? 0) || 0;
          this.pendingOperators = Number(d?.pendingOperators ?? d?.PendingOperators ?? 0) || 0;
          this.loadReactivationCount();
        },
        error: () => {
          this.pendingBusApprovals = 0;
          this.pendingOperators = 0;
          this.loadReactivationCount();
        }
      });
  }

  private loadReactivationCount(): void {
    this.http
      .get<any[]>(`${environment.apiUrl}/Admin/reactivation-requests`, { params: { status: 'PENDING' } })
      .subscribe({
        next: (rows) => {
          this.pendingReactivations = (rows ?? []).length;
          this.loadingCounts = false;
        },
        error: () => {
          this.pendingReactivations = 0;
          this.loadingCounts = false;
        }
      });
  }

  totalPending(): number {
    return this.pendingBusApprovals + this.pendingOperators + this.pendingReactivations;
  }
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, AdminNavComponent],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin.css']
})
export class AdminDashboardComponent implements OnInit {
  stats: any = {};
  loading = true;

  constructor(private http: HttpClient) { }

  private normalizeDashboard(d: any): any {
    if (!d || typeof d !== 'object') return {};
    const n = (camel: string, pascal: string, def: number | null = 0) =>
      d[camel] ?? d[pascal] ?? def;
    const arr = (camel: string, pascal: string) => (d[camel] ?? d[pascal] ?? []) as any[];
    const ops = arr('operatorBreakdown', 'OperatorBreakdown').map((o) => ({
      id: o.id ?? o.Id,
      companyName: o.companyName ?? o.CompanyName,
      totalBookings: o.totalBookings ?? o.TotalBookings ?? 0,
      activeBuses: o.activeBuses ?? o.ActiveBuses ?? 0,
      totalRevenue: o.totalRevenue ?? o.TotalRevenue ?? 0
    }));
    const routes = arr('routeBreakdown', 'RouteBreakdown').map((r) => ({
      source: r.source ?? r.Source,
      destination: r.destination ?? r.Destination,
      revenue: r.revenue ?? r.Revenue ?? 0,
      bookings: r.bookings ?? r.Bookings ?? 0
    }));
    return {
      totalBookingValue: n('totalBookingValue', 'TotalBookingValue'),
      platformRevenue: n('platformRevenue', 'PlatformRevenue'),
      totalConfirmedBookings: n('totalConfirmedBookings', 'TotalConfirmedBookings'),
      activeBookersLast30Days: n('activeBookersLast30Days', 'ActiveBookersLast30Days'),
      registeredTravellers: n('registeredTravellers', 'RegisteredTravellers'),
      activeBuses: n('activeBuses', 'ActiveBuses'),
      pendingBusApprovals: n('pendingBusApprovals', 'PendingBusApprovals'),
      pendingOperators: n('pendingOperators', 'PendingOperators'),
      approvedOperators: n('approvedOperators', 'ApprovedOperators'),
      operatorBreakdown: ops,
      routeBreakdown: routes
    };
  }

  ngOnInit(): void {
    this.http.get<any>(`${environment.apiUrl}/Admin/dashboard`).subscribe({
      next: (d) => {
        this.stats = this.normalizeDashboard(d);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}

@Component({
  selector: 'app-admin-bus-requests',
  standalone: true,
  imports: [CommonModule, AdminNavComponent],
  templateUrl: './admin-bus-requests.html',
  styleUrls: ['./admin.css']
})
export class AdminBusRequestsComponent implements OnInit {
  buses: any[] = [];
  loading = true;
  acting: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/buses`, { params: { status: 'PENDING_APPROVAL' } }).subscribe({
      next: (d) => {
        this.buses = (d ?? []).map((b) => ({
          id: b.id ?? b.Id,
          busNumber: b.busNumber ?? b.BusNumber,
          source: b.source ?? b.Source,
          destination: b.destination ?? b.Destination,
          operatorName: b.operatorName ?? b.OperatorName,
          startTime: b.startTime ?? b.StartTime,
          status: b.status ?? b.Status
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load bus requests');
      }
    });
  }

  approveBus(id: string): void {
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/approve-bus/${id}`, {}).subscribe({
      next: () => {
        this.toast.success('Bus approved');
        this.buses = this.buses.filter((b) => b.id !== id);
        this.acting = null;
      },
      error: () => {
        this.acting = null;
        this.toast.error('Approve failed');
      }
    });
  }
}

@Component({
  selector: 'app-admin-operator-requests',
  standalone: true,
  imports: [CommonModule, AdminNavComponent, RouterLink],
  templateUrl: './admin-operator-requests.html',
  styleUrls: ['./admin.css']
})
export class AdminOperatorRequestsComponent implements OnInit {
  operators: any[] = [];
  loading = true;
  acting: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/operators`, { params: { status: 'PENDING' } }).subscribe({
      next: (d) => {
        this.operators = (d ?? []).map((o) => ({
          id: o.operatorId ?? o.OperatorId ?? o.id ?? o.Id,
          companyName: o.companyName ?? o.CompanyName,
          status: o.status ?? o.Status,
          ownerName: o.ownerName ?? o.OwnerName,
          userEmail: o.userEmail ?? o.UserEmail
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load applications');
      }
    });
  }

  approve(id: string): void {
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/approve-operator/${id}`, {}).subscribe({
      next: () => {
        this.toast.success('Operator approved');
        this.operators = this.operators.filter((o) => o.id !== id);
        this.acting = null;
      },
      error: () => {
        this.acting = null;
        this.toast.error('Approve failed');
      }
    });
  }

  async reject(id: string): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Reject operator application',
      message: 'Optionally add a reason. This may be shared by email.',
      inputLabel: 'Reason (optional)',
      inputPlaceholder: 'Enter reason',
      confirmText: 'Reject application'
    });
    if (reason === null) return;
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/reject-operator/${id}`, { reason: reason || null }).subscribe({
      next: () => {
        this.toast.success('Application rejected');
        this.operators = this.operators.filter((o) => o.id !== id);
        this.acting = null;
      },
      error: () => {
        this.acting = null;
        this.toast.error('Reject failed');
      }
    });
  }
}

@Component({
  selector: 'app-admin-operators',
  standalone: true,
  imports: [CommonModule, AdminNavComponent, RouterLink],
  templateUrl: './admin-operators.html',
  styleUrls: ['./admin.css']
})
export class AdminOperatorsComponent implements OnInit {
  operators: any[] = [];
  loading = true;
  filter: 'ALL' | 'PENDING' | 'APPROVED' | 'REJECTED' = 'ALL';

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/Admin/operators`).subscribe({
      next: (d) => {
        this.operators = (d ?? []).map((o) => ({
          id: o.operatorId ?? o.OperatorId ?? o.id ?? o.Id,
          userId: o.userId ?? o.UserId,
          companyName: o.companyName ?? o.CompanyName,
          status: o.status ?? o.Status,
          ownerName: o.ownerName ?? o.OwnerName,
          userEmail: o.userEmail ?? o.UserEmail
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load operators');
      }
    });
  }

  st(s: string | undefined): string {
    return (s ?? '').toString().trim().toUpperCase();
  }

  setFilter(f: typeof this.filter): void {
    this.filter = f;
  }

  filtered(): any[] {
    if (this.filter === 'ALL') return this.operators;
    return this.operators.filter((o) => this.st(o.status) === this.filter);
  }

  approve(id: string): void {
    this.http.put(`${environment.apiUrl}/Admin/approve-operator/${id}`, {}).subscribe({
      next: () => {
        this.toast.success('Operator approved');
        const op = this.operators.find((o) => o.id === id);
        if (op) op.status = 'APPROVED';
      },
      error: () => this.toast.error('Approve failed')
    });
  }

  async reject(id: string): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Reject operator',
      message: 'Optionally add a reason. This may be shared by email.',
      inputLabel: 'Reason (optional)',
      inputPlaceholder: 'Enter reason',
      confirmText: 'Reject operator'
    });
    if (reason === null) return;
    this.http.put(`${environment.apiUrl}/Admin/reject-operator/${id}`, { reason: reason || null }).subscribe({
      next: () => {
        this.toast.success('Application rejected');
        const op = this.operators.find((o) => o.id === id);
        if (op) op.status = 'REJECTED';
      },
      error: () => this.toast.error('Reject failed')
    });
  }
}

@Component({
  selector: 'app-admin-operator-detail',
  standalone: true,
  imports: [CommonModule, AdminNavComponent, RouterLink],
  templateUrl: './admin-operator-detail.html',
  styleUrls: ['./admin.css']
})
export class AdminOperatorDetailComponent {
  detail: any = null;
  loading = true;
  acting: string | null = null;

  private readonly destroyRef = inject(DestroyRef);

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) {
    this.route.paramMap
      .pipe(
        map((pm) => pm.get('operatorId')),
        filter((id): id is string => !!id && id.trim().length > 0),
        distinctUntilChanged(),
        tap(() => {
          this.loading = true;
          this.detail = null;
        }),
        switchMap((id) =>
          this.http.get<any>(`${environment.apiUrl}/Admin/operator/${id}`).pipe(
            catchError(() => {
              this.loading = false;
              this.detail = null;
              this.toast.error('Operator not found');
              return of(null);
            })
          )
        ),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((d) => {
        if (d == null) return;
        this.applyDetailPayload(d);
        this.loading = false;
      });
  }

  private applyDetailPayload(d: any): void {
    const rawOwner = d.owner ?? d.Owner;
    const owner =
      rawOwner == null
        ? null
        : {
          id: rawOwner.id ?? rawOwner.Id,
          name: rawOwner.name ?? rawOwner.Name,
          email: rawOwner.email ?? rawOwner.Email,
          role: rawOwner.role ?? rawOwner.Role
        };
    this.detail = {
      id: d.id ?? d.Id,
      companyName: d.companyName ?? d.CompanyName,
      status: d.status ?? d.Status,
      createdAt: d.createdAt ?? d.CreatedAt,
      logoUrl: d.logoUrl ?? d.LogoUrl,
      owner,
      buses: (d.buses ?? d.Buses ?? []).map((b: any) => ({
        id: b.id ?? b.Id,
        busNumber: b.busNumber ?? b.BusNumber,
        status: b.status ?? b.Status,
        price: b.price ?? b.Price,
        totalSeats: b.totalSeats ?? b.TotalSeats,
        availableSeats: b.availableSeats ?? b.AvailableSeats,
        startTime: b.startTime ?? b.StartTime,
        endTime: b.endTime ?? b.EndTime,
        source: b.source ?? b.Source,
        destination: b.destination ?? b.Destination
      }))
    };
  }

  statusU(s: string | undefined): string {
    return (s ?? '').toString().trim().toUpperCase();
  }

  async deactivateBus(id: string, busNumber: string): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Deactivate bus',
      message: `Reason for deactivating bus ${busNumber}. This is emailed to the operator.`,
      inputLabel: 'Reason',
      inputPlaceholder: 'Enter deactivation reason',
      confirmText: 'Deactivate bus'
    });
    if (reason === null) return;
    if (!reason.trim()) {
      this.toast.warning('Please enter a short reason for the operator.');
      return;
    }
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/bus/${id}/deactivate`, { reason }).subscribe({
      next: () => {
        this.toast.success('Bus deactivated; operator notified by email.');
        const b = this.detail?.buses?.find((x: any) => x.id === id);
        if (b) b.status = 'INACTIVE';
        this.acting = null;
      },
      error: () => {
        this.acting = null;
        this.toast.error('Could not deactivate bus');
      }
    });
  }
}

@Component({
  selector: 'app-admin-routes',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminNavComponent],
  templateUrl: './admin-routes.html',
  styleUrls: ['./admin.css']
})
export class AdminRoutesComponent implements OnInit {
  routes: any[] = [];
  source = '';
  destination = '';
  loading = true;
  adding = false;
  deletingId: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.loadRoutes();
  }

  loadRoutes(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/Admin/routes`).subscribe({
      next: (d) => {
        this.routes = (d ?? []).map((r) => ({
          ...r,
          id: r.id ?? r.Id,
          source: r.source ?? r.Source,
          destination: r.destination ?? r.Destination
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  addRoute(): void {
    if (!this.source || !this.destination) return;
    this.adding = true;
    this.http.post(`${environment.apiUrl}/Admin/add-route`, { source: this.source, destination: this.destination }).subscribe({
      next: () => {
        this.adding = false;
        this.toast.success('Route added');
        this.source = '';
        this.destination = '';
        this.loadRoutes();
      },
      error: () => {
        this.adding = false;
        this.toast.error('Failed to add route');
      }
    });
  }

  async deleteRoute(r: { id: string; source: string; destination: string }): Promise<void> {
    const label = `${r.source} → ${r.destination}`;
    const ok = await this.dialog.confirm({
      title: 'Delete route',
      message: `Delete route "${label}"?\n\nThis cannot be undone if no buses use it.`,
      confirmText: 'Delete route'
    });
    if (!ok) return;
    this.deletingId = r.id;
    this.http.delete<{ message?: string }>(`${environment.apiUrl}/Admin/routes/${r.id}`).subscribe({
      next: () => {
        this.toast.success('Route deleted');
        this.routes = this.routes.filter((x) => x.id !== r.id);
        this.deletingId = null;
      },
      error: (err) => {
        this.deletingId = null;
        const msg = err?.error?.message ?? 'Could not delete route';
        this.toast.error(msg);
      }
    });
  }
}

@Component({
  selector: 'app-admin-route-change-requests',
  standalone: true,
  imports: [CommonModule, AdminNavComponent],
  templateUrl: './admin-route-change-requests.html',
  styleUrls: ['./admin.css']
})
export class AdminRouteChangeRequestsComponent implements OnInit {
  rows: any[] = [];
  loading = true;
  acting: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/Admin/route-change-requests`, { params: { status: 'PENDING' } }).subscribe({
      next: (d) => {
        this.rows = (d ?? []).map((r) => ({
          id: r.id ?? r.Id,
          busNumber: r.busNumber ?? r.BusNumber,
          operatorReason: r.operatorReason ?? r.OperatorReason,
          status: r.status ?? r.Status
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load route change requests');
      }
    });
  }

  approve(id: string): void {
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/route-change-requests/${id}/approve`, {}).subscribe({
      next: () => {
        this.toast.success('Route change approved');
        this.rows = this.rows.filter((r) => r.id !== id);
        this.acting = null;
      },
      error: (err) => {
        this.acting = null;
        this.toast.error(err?.error?.message ?? 'Approve failed');
      }
    });
  }

  async reject(id: string): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Reject route change request',
      message: 'Optionally add a reason for rejection.',
      inputLabel: 'Reason (optional)',
      inputPlaceholder: 'Enter reason',
      confirmText: 'Reject request'
    });
    if (reason === null) return;
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/route-change-requests/${id}/reject`, { reason: reason || null }).subscribe({
      next: () => {
        this.toast.success('Route change rejected');
        this.rows = this.rows.filter((r) => r.id !== id);
        this.acting = null;
      },
      error: (err) => {
        this.acting = null;
        this.toast.error(err?.error?.message ?? 'Reject failed');
      }
    });
  }
}

@Component({
  selector: 'app-admin-reactivation-requests',
  standalone: true,
  imports: [CommonModule, AdminNavComponent],
  templateUrl: './admin-reactivation-requests.html',
  styleUrls: ['./admin.css']
})
export class AdminReactivationRequestsComponent implements OnInit {
  rows: any[] = [];
  loading = true;
  acting: string | null = null;

  constructor(
    private http: HttpClient,
    private toast: ToastService,
    private dialog: DialogService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/Admin/reactivation-requests`, { params: { status: 'PENDING' } }).subscribe({
      next: (d) => {
        this.rows = (d ?? []).map((r) => ({
          id: r.id ?? r.Id,
          busNumber: r.busNumber ?? r.BusNumber,
          operatorReason: r.operatorReason ?? r.OperatorReason,
          status: r.status ?? r.Status
        }));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load reactivation requests');
      }
    });
  }

  approve(id: string): void {
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/reactivation-requests/${id}/approve`, {}).subscribe({
      next: () => {
        this.toast.success('Reactivation approved');
        this.rows = this.rows.filter((r) => r.id !== id);
        this.acting = null;
      },
      error: (err) => {
        this.acting = null;
        this.toast.error(err?.error?.message ?? 'Approve failed');
      }
    });
  }

  async reject(id: string): Promise<void> {
    const reason = await this.dialog.prompt({
      title: 'Reject reactivation request',
      message: 'Optionally add a reason for rejection.',
      inputLabel: 'Reason (optional)',
      inputPlaceholder: 'Enter reason',
      confirmText: 'Reject request'
    });
    if (reason === null) return;
    this.acting = id;
    this.http.put(`${environment.apiUrl}/Admin/reactivation-requests/${id}/reject`, { reason: reason || null }).subscribe({
      next: () => {
        this.toast.success('Reactivation rejected');
        this.rows = this.rows.filter((r) => r.id !== id);
        this.acting = null;
      },
      error: (err) => {
        this.acting = null;
        this.toast.error(err?.error?.message ?? 'Reject failed');
      }
    });
  }
}
