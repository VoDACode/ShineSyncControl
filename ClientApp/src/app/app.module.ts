import { BrowserModule } from '@angular/platform-browser';
import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { DeviceItemComponent } from './components/device-item/device-item.component';
import { DevicePageComponent } from './components/device-page/device-page.component';
import { SwitchComponent } from './components/switch/switch.component';
import { AuthGuardService } from './auth.guard';

// export function HttpLoaderFactory(http: HttpClient) {
//   return new TranslateHttpLoader(http,
//     './assets/i18n/',
//     '.json');
// }

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    LoginComponent,
    RegisterComponent,
    HomePageComponent,
    DeviceItemComponent, DevicePageComponent,
    SwitchComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: LoginComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'home', component: HomePageComponent, canActivate: [AuthGuardService] },
      { path: 'device/:id', component: DevicePageComponent, canActivate: [AuthGuardService] },
    ])
  ],
  providers: [AuthGuardService, { provide: LOCALE_ID, useValue: 'uk-UA' }],
  bootstrap: [AppComponent]
})
export class AppModule { }
