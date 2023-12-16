import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceModel } from 'src/app/models/device.model';
import { DevicePropertyModel } from 'src/app/models/property.model';
import { ShortActionModel } from 'src/app/models/short.action.model';
import { DeviceApiService } from 'src/app/services/device-api.service';
import { environment } from 'src/environments/environment';
import { WebSocketSubject, webSocket } from 'rxjs/webSocket';

@Component({
  selector: 'app-device-page',
  templateUrl: './device-page.component.html',
  styleUrls: ['./device-page.component.css']
})
export class DevicePageComponent implements OnInit, OnDestroy {
  public device: DeviceModel = new DeviceModel();
  public deviceProperties: DevicePropertyModel[] = [];
  public deviceActions: ShortActionModel[] = [];
  public editedDevice: DeviceModel = new DeviceModel();

  private ws: WebSocketSubject<any> | undefined;

  public get hasChanges(): boolean {
    if (this.editedDevice.description === undefined || this.editedDevice.description === null || this.editedDevice.description === '') {
      this.editedDevice.description = null;
    }
    if (this.editedDevice.name === undefined || this.editedDevice.name === null || this.editedDevice.name === '') {
      this.editedDevice.name = null;
    }
    return JSON.stringify(this.device) !== JSON.stringify(this.editedDevice);
  }

  public deviceId: string = "1";

  public get onlineStatus(): string {
    if (this.device.lastOnline && this.device.lastOnline.getTime() > 0) {
      let time = new Date().getTime() - this.device.lastOnline.getTime();
      if (time < 1000 * 60) {
        return "Online";
      } else {
        return "Offline for " + Math.floor(time / 1000 / 60) + " minutes";
      }
    } else {
      return "Offline for a long time";
    }
  }

  public editMode: boolean = false;

  constructor(private route: ActivatedRoute, private deviceApiService: DeviceApiService, private router: Router) {
    this.route.params.subscribe(params => {
      this.deviceId = params['id'];
    });
  }
  ngOnDestroy(): void {
    console.log('Destroying websocket');
    console.log(this.ws);
    this.ws?.complete();
    console.log(this.ws);
    this.ws = undefined;
  }

  public ngOnInit() {
    this.loadDevice();
  }

  private loadDevice() {
    this.deviceApiService.getDevice(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.device = res.data;
        this.editedDevice = Object.assign({}, this.device);
        this.ws = webSocket(`${environment.webSocketUrl}/api/device/ws/${this.deviceId}/update`);
        this.ws.subscribe((msg) => this.onMessage(msg));
        this.loadProperties();
        this.loadActions();
      } else {
        alert(res.message);
        this.router.navigate(['/home']);
      }
    });
  }

  private loadProperties() {
    this.deviceApiService.getDeviceProperties(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.deviceProperties = res.data;
        var canBeEdited = this.deviceProperties.filter(x => !x.isReadOnly).sort((a, b) => a.propertyName.localeCompare(b.propertyName));
        var readOnly = this.deviceProperties.filter(x => x.isReadOnly).sort((a, b) => a.propertyName.localeCompare(b.propertyName));
        this.deviceProperties = [...canBeEdited, ...readOnly];
      } else {
        alert(res.message);
      }
    });
  }

  private loadActions() {
    this.deviceApiService.getDeviceActions(this.deviceId).subscribe(res => {
      if (res.success && res.data) {
        this.deviceActions = res.data;
      } else {
        alert(res.message);
      }
    });
  }

  public onEditClick(status: boolean) {
    this.editMode = status;
    if (status) {
      this.editedDevice = Object.assign({}, this.device);
    }
  }

  public onCancelClick() {
    this.editMode = false;
  }

  public onSaveClick() {
    this.deviceApiService.updateDevice(this.editedDevice).subscribe(res => {
      if (res.success && res.data) {
        this.device = res.data;
        this.editedDevice = Object.assign({}, this.device);
        this.editMode = false;
      } else {
        alert(res.message);
      }
    });
  }

  public onSetProperty(property: DevicePropertyModel, value: any = null) {
    value = value == null ? property.value : (value ? '1' : '0');
    this.deviceApiService.setProperty(this.deviceId, property.propertyName, value).subscribe(res => {
      if (res.success && res.data) {
        property.value = res.data.value;
      } else {
        alert(res.message);
      }
    });
  }

  private onMessage(data: any) {
    console.log(data);
    this.device.lastOnline = new Date(data.lastOnline);
    this.deviceProperties.forEach(x => {
      var property = data.properties.find((y: any) => y.name == x.propertyName);
      if (property && property.value != x.value) {
        console.log('Updating property ' + x.propertyName + ' to ' + property.value);
        x.value = property.value;
      }
    });

    // let time = Number(data);
    // this.device.lastOnline = new Date(time);
  }

}
