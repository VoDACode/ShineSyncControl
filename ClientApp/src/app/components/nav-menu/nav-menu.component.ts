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
  siteLanguage = 'English';
  languageList = [
    { code: 'en', label: 'English' },
    { code: 'uk', label: 'Ukrainian' },
  ];

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
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  changeSiteLanguage(localeCode: string): void {
    const selectedLanguage = this.languageList
      .find((language) => language.code === localeCode)
      ?.label.toString();
    if (selectedLanguage) {
      this.siteLanguage = selectedLanguage;
      this.translate.use(localeCode);
    }
    const currentLanguage = this.translate.currentLang;
    console.log('currentLanguage', currentLanguage);
  }
}
