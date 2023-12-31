import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceGroupsPageComponent } from './device-groups-page.component';

describe('DeviceGroupsPageComponent', () => {
  let component: DeviceGroupsPageComponent;
  let fixture: ComponentFixture<DeviceGroupsPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DeviceGroupsPageComponent]
    });
    fixture = TestBed.createComponent(DeviceGroupsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
