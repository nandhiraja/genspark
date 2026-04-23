import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;

  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}

  onSubmit(): void {
    if (!this.email || !this.password) { this.toast.warning('Please fill all fields'); return; }
    this.loading = true;
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.toast.success('Welcome back!');
        const role = this.auth.getRole();
        if (role === 'ADMIN') this.router.navigate(['/admin/dashboard']);
        else if (role === 'OPERATOR') this.router.navigate(['/operator/dashboard']);
        else this.router.navigate(['/user/search']);
      },
      error: () => { this.loading = false; this.toast.error('Invalid email or password'); }
    });
  }
}
