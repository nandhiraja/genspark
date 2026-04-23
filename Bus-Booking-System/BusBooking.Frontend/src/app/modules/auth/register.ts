import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { ToastService } from '../../core/toast.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./login.css']
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  loading = false;

  constructor(private auth: AuthService, private router: Router, private toast: ToastService) {}

  onSubmit(): void {
    if (!this.name || !this.email || !this.password) { this.toast.warning('Please fill all fields'); return; }
    this.loading = true;
    this.auth.register({ name: this.name, email: this.email, password: this.password }).subscribe({
      next: () => {
        this.toast.success('Account created! Please log in.');
        this.router.navigate(['/auth/login']);
      },
      error: () => { this.loading = false; this.toast.error('Registration failed. Email may already exist.'); }
    });
  }
}
