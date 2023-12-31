import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceAddPageComponent } from './device-add-page.component';

describe('DeviceAddPageComponent', () => {
  let component: DeviceAddPageComponent;
  let fixture: ComponentFixture<DeviceAddPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DeviceAddPageComponent]
    });
    fixture = TestBed.createComponent(DeviceAddPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
