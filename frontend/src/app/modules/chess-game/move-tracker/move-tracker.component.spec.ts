import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MoveTrackerComponent } from './move-tracker.component';

describe('MoveTrackerComponent', () => {
  let component: MoveTrackerComponent;
  let fixture: ComponentFixture<MoveTrackerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MoveTrackerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MoveTrackerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
