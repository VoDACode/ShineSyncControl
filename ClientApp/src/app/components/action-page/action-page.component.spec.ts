import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionPageComponent } from './action-page.component';

describe('ActionPageComponent', () => {
  let component: ActionPageComponent;
  let fixture: ComponentFixture<ActionPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActionPageComponent]
    });
    fixture = TestBed.createComponent(ActionPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
