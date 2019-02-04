import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckmateDialogComponent } from './checkmate-dialog.component';

describe('CheckmateDialogComponent', () => {
  let component: CheckmateDialogComponent;
  let fixture: ComponentFixture<CheckmateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckmateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckmateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
