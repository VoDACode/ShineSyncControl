import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionListPageComponent } from './action-list-page.component';

describe('ActionListPageComponent', () => {
  let component: ActionListPageComponent;
  let fixture: ComponentFixture<ActionListPageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ActionListPageComponent]
    });
    fixture = TestBed.createComponent(ActionListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
