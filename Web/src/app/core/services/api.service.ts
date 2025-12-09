import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  public apiUrl = environment.apiUrl;

  get<T>(endpoint: string, params?: HttpParams) {
    return this.http.get<T>(`${this.apiUrl}/${endpoint}`, { 
      params,
      withCredentials: true 
    });
  }

  post<T>(endpoint: string, body: any) {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }

  put<T>(endpoint: string, body: any) {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }

  delete<T>(endpoint: string) {
    return this.http.delete<T>(`${this.apiUrl}/${endpoint}`, { 
      withCredentials: true 
    });
  }

  patch<T>(endpoint: string, body: any) {
    return this.http.patch<T>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }
}
