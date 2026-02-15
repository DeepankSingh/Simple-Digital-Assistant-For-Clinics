import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface PatientDto {
  id: string;
  fullName: string;
  phone: string;
  age: number;
  gender: string;
  address: string;
  medicalHistoryNotes?: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly baseUrl = '/api/patients';

  constructor(private readonly http: HttpClient) {}

  search(q = '', page = 1, pageSize = 20): Observable<{ items: PatientDto[]; total: number }> {
    const params = new HttpParams().set('q', q).set('page', page).set('pageSize', pageSize);
    return this.http.get<{ items: PatientDto[]; total: number }>(this.baseUrl, { params });
  }

  create(payload: Omit<PatientDto, 'id' | 'createdAt'>) {
    return this.http.post<PatientDto>(this.baseUrl, payload);
  }
}
