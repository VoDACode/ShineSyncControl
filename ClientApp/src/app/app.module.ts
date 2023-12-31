import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { DeviceItemComponent } from './components/device-item/device-item.component';
import { DevicePageComponent } from './components/device-page/device-page.component';
import { SwitchComponent } from './components/switch/switch.component';
import { AuthGuard } from './auth.guard';
import { NgxTranslateModule } from './translate/translate.module';
import { ActionPageComponent } from './components/action-page/action-page.component';
import { EditPropertyComponent } from './components/edit-property/edit-property.component';
import { ExpressionComponent } from './components/expression/expression.component';
import { SaveChangesModelComponent } from './components/save-changes-model/save-changes-model.component';
import { TaskPageComponent } from './components/task-page/task-page.component';
import { DeviceAddPageComponent } from './components/device-add-page/device-add-page.component';
import { DeviceRegisterPageComponent } from './components/device-register-page/device-register-page.component';
import { DeviceGroupsPageComponent } from './components/device-groups-page/device-groups-page.component';
import { ActionListPageComponent } from './components/action-list-page/action-list-page.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    LoginComponent,
    RegisterComponent,
    HomePageComponent,
    DeviceItemComponent, DevicePageComponent,
    SwitchComponent,
    ActionPageComponent,
    ActionListPageComponent,
    EditPropertyComponent,
    ExpressionComponent,
    SaveChangesModelComponent,
    TaskPageComponent,
    DeviceAddPageComponent,
    DeviceRegisterPageComponent,
    DeviceGroupsPageComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: LoginComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'home', component: HomePageComponent, canActivate: [AuthGuard] },
      {
        path: 'device', canActivate: [AuthGuard], children: [
          { path: 'add', component: DeviceAddPageComponent, canActivate: [AuthGuard] },
          { path: 'register', component: DeviceRegisterPageComponent, canActivate: [AuthGuard] },
          {
            path: 'groups', canActivate: [AuthGuard], children: [
              { path: '', component: DeviceGroupsPageComponent, canActivate: [AuthGuard] },
              { path: ':id', component: DeviceGroupsPageComponent, canActivate: [AuthGuard] },
              { path: ':id/:mode', component: DeviceGroupsPageComponent, canActivate: [AuthGuard] }
            ]
          },
          { path: ':id', component: DevicePageComponent, canActivate: [AuthGuard] }
        ]
      },
      {
        path: 'action', canActivate: [AuthGuard], children: [
          { path: '', component: ActionListPageComponent, canActivate: [AuthGuard] },
          { path: ':id', component: ActionPageComponent, canActivate: [AuthGuard] }
        ]
      },
      { path: 'task/:id', component: TaskPageComponent, canActivate: [AuthGuard] },
    ]),
    NgxTranslateModule
  ],
  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
