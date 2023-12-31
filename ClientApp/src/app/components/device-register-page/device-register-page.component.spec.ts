import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceRegisterPageComponent } from './device-register-page.component';

describe('DeviceRegisterPageComponent', () => {
  let component: DeviceRegisterPageComponent;
  let fixture: ComponentFixture<DeviceRegisterPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DeviceRegisterPageComponent]
    });
    fixture = TestBed.createComponent(DeviceRegisterPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
