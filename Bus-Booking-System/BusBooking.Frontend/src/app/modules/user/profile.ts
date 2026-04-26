import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { AuthService } from '../../core/auth.service';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class ProfileComponent implements OnInit {
  name = '';
  phone = '';
  email = '';
  role = '';
  loading = false;
  pageLoading = true;

  constructor(
    private http: HttpClient,
    public auth: AuthService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.pageLoading = true;
    this.http.get<{ name?: string; email?: string; phone?: string; role?: string }>(`${environment.apiUrl}/Auth/me`).subscribe({
      next: (p) => {
        this.name = p.name ?? '';
        this.phone = p.phone ?? '';
        this.email = p.email ?? '';
        this.role = p.role ?? '';
        this.pageLoading = false;
      },
      error: () => {
        this.pageLoading = false;
        this.toast.error('Could not load your profile');
      }
    });
  }

  save(): void {
    this.loading = true;
    this.http.put(`${environment.apiUrl}/User/profile`, { name: this.name, phone: this.phone }).subscribe({
      next: () => {
        this.loading = false;
        this.toast.success('Profile updated');
        this.auth.refreshProfileFromApi();
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to update');
      }
    });
  }
}
