import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-appointment-calendar',
  template: `
  <section>
    <h2>Appointment Calendar</h2>
    <div *ngIf="loading">Loading...</div>
    <ul>
      <li *ngFor="let item of items">{{ item.startAtUtc | date:'short' }} - {{ item.status }}</li>
    </ul>
  </section>`
})
export class AppointmentCalendarComponent implements OnInit {
  items: any[] = [];
  loading = false;

  constructor(private readonly http: HttpClient) {}

  ngOnInit(): void {
    this.loading = true;
    const now = new Date();
    const params = new HttpParams().set('year', now.getFullYear()).set('month', now.getMonth() + 1);
    this.http.get<any[]>('/api/appointments/calendar', { params }).subscribe({
      next: (res) => { this.items = res; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
