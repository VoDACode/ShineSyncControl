import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceGroupModel } from 'src/app/models/device.group.model';
import { DeviceModel } from 'src/app/models/device.model';
import { DeviceApiService } from 'src/app/services/device-api.service';
import { DeviceGroupsApiService } from 'src/app/services/device-groups-api.service';

@Component({
  selector: 'app-device-groups-page',
  templateUrl: './device-groups-page.component.html',
  styleUrls: ['./device-groups-page.component.css']
})
export class DeviceGroupsPageComponent {
  public id: string = '';
  public editMode: boolean = false;

  public deviceGroups: DeviceGroupModel[] = [];
  public currentGroup: DeviceGroupModel = new DeviceGroupModel();
  public devices: DeviceModel[] = [];

  public deletedDevices: DeviceModel[] = [];
  public addedDevices: DeviceModel[] = [];

  public get isRoot(): boolean {
    return this.id == '';
  }

  public get isNewGroup(): boolean {
    return this.id == 'new';
  }

  public get isEditMode(): boolean {
    return this.editMode && this.id != '' && this.id != 'new';
  }

  public get isViewMode(): boolean {
    return !this.editMode && !this.isNewGroup && !this.isRoot;
  }
  
  public get canAddNewDevice(): boolean {
    return this.devices.length > 0 && (this.currentGroup.devices?.length ?? 0) < this.devices.length;
  }

  constructor(private route: ActivatedRoute, private router: Router, private deviceGroupsApiService: DeviceGroupsApiService, private deviceApiService: DeviceApiService) {
    this.route.params.subscribe(params => {
      this.id = params['id'] || '';
      this.editMode = params['mode'] == 'edit';

      if (this.isNewGroup) {
        this.loadDevices();
      } else {
        if (this.isEditMode) {
          this.loadDevices();
          this.loadGroup();
        } else if (this.isRoot) {
          this.loadGroups();
        } else {
          this.loadGroup();
        }
      }
    });
  }

  public onEditClick(event: boolean): void {
    this.editMode = event;
    if (this.isEditMode) {
      this.loadDevices();
    }
  }


  public addNewDeviceToGroup(): void {
    if (this.currentGroup.devices == null) {
      this.currentGroup.devices = [];
    }
    let device = { ...this.devices[0] };
    this.currentGroup.devices.push(device);
    if (this.isEditMode) {
      this.addedDevices.push(device);
    }
  }

  public removeDeviceFromGroup(device: DeviceModel): void {
    if (this.currentGroup.devices != null) {
      let index = this.currentGroup.devices.indexOf(device);
      if (index > -1) {
        this.currentGroup.devices.splice(index, 1);
      }
    }
    if (this.isEditMode) {
      if (this.addedDevices.indexOf(device) > -1) {
        this.addedDevices.splice(this.addedDevices.indexOf(device), 1);
      } else {
        this.deletedDevices.push(device);
      }
    }
  }

  public onSelectDevice(device: DeviceModel, index: number): void {
    if (this.currentGroup.devices == null) {
      this.currentGroup.devices = [];
    }
    console.log(device, index);
    this.currentGroup.devices[index] = device;
  }

  public saveGroup(): void {
    if (this.isNewGroup) {
      this.deviceGroupsApiService.createGroup(this.currentGroup).subscribe(response => {
        if (response.success == true && response.data != null) {
          this.currentGroup = response.data;
          this.router.navigate(['/device/groups', this.currentGroup.id]);
        }
      });
    } else {
      this.deviceGroupsApiService.updateGroup(this.currentGroup).subscribe(response => {
        if (response.success == true && response.data != null) {
          this.currentGroup = response.data;
          this.router.navigate(['/device/groups', this.currentGroup.id]);
          this.editMode = false;
        }
      });
    }
  }

  private loadGroups(): void {
    this.deviceGroupsApiService.getGroups().subscribe(response => {
      if (response.success == true && response.data != null) {
        this.deviceGroups = response.data;
      }
    });
  }

  private loadGroup(): void {
    this.deviceGroupsApiService.getGroup(Number(this.id)).subscribe(response => {
      if (response.success == true && response.data != null) {
        this.currentGroup = response.data;
      }
    });
  }

  private loadDevices(): void {
    this.deviceApiService.getDevices().subscribe(response => {
      if (response.success == true && response.data != null) {
        this.devices = response.data;
        if(this.currentGroup.devices == null){
          this.currentGroup.devices = [];
        }
      }
    });
  }
}
