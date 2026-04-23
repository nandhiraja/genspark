import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search.html',
  styleUrls: ['./search.css']
})
export class SearchComponent {
  source = '';
  destination = '';
  date = '';
  buses: any[] = [];
  loading = false;
  searched = false;

  constructor(private http: HttpClient, private router: Router) {}

  search(): void {
    if (!this.source || !this.destination || !this.date) return;
    this.loading = true;
    this.searched = true;
    this.http.get<any[]>(`${environment.apiUrl}/User/search-buses`, {
      params: { source: this.source, destination: this.destination, date: this.date }
    }).subscribe({
      next: (data) => { this.buses = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  selectBus(busId: string): void {
    this.router.navigate(['/user/seats', busId]);
  }

  formatTime(dt: string): string {
    return new Date(dt).toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit' });
  }

  getDuration(start: string, end: string): string {
    const diff = new Date(end).getTime() - new Date(start).getTime();
    const h = Math.floor(diff / 3600000);
    const m = Math.floor((diff % 3600000) / 60000);
    return `${h}h ${m}m`;
  }
}
