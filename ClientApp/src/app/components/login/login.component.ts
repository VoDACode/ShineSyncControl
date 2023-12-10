import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthApiService } from 'src/app/services/auth-api.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public email: string = "";
  public password: string = "";

  constructor(private authApiService: AuthApiService, private router: Router) {
    this.authApiService.check().subscribe(res => {
      if (res) {
        this.router.navigate(['/home']);
      }
    });
  }

  public login() {
    this.authApiService.login(this.email, this.password).subscribe(res => {
      if (res.success) {
        this.router.navigate(['/home']);
      } else {
        alert(res.message);
      }
    });
  }
}
