import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { UserModel } from 'src/app/models/user.model';
import { AuthApiService } from 'src/app/services/auth-api.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  languageList = [
    { code: 'en', label: '🇺🇸 English' },
    { code: 'uk', label: '🇺🇦 Українська' },
  ];

  public get currentLanguage(): string {
    return this.translate.currentLang;
  }

  isExpanded = false;

  isAuthorized = false;

  public user: UserModel = new UserModel();

  public get canRegisterNewDevice(): boolean {
    return this.user?.role === 'Admin' || this.user?.role === 'Registrar';
  }

  constructor(private translate: TranslateService, private authApi: AuthApiService, private userApiService: UserApiService) {
    this.authApi.check().subscribe(res => {
      if (res) {
        this.isAuthorized = true;
      }
    });
    this.userApiService.getSelf().subscribe(res => {
      if(res.data == null) {
        return;
      }
      this.user = res.data;
    });

    const language = localStorage.getItem('language');
    if (language) {
      this.translate.use(language);
    }
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  changeLanguage(language: string): void {
    this.translate.use(language);
    localStorage.setItem('language', language);
  }
}
