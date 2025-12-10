import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UUID } from 'crypto';
import { ApiResponse } from '../models/api-response.model';

export const endpoints = {
  auth: {
    login: 'users/login',
    logout: 'users/logout',
    loginInfo: 'users/me',
    loginWithGoogle: (callbackUrl: string) => `${environment.apiUrl}/users/google-login?returnUrl=${encodeURIComponent(callbackUrl)}`,
  },
  manageUser: {
    getAll: (query: string) => `users?${query}`,
    getDetail: (id: UUID) => `users/${id}/detail`,
    update: (id: UUID) => `users/${id}`,
    delete: (id: UUID) => `users/${id}`,
    logs: (query: string) => `logs/users?${query}`
  }
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  public apiUrl = environment.apiUrl;

  get<T=void>(endpoint: string, params?: HttpParams) {
    return this.http.get<ApiResponse<T>>(`${this.apiUrl}/${endpoint}`, { 
      params,
      withCredentials: true 
    });
  }

  post<T=void>(endpoint: string, body: any) {
    return this.http.post<ApiResponse<T>>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }

  put<T=void>(endpoint: string, body: any) {
    return this.http.put<ApiResponse<T>>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }

  delete<T=void>(endpoint: string) {
    return this.http.delete<ApiResponse<T>>(`${this.apiUrl}/${endpoint}`, { 
      withCredentials: true 
    });
  }

  patch<T=void>(endpoint: string, body: any) {
    return this.http.patch<ApiResponse<T>>(`${this.apiUrl}/${endpoint}`, body, { 
      withCredentials: true 
    });
  }
}
