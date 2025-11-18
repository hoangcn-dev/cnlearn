import { Injectable, inject, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable, BehaviorSubject, tap, catchError, throwError, map, switchMap, finalize } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { ApiResponse } from '../../../core/models/api-response.model';
import { LoginInfo, LoginRequest } from '../models/user.model';
import { HttpParams } from '@angular/common/http';
import { LoadingService } from '../../../core/services/loading.service';

const USER_KEY = 'current_user';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private api = inject(ApiService);
    private platformId = inject(PLATFORM_ID);
    private isBrowser = isPlatformBrowser(this.platformId);
    private loadingService = inject(LoadingService);

    private currentUserSubject = new BehaviorSubject<LoginInfo | null>(this.getUserFromStorage());
    public currentUser$ = this.currentUserSubject.asObservable();
    
    private isAuthenticatedSubject = new BehaviorSubject<boolean>(!!this.getUserFromStorage());
    public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

    public isLoading = signal(false);

    login(request: LoginRequest): Observable<LoginInfo> {
        this.isLoading.set(true);
        this.loadingService.show('Đang đăng nhập...');
        
        return this.api.post<ApiResponse<void>>('users/login', request).pipe(
            map(response => {
                return response;
            }),
            switchMap(() => {
                this.loadingService.update('Đang tải thông tin...');
                return this.getLoginInfo();
            }),
            tap(loginInfo => {
                this.setUser(loginInfo);
                this.currentUserSubject.next(loginInfo);
                this.isAuthenticatedSubject.next(true);
            }),
            finalize(() => {
                this.isLoading.set(false);
                this.loadingService.hide();
            })
        );
    }

    getLoginInfo(): Observable<LoginInfo> {
        return this.api.get<ApiResponse<LoginInfo>>('users/login-info').pipe(
            map(response => {
                if (response.errorMessage || !response.data) {
                    throw new Error(response.errorMessage || 'Không thể lấy thông tin user');
                }
                return response.data;
            }),
            tap(loginInfo => {
                this.setUser(loginInfo);
                this.currentUserSubject.next(loginInfo);
                this.isAuthenticatedSubject.next(true);
            }),
            catchError(error => {
                if (error.status === 401) {
                    this.clearStorage();
                    this.currentUserSubject.next(null);
                    this.isAuthenticatedSubject.next(false);
                }
                return throwError(() => error);
            })
        );
    }

    loginWithGoogle(returnUrl: string): void {
        if (!this.isBrowser) return;
        const fullReturnUrl = window.location.origin + returnUrl;
        const url = `${this.api.apiUrl}/users/google-login?returnUrl=${encodeURIComponent(fullReturnUrl)}`;
        window.location.href = url;
    }

    logout(): Observable<void> {
        this.loadingService.show('Đang đăng xuất...');
        
        return this.api.post<ApiResponse<void>>('users/logout', {}).pipe(
            tap(() => {
                this.clearStorage();
                this.currentUserSubject.next(null);
                this.isAuthenticatedSubject.next(false);
            }),
            map(() => void 0),
            catchError(error => {
                this.clearStorage();
                this.currentUserSubject.next(null);
                this.isAuthenticatedSubject.next(false);
                return throwError(() => error);
            }),
            finalize(() => this.loadingService.hide())
        );
    }

    isAuthenticated(): boolean {
        return !!this.getUserFromStorage();
    }

    getCurrentUser(): LoginInfo | null {
        const user = this.currentUserSubject.value;
        return user;
    }

    private setUser(user: LoginInfo): void {
        if (this.isBrowser) {
            sessionStorage.setItem(USER_KEY, JSON.stringify(user));
        }
    }

    private getUserFromStorage(): LoginInfo | null {
        if (!this.isBrowser) return null;
        const userStr = sessionStorage.getItem(USER_KEY);
        return userStr ? JSON.parse(userStr) : null;
    }

    private clearStorage(): void {
        if (!this.isBrowser) return;
        sessionStorage.removeItem(USER_KEY);
    }
}
