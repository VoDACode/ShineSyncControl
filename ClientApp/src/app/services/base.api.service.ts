import { HttpErrorResponse } from "@angular/common/http";
import { Router } from "@angular/router";
import { of } from "rxjs";
import { BaseResponse } from "../models/base.response";

export abstract class BaseApiService {

    constructor(protected router: Router) { }

    protected handleError<T>(error: HttpErrorResponse)
    {
        if (error.status === 0) {
            console.error('An error occurred:', error.error);
        } else if (error.status === 401) {
            localStorage.removeItem('isLoggedin');
            this.router.navigate(['/login']);
        }
        else {
            console.error(
                `Backend returned code ${error.status}, ` +
                `body was: ${JSON.stringify(error.error)}`);
        }
        return of(error.error as BaseResponse<T>);
    }
}