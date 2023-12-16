import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUserModel } from '../models/register.user.model';
import { BaseResponse } from '../models/base.response';
import { BaseApiService } from './base.api.service';
import { Observable, catchError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService extends BaseApiService {

  public get isAuthenticated(): boolean {
    return localStorage.getItem('isLoggedin') == 'true';
  }

  constructor(private http: HttpClient, router: Router) {
    super(router);
  }

  public login(email: string, password: string) {
    return this.http.post<BaseResponse<any>>('/api/auth/login', {
      email,
      password
    }).pipe(catchError((e) => this.handleError<any>(e)));
  }

  public register(user: RegisterUserModel) {
    return this.http.post<BaseResponse<any>>('/api/auth/register', user)
      .pipe(catchError((e) => this.handleError<any>(e)));
  }

  public logout() {
    return this.http.get<BaseResponse<any>>('/api/auth/logout')
      .pipe(catchError((e) => this.handleError<any>(e)));
  }

  public check(): Observable<boolean> {
    var isLoggedin = localStorage.getItem('isLoggedin');
    if (isLoggedin) {
      return new Observable<boolean>(observer => {
        observer.next(true);
      });
    }

    var observer = new Observable<boolean>(sub => {
      var request = this.http.get<BaseResponse<any>>('/api/auth/check');
      request.subscribe(res => {
        if (res.success) {
          localStorage.setItem('isLoggedin', 'true');
          sub.next(true);
        } else {
          localStorage.removeItem('isLoggedin');
          sub.next(false);
        }
      });
    });
    return observer;
  }
}
