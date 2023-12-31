import { TestBed } from '@angular/core/testing';

import { DeviceGroupsApiService } from './device-groups-api.service';

describe('DeviceGroupsApiService', () => {
  let service: DeviceGroupsApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceGroupsApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
