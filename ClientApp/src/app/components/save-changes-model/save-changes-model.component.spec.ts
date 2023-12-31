import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveChangesModelComponent } from './save-changes-model.component';

describe('SaveChangesModelComponent', () => {
  let component: SaveChangesModelComponent;
  let fixture: ComponentFixture<SaveChangesModelComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SaveChangesModelComponent]
    });
    fixture = TestBed.createComponent(SaveChangesModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
