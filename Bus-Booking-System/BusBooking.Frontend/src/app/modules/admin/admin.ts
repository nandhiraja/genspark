import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.html',
  styleUrls: ['./admin.css']
})
export class AdminDashboardComponent implements OnInit {
  stats: any = {};
  loading = true;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.http.get(`${environment.apiUrl}/Admin/dashboard`).subscribe({
      next: d => {
        console.log('DEBUG: Dashboard Stats Received', d); // Added Debugger
        this.stats = d;
        this.loading = false;
      },
      error: (err) => {
        console.error('DEBUG: Dashboard Stats Error', err);
        this.loading = false;
      }
    });
  }
}

@Component({
  selector: 'app-admin-operators',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-operators.html',
  styleUrls: ['./admin.css']
})
export class AdminOperatorsComponent implements OnInit {
  operators: any[] = [];
  loading = true;

  constructor(private http: HttpClient, private toast: ToastService) { }

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/operators`).subscribe({
      next: d => {
        console.log('DEBUG: Operators List Received', d); // Added Debugger
        this.operators = d;
        this.loading = false;
      },
      error: (err) => {
        console.error('DEBUG: Fetch Operators Error', err);
        this.loading = false;
        this.toast.error('Failed to fetch operator applications');
      }
    });
  }

  approve(id: string): void {
    this.http.post(`${environment.apiUrl}/Admin/approve-operator/${id}`, {}).subscribe({
      next: (res) => {
        console.log(`DEBUG: Operator ${id} Approved`, res); // Added Debugger
        this.toast.success('Operator approved!');
        const op = this.operators.find(o => o.id === id);
        if (op) op.status = 'APPROVED';
      },
      error: (err) => {
        console.error(`DEBUG: Approve Operator ${id} Error`, err);
        this.toast.error('Failed to approve');
      }
    });
  }
}

@Component({
  selector: 'app-admin-routes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-routes.html',
  styleUrls: ['./admin.css']
})
export class AdminRoutesComponent implements OnInit {
  routes: any[] = [];
  source = '';
  destination = '';
  loading = true;
  adding = false;

  constructor(private http: HttpClient, private toast: ToastService) { }

  ngOnInit(): void {
    this.loadRoutes();
  }

  loadRoutes(): void {
    this.http.get<any[]>(`${environment.apiUrl}/Admin/routes`).subscribe({
      next: d => {
        console.log('DEBUG: Routes Loaded', d); // Added Debugger
        this.routes = d;
        this.loading = false;
      },
      error: (err) => {
        console.error('DEBUG: Load Routes Error', err);
        this.loading = false;
      }
    });
  }

  addRoute(): void {
    if (!this.source || !this.destination) return;
    this.adding = true;
    this.http.post(`${environment.apiUrl}/Admin/add-route`, {
      source: this.source,
      destination: this.destination
    }).subscribe({
      next: (res) => {
        console.log('DEBUG: Route Added Successfully', res); // Added Debugger
        this.adding = false;
        this.toast.success('Route added!');
        this.source = '';
        this.destination = '';
        this.loadRoutes();
      },
      error: (err) => {
        console.error('DEBUG: Add Route Error', err);
        this.adding = false;
        this.toast.error('Failed to add route');
      }
    });
  }
}