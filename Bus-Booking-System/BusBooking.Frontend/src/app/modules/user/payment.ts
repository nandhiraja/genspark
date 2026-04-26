import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../core/environment';
import { ToastService } from '../../core/toast.service';
import { AuthService } from '../../core/auth.service';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

const CHECKOUT_KEY = 'busBookingCheckout';

export interface CheckoutSeat {
  id: string;
  seatNumber: number;
}

export interface CheckoutPassenger {
  name: string;
  age: number;
  gender: string;
  phone: string;
}

export interface CheckoutPayload {
  busId: string;
  travelDate: string;
  seats: CheckoutSeat[];
  passengers: CheckoutPassenger[];
}

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './payment.html',
  styleUrls: ['./payment.css']
})
export class PaymentComponent implements OnInit {
  step: 'review' | 'processing' | 'success' = 'review';
  checkout: CheckoutPayload | null = null;
  bus: {
    busNumber: string;
    source: string;
    destination: string;
    departureTime: string;
    arrivalTime: string;
    operatorName: string;
    sourceBoardingPoint?: { label?: string; addressLine?: string } | null;
    destinationBoardingPoint?: { label?: string; addressLine?: string } | null;
    basePrice: number;
    platformFee: number;
    totalPrice: number;
  } | null = null;

  loading = true;
  bookingId = '';
  payError = false;

