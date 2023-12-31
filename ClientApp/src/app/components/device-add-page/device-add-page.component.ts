import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DeviceApiService } from 'src/app/services/device-api.service';
import { SystemApiService } from 'src/app/services/system-api.service';

@Component({
  selector: 'app-device-add-page',
  templateUrl: './device-add-page.component.html',
  styleUrls: ['./device-add-page.component.css']
})
export class DeviceAddPageComponent {
  public showLoading = false;
  public deviceId: string = "";

  public get canBeConnected(): boolean {
    return this.deviceId.length > 0;
  }

  public get deviceActivationTimeOut(): number {
    return this.systemApiService.deviceActivationTimeOut;
  }
  public get waitingText(): string {
    let minutes = Math.floor(this.timeOut / 60);
    let seconds = this.timeOut % 60;
    let secondsString = seconds < 10 ? "0" + seconds : seconds;
    return this.translate.instant("page.add-device.waiting-for-device-activation", { minutes: minutes, seconds: secondsString });
  }

  private timeOutInterval: any;
  private timeOut: number = 0;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private translate: TranslateService, private deviceApiService: DeviceApiService, private systemApiService: SystemApiService) {
    this.activatedRoute.queryParams.subscribe(params => {
      if (params['id']) {
        this.deviceId = params['id'];
        if (this.deviceId != "" && params['auto-activate'] == "true") {
          this.connectDevice();
        }
      }
    });
  }

  public connectDevice() {
    this.showLoading = true;

    this.timeOut = this.deviceActivationTimeOut;
    this.timeOutInterval = setInterval(() => {
      this.timeOut--;
      if (this.timeOut <= 0) {
        clearInterval(this.timeOutInterval);
        this.cancelAction();
      }
    }, 1000);

    this.deviceApiService.connectDevice(this.deviceId).subscribe((response) => {
      this.showLoading = false;
      if (response.success) {
        this.router.navigate(['/device', this.deviceId]);
      } else {
        alert(response.message);
      }
      if(this.timeOutInterval != null){
        clearInterval(this.timeOutInterval);
      }
    });
  }

  public cancelAction() {
    let queryParams = Object.assign({}, this.activatedRoute.snapshot.queryParams);
    queryParams['auto-activate'] = false;
    this.router.navigate([this.router.url.split('?')[0]], { queryParams: queryParams }).then(() => {
      location.href = this.router.url;
    });
  }
}
