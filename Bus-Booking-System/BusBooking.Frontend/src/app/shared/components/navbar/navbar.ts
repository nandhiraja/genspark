import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class NavbarComponent implements OnInit {
  constructor(public auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) this.auth.refreshProfileFromApi();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }

  roleBadgeClass(): string {
    const r = this.auth.getRole();
    if (r === 'ADMIN') return 'role-badge role-admin';
    if (r === 'OPERATOR') return 'role-badge role-operator';
    return 'role-badge role-user';
  }

  roleLabel(): string {
    const r = this.auth.getRole();
    if (r === 'ADMIN') return 'Admin';
    if (r === 'OPERATOR') return 'Operator';
    if (r === 'USER') return 'Traveller';
    return r || '';
  }
}