  constructor(
    private http: HttpClient,
    private router: Router,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit(): void {
    this.checkout = this.readCheckout();
    if (!this.checkout?.busId || !this.checkout.seats?.length) {
      this.toast.warning('No booking in progress. Select seats again.');
      this.router.navigate(['/user/search']);
      return;
    }
    this.http.get<any>(`${environment.apiUrl}/User/bus/${this.checkout.busId}`).subscribe({
      next: (data) => {
        this.bus = {
          busNumber: data.busNumber,
          source: data.source,
          destination: data.destination,
          departureTime: data.departureTime,
          arrivalTime: data.arrivalTime,
          operatorName: data.operatorName,
          sourceBoardingPoint: data.sourceBoardingPoint ?? null,
          destinationBoardingPoint: data.destinationBoardingPoint ?? null,
          basePrice: data.basePrice,
          platformFee: data.platformFee,
          totalPrice: data.totalPrice
        };
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Could not load trip details.');
        this.router.navigate(['/user/search']);
      }
    });
  }

  get seatCount(): number {
    return this.checkout?.seats.length ?? 0;
  }

  get subtotal(): number {
    if (!this.bus) return 0;
    return Math.round(this.bus.basePrice * this.seatCount * 100) / 100;
  }

  get fees(): number {
    if (!this.bus) return 0;
    return Math.round(this.bus.platformFee * this.seatCount * 100) / 100;
  }

  get grandTotal(): number {
    if (!this.bus) return 0;
    return Math.round(this.bus.totalPrice * this.seatCount * 100) / 100;
  }

  formatMoney(n: number): string {
    return n.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  formatWhen(iso: string): string {
    return new Date(iso).toLocaleString('en-IN', {
      weekday: 'short',
      day: 'numeric',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  travelDateLabel(): string {
    if (this.checkout?.travelDate) {
      const d = new Date(this.checkout.travelDate + 'T12:00:00');
      return d.toLocaleDateString('en-IN', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
    }
    if (this.bus?.departureTime) {
      return new Date(this.bus.departureTime).toLocaleDateString('en-IN', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric'
      });
    }
    return '—';
  }

  proceedToPay(): void {
    if (!this.checkout || !this.bus) return;
    this.payError = false;
    this.step = 'processing';

    window.setTimeout(() => {
      this.http
        .post<{ bookingId: string }>(`${environment.apiUrl}/Booking/confirm`, {
          busId: this.checkout!.busId,
          seatIds: this.checkout!.seats.map((s) => s.id),
          passengers: this.checkout!.passengers
        })
        .subscribe({
          next: (res) => {
            this.bookingId = res.bookingId;
            sessionStorage.removeItem(CHECKOUT_KEY);
            this.step = 'success';
            this.toast.success('Payment successful. Confirmation sent to your email.');
          },
          error: () => {
            this.step = 'review';
            this.payError = true;
            this.toast.error('Payment could not be completed. Please try again.');
          }
        });
    }, 2200);
  }

  downloadBill(): void {
    if (!this.checkout || !this.bus) return;
    const email = this.auth.getEmail() ?? '—';
    const doc = new jsPDF({ unit: 'pt', format: 'a4' });
    const pageWidth = doc.internal.pageSize.getWidth();
    const pageHeight = doc.internal.pageSize.getHeight();
    const margin = 40;
    const cardWidth = pageWidth - margin * 2;
    const routeText = `${this.bus.source} to ${this.bus.destination}`;
    const sourcePoint = this.bus.sourceBoardingPoint?.label
      ? `${this.bus.sourceBoardingPoint.label}${this.bus.sourceBoardingPoint.addressLine ? ' - ' + this.bus.sourceBoardingPoint.addressLine : ''}`
      : 'Main pickup point';
    const destinationPoint = this.bus.destinationBoardingPoint?.label
      ? `${this.bus.destinationBoardingPoint.label}${this.bus.destinationBoardingPoint.addressLine ? ' - ' + this.bus.destinationBoardingPoint.addressLine : ''}`
      : 'Main drop point';
    const money = (v: number) => `INR ${this.formatMoney(v)}`;

    // Header band
    doc.setFillColor(79, 70, 229);
    doc.rect(0, 0, pageWidth, 92, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(21);
    doc.text('GO-BUS', margin, 44);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10.5);
    doc.text('E-Ticket Invoice', margin, 64);

    doc.setFontSize(9.5);
    doc.text(`Invoice Date: ${new Date().toLocaleString('en-IN')}`, pageWidth - margin, 44, { align: 'right' });
    doc.text(`Booking Ref: ${(this.bookingId || 'booking').slice(0, 8).toUpperCase()}`, pageWidth - margin, 60, { align: 'right' });

    // Main details
    doc.setTextColor(15, 23, 42);
    let y = 116;
    doc.setDrawColor(226, 232, 240);
    doc.setFillColor(255, 255, 255);
    doc.roundedRect(margin, y - 10, cardWidth, 122, 10, 10, 'FD');

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(12);
    doc.text('Trip Summary', margin, y);
    y += 20;
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.text(`Operator: ${this.bus.operatorName}`, margin, y);
    doc.text(`Bus: ${this.bus.busNumber}`, margin + cardWidth / 2, y);
    y += 16;
    doc.text(`Route: ${routeText}`, margin, y);
    y += 16;
    doc.text(`Source Boarding: ${sourcePoint}`, margin, y, { maxWidth: cardWidth / 2 - 20 });
    doc.text(`Destination Drop: ${destinationPoint}`, margin + cardWidth / 2, y, { maxWidth: cardWidth / 2 - 20 });
    y += 16;
    doc.text(`Travel Date: ${this.travelDateLabel()}`, margin, y);
    doc.text(`Contact Email: ${email}`, margin + cardWidth / 2, y);
    y += 16;
    doc.text(`Departure: ${this.formatWhen(this.bus.departureTime)}`, margin, y);
    doc.text(`Arrival: ${this.formatWhen(this.bus.arrivalTime)}`, margin + cardWidth / 2, y);

    // Passenger table
    autoTable(doc, {
      startY: y + 26,
      head: [['Seat', 'Passenger Name', 'Age', 'Gender', 'Phone']],
      body: this.checkout.seats.map((s, i) => {
        const p = this.checkout!.passengers[i];
        return [String(s.seatNumber), p.name, String(p.age), p.gender, p.phone];
      }),
      headStyles: { fillColor: [99, 102, 241], textColor: 255 },
      styles: { fontSize: 10, cellPadding: 7, lineColor: [226, 232, 240], lineWidth: 0.6 },
      bodyStyles: { textColor: [15, 23, 42] }
    });

    const afterPax = (doc as any).lastAutoTable?.finalY ?? y + 40;

    // Fare table
    autoTable(doc, {
      startY: afterPax + 18,
      head: [['Fare Breakdown', 'Amount (INR)']],
      body: [
        ['Base fare', money(this.subtotal)],
        ['Platform fee (5%)', money(this.fees)],
        ['Seats', String(this.seatCount)],
        ['Total paid', money(this.grandTotal)]
      ],
      headStyles: { fillColor: [15, 23, 42], textColor: 255 },
      styles: { fontSize: 10, cellPadding: 7, lineColor: [226, 232, 240], lineWidth: 0.6 },
      columnStyles: { 1: { halign: 'right' } },
      didParseCell: (data) => {
        if (data.row.index === 3) {
          data.cell.styles.fontStyle = 'bold';
          data.cell.styles.fillColor = [238, 242, 255];
        }
      }
    });

    const afterFare = (doc as any).lastAutoTable?.finalY ?? afterPax + 80;
    doc.setFontSize(9.5);
    doc.setTextColor(100, 116, 139);
    const footerY = Math.min(afterFare + 22, pageHeight - 42);
    doc.text('This is a system-generated invoice and does not require a signature.', margin, footerY);
    doc.text('Thank you for choosing GO-BUS. Have a safe journey!', margin, footerY + 16);

    doc.save(`go-bus-invoice-${(this.bookingId || 'booking').slice(0, 8)}.pdf`);
  }

  goDashboard(): void {
    this.router.navigate(['/user/dashboard']);
  }

  goSearch(): void {
    this.router.navigate(['/user/search']);
  }

  private readCheckout(): CheckoutPayload | null {
    try {
      const raw = sessionStorage.getItem(CHECKOUT_KEY);
      if (!raw) return null;
      return JSON.parse(raw) as CheckoutPayload;
    } catch {
      return null;
    }
  }
}
