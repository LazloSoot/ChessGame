import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpinnerRectComponent } from './spinner-rect.component';

describe('SpinnerRectComponent', () => {
  let component: SpinnerRectComponent;
  let fixture: ComponentFixture<SpinnerRectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SpinnerRectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpinnerRectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
