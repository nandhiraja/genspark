import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

interface Seat { id: string; seatNumber: number; status: string; }
interface Passenger { name: string; age: number; gender: string; }

@Component({
  selector: 'app-seats',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './seats.html',
  styleUrls: ['./seats.css']
})
export class SeatsComponent implements OnInit, OnDestroy {
  busId = '';
  seats: Seat[] = [];
  selectedSeats: Seat[] = [];
  passengers: Passenger[] = [];
  loading = true;
  locking = false;
  confirming = false;
  locked = false;
  timer = 300; // 5 minutes
  timerInterval: any;
  step: 'select' | 'details' = 'select';

  constructor(
    private route: ActivatedRoute, private router: Router,
    private http: HttpClient, private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.busId = this.route.snapshot.paramMap.get('busId')!;
    this.loadSeats();
  }

  loadSeats(): void {
    this.loading = true;
    this.http.get<Seat[]>(`${environment.apiUrl}/User/bus-seats/${this.busId}`).subscribe({
      next: (data) => { this.seats = data; this.loading = false; },
      error: (err) => { 
        this.loading = false; 
        this.toast.error('Failed to load seats. Please check your connection.');
        console.error('Seats Load Error:', err);
      }
    });
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 'AVAILABLE') return;
    const idx = this.selectedSeats.findIndex(s => s.id === seat.id);
    if (idx > -1) { this.selectedSeats.splice(idx, 1); }
    else { this.selectedSeats.push(seat); }
  }

  isSelected(seat: Seat): boolean {
    return this.selectedSeats.some(s => s.id === seat.id);
  }

  lockSeats(): void {
    if (this.selectedSeats.length === 0) { this.toast.warning('Select at least one seat'); return; }
    this.locking = true;
    this.http.post(`${environment.apiUrl}/Booking/lock-seats`, {
      busId: this.busId,
      seatIds: this.selectedSeats.map(s => s.id)
    }).subscribe({
      next: () => {
        this.locking = false;
        this.locked = true;
        this.step = 'details';
        this.passengers = this.selectedSeats.map(() => ({ name: '', age: 0, gender: 'M' }));
        this.startTimer();
        this.toast.success('Seats locked for 5 minutes!');
      },
      error: () => { this.locking = false; this.toast.error('Failed to lock. Seats may be taken.'); }
    });
  }

  startTimer(): void {
    this.timerInterval = setInterval(() => {
      this.timer--;
      if (this.timer <= 0) {
        clearInterval(this.timerInterval);
        this.toast.warning('Lock expired. Seats released.');
        this.locked = false;
        this.step = 'select';
        this.selectedSeats = [];
        this.loadSeats();
      }
    }, 1000);
  }

  get timerDisplay(): string {
    const m = Math.floor(this.timer / 60);
    const s = this.timer % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  confirmBooking(): void {
    const invalid = this.passengers.some(p => !p.name || p.age <= 0);
    if (invalid) { this.toast.warning('Please fill all passenger details'); return; }
    this.confirming = true;
    this.http.post(`${environment.apiUrl}/Booking/confirm`, {
      busId: this.busId,
      seatIds: this.selectedSeats.map(s => s.id),
      passengers: this.passengers
    }).subscribe({
      next: () => {
        clearInterval(this.timerInterval);
        this.toast.success('Booking confirmed! 🎉');
        this.router.navigate(['/user/dashboard']);
      },
      error: () => { this.confirming = false; this.toast.error('Booking failed. Please try again.'); }
    });
  }

  ngOnDestroy(): void {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }
}
