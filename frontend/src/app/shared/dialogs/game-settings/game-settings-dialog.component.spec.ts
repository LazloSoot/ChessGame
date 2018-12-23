import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GameSettingsDialogComponent } from './game-settings-dialog.component';

describe('GameSettingsDialogComponent', () => {
  let component: GameSettingsDialogComponent;
  let fixture: ComponentFixture<GameSettingsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GameSettingsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GameSettingsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
