import { Component, OnInit } from '@angular/core';
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
export class SearchComponent implements OnInit {
  source = '';
  destination = '';
  date = '';
  buses: any[] = [];
  loading = false;
  searched = false;

  // Autocomplete data
  allSources: string[] = [];
  allDestinations: string[] = [];
  filteredSources: string[] = [];
  filteredDestinations: string[] = [];
  showSourceDropdown = false;
  showDestDropdown = false;
  minDate = '';

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];
    this.fetchLocations();
    this.loadDefaultBuses();
  }

  loadDefaultBuses(): void {
    this.loading = true;
    this.http.get<any[]>(`${environment.apiUrl}/User/search-buses`).subscribe({
      next: (data) => { this.buses = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  fetchLocations(): void {
    this.http.get<{ sources: string[], destinations: string[] }>(`${environment.apiUrl}/User/locations`)
      .subscribe({
        next: (data) => {
          this.allSources = data.sources;
          this.allDestinations = data.destinations;
        }
      });
  }

  onSourceInput(): void {
    this.filteredSources = this.allSources.filter(s => 
      s.toLowerCase().includes(this.source.toLowerCase())
    );
    this.showSourceDropdown = this.filteredSources.length > 0;
  }

  onDestInput(): void {
    this.filteredDestinations = this.allDestinations.filter(d => 
      d.toLowerCase().includes(this.destination.toLowerCase())
    );
    this.showDestDropdown = this.filteredDestinations.length > 0;
  }

  selectSource(city: string): void {
    this.source = city;
    this.showSourceDropdown = false;
  }

  selectDestination(city: string): void {
    this.destination = city;
    this.showDestDropdown = false;
  }

  closeDropdowns(): void {
    this.showSourceDropdown = false;
    this.showDestDropdown = false;
  }

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
    this.router.navigate(['/user/seats', busId], {
      state: { travelDate: this.date || '' }
    });
  }

  formatTime(dt: string): string {
    return new Date(dt).toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit' });
  }

  /** Day of departure for the trip (from scheduled start). */
  formatDepartureDay(iso: string): string {
    return new Date(iso).toLocaleDateString('en-IN', {
      weekday: 'long',
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }

  getDuration(start: string, end: string): string {
    const diff = new Date(end).getTime() - new Date(start).getTime();
    const h = Math.floor(diff / 3600000);
    const m = Math.floor((diff % 3600000) / 60000);
    return `${h}h ${m}m`;
  }
}
