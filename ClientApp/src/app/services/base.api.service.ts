import { HttpErrorResponse } from "@angular/common/http";
import { Router } from "@angular/router";

export abstract class BaseApiService {

    constructor(protected router: Router) { }

    protected handleError(error: HttpErrorResponse)
    {
        if (error.status === 0) {
            console.error('An error occurred:', error.error);
        } else if (error.status === 400) {
            return [];
        } else if (error.status === 401) {
            localStorage.removeItem('isLoggedin');
            this.router.navigate(['/login']);
            return [];
        }
        else {
            console.error(
                `Backend returned code ${error.status}, ` +
                `body was: ${error.error}`);
        }
        return [];
    }
}