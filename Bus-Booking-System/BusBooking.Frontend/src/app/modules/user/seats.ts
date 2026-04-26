import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';

interface Seat { id: string; seatNumber: number; status: string; }
interface Passenger { name: string; age: number; gender: string; phone: string; }

const CHECKOUT_KEY = 'busBookingCheckout';

@Component({
  selector: 'app-seats',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './seats.html',
  styleUrls: ['./seats.css']
})
export class SeatsComponent implements OnInit, OnDestroy {
  busId = '';
  /** yyyy-mm-dd from search, when provided */
  travelDate = '';
  seats: Seat[] = [];
  layoutSeats: (Seat | null)[] = []; // Null represents a gap/aisle
  selectedSeats: Seat[] = [];
  passengers: Passenger[] = [];
  loading = true;
  locking = false;
  locked = false;
  timer = 300; // 5 minutes
  timerInterval: any;
  step: 'select' | 'details' = 'select';

  constructor(
    private route: ActivatedRoute, public router: Router,
    private http: HttpClient, private toast: ToastService
  ) {
    const nav = this.router.getCurrentNavigation();
    const st = nav?.extras?.state as { travelDate?: string } | undefined;
    if (st?.travelDate) this.travelDate = st.travelDate;
  }

  ngOnInit(): void {
    this.busId = this.route.snapshot.paramMap.get('busId')!;
    const hist = history.state as { travelDate?: string } | undefined;
    if (hist?.travelDate && !this.travelDate) this.travelDate = hist.travelDate;
    this.loadSeats();
  }

  loadSeats(): void {
    this.loading = true;
    this.http.get<Seat[]>(`${environment.apiUrl}/User/bus-seats/${this.busId}`).subscribe({
      next: (data) => { 
        this.seats = data; 
        this.prepareLayout();
        this.loading = false; 
      },
      error: (err) => { 
        this.loading = false; 
        this.toast.error('Failed to load seats. Please check your connection.');
        console.error('Seats Load Error:', err);
      }
    });
  }

  prepareLayout(): void {
    const layout: (Seat | null)[] = [];
    const totalSeats = this.seats.length;
    // We assume 2 + 1 (aisle) + 3 layout = 6 columns
    // Last row is full 6 seats
    const normalRows = Math.floor((totalSeats - 6) / 5);
    
    let seatIdx = 0;
    // Process normal rows
    for (let r = 0; r < normalRows; r++) {
      layout.push(this.seats[seatIdx++]); // Left 1
      layout.push(this.seats[seatIdx++]); // Left 2
      layout.push(null);                  // Aisle
      layout.push(this.seats[seatIdx++]); // Right 1
      layout.push(this.seats[seatIdx++]); // Right 2
      layout.push(this.seats[seatIdx++]); // Right 3
    }
    
    // Process last row (full 6)
    while (seatIdx < totalSeats) {
      layout.push(this.seats[seatIdx++]);
    }
    
    this.layoutSeats = layout;
  }

  toggleSeat(seat: Seat): void {
    if (seat.status !== 'AVAILABLE' && !this.isSelected(seat)) return;
    
    const idx = this.selectedSeats.findIndex(s => s.id === seat.id);
    if (idx > -1) {
      // If already locked, we need to unlock on backend
      if (this.locked) {
        this.unlockSeat(seat.id);
      } else {
        this.selectedSeats.splice(idx, 1);
      }
    } else {
      if (this.locked) return; // Can't add new seats after locking
      this.selectedSeats.push(seat);
    }
  }

  unlockSeat(seatId: string): void {
    this.http.post(`${environment.apiUrl}/Booking/unlock-seats`, {
      busId: this.busId,
      seatIds: [seatId]
    }).subscribe({
      next: () => {
        const idx = this.selectedSeats.findIndex(s => s.id === seatId);
        if (idx > -1) {
          this.selectedSeats.splice(idx, 1);
          this.passengers.splice(idx, 1);
          
          if (this.selectedSeats.length === 0) {
            this.resetSelection();
          }
        }
        this.toast.success('Seat released.');
      },
      error: () => this.toast.error('Failed to release seat.')
    });
  }

  resetSelection(): void {
    clearInterval(this.timerInterval);
    this.locked = false;
    this.step = 'select';
    this.selectedSeats = [];
    this.passengers = [];
    this.timer = 300;
    this.loadSeats();
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
        this.passengers = this.selectedSeats.map(() => ({ name: '', age: 0, gender: 'M', phone: '' }));
        this.startTimer();
        this.toast.success('Seats locked for 5 minutes!');
      },
      error: () => { this.locking = false; this.toast.error('Failed to lock. Seats may be taken.'); }
    });
  }

  startTimer(): void {
    if (this.timerInterval) clearInterval(this.timerInterval);
    this.timerInterval = setInterval(() => {
      this.timer--;
      if (this.timer <= 0) {
        this.resetSelection();
        this.toast.warning('Lock expired. Seats released.');
      }
    }, 1000);
  }

  get timerDisplay(): string {
    const m = Math.floor(this.timer / 60);
    const s = this.timer % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  /** Readable travel day from search (yyyy-mm-dd). */
  get travelDateLabel(): string {
    if (!this.travelDate) return '';
    return new Date(this.travelDate + 'T12:00:00').toLocaleDateString('en-IN', {
      weekday: 'long',
      day: 'numeric',
      month: 'short',
      year: 'numeric'
    });
  }

  proceedToPayment(): void {
    const invalid = this.passengers.some(p => !p.name || p.age <= 0 || !p.phone);
    if (invalid) {
      this.toast.warning('Please fill all passenger details including phone');
      return;
    }
    const payload = {
      busId: this.busId,
      travelDate: this.travelDate,
      seats: this.selectedSeats.map(s => ({ id: s.id, seatNumber: s.seatNumber })),
      passengers: this.passengers.map(p => ({
        name: p.name,
        age: p.age,
        gender: p.gender,
        phone: p.phone
      }))
    };
    sessionStorage.setItem(CHECKOUT_KEY, JSON.stringify(payload));
    this.router.navigate(['/user/payment']);
  }

  ngOnDestroy(): void {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }
}
