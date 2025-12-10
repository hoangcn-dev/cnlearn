import { inject, Injectable } from '@angular/core';
import { Router, Routes } from '@angular/router';
import { ToastService } from './toast.service';

@Injectable({
    providedIn: 'root',
})
export class UtilService {
    router = inject(Router);
    toastService = inject(ToastService);

    toQueryString(obj: any): string {
        return Object.keys(obj)
            .filter(key => obj[key] !== undefined && obj[key] !== null)
            .map(key => `${encodeURIComponent(key)}=${encodeURIComponent(obj[key])}`)
            .join("&");
    }
}
