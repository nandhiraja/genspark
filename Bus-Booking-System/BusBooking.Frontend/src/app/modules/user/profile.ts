import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { AuthService } from '../../core/auth.service';
import { ToastService } from '../../core/toast.service';

@Component({ selector: 'app-profile', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './profile.html', styleUrls: ['./dashboard.css'] })
export class ProfileComponent implements OnInit {
  name = ''; phone = ''; loading = false;
  constructor(private http: HttpClient, private auth: AuthService, private toast: ToastService) {}
  ngOnInit(): void {}
  save(): void {
    this.loading = true;
    this.http.put(`${environment.apiUrl}/User/profile`, { name: this.name, phone: this.phone }).subscribe({
      next: () => { this.loading = false; this.toast.success('Profile updated'); },
      error: () => { this.loading = false; this.toast.error('Failed to update'); }
    });
  }
}
